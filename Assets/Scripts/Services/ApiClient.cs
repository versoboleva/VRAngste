using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;

public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance { get; private set; }

    [Header("Connection Settings")]
    public int port = 35614;

    public bool IsConnected => client != null && client.Connected;
    public event Action<Envelope> OnBytesReceived;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool running = false;
    private readonly ConcurrentQueue<Envelope> receivedQueue = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Connect(string nonce, string host)
    {
        if (IsConnected)
        {
            Debug.LogWarning("Already connected.");
            return;
        }

        if (nonce.Length != 4)
        {
            Debug.LogError("Nonce must be exactly 4 ASCII characters.");
            return;
        }

        try
        {
            client = new TcpClient();
            client.Connect(host, port);
            stream = client.GetStream();

            byte[] nonceBytes = Encoding.ASCII.GetBytes(nonce);
            stream.Write(nonceBytes, 0, nonceBytes.Length);
            stream.Flush();

            running = true;
            receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
            receiveThread.Start();

            Debug.Log("Connected");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Connection error: {ex.Message}");
            Disconnect();
        }
    }

    public void Disconnect()
    {
        running = false;

        try { stream?.Dispose(); } catch { }
        try { client?.Dispose(); } catch { }

        stream = null;
        client = null;

        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Join(100);

        receiveThread = null;

        Debug.Log("Disconnected");
    }

    public void Send(Envelope envelope)
    {
        if (!IsConnected || stream == null)
        {
            Debug.LogWarning("Not connected to server");
            return;
        }

        try
        {
            byte[] data = envelope.ToByteArray();
            byte[] lengthPrefix = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(data.Length));
            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Send error: {ex.Message}");
            Disconnect();
        }
    }

    private bool ReadExact(NetworkStream stream, byte[] buffer, int count)
    {
        int offset = 0;
        while (offset < count)
        {
            int read = stream.Read(buffer, offset, count - offset);
            if (read == 0) return false;
            offset += read;
        }
        return true;
    }

    private void ReceiveLoop()
    {
        try
        {
            while (running && client != null && client.Connected)
            {
                byte[] lengthBuffer = new byte[4];
                if (!ReadExact(stream, lengthBuffer, 4))
                    break;

                int length = System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBuffer, 0));
                if (length <= 0) continue;

                byte[] data = new byte[length];
                if (!ReadExact(stream, data, length))
                    break;

                try
                {
                    var envelope = Envelope.Parser.ParseFrom(data);
                    receivedQueue.Enqueue(envelope);
                }
                catch (Exception parseEx)
                {
                    Debug.LogError($"Protobuf parse error: {parseEx.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Receive loop ended: {ex.Message}");
        }
        finally
        {
            Disconnect();
        }
    }

    private void Update()
    {
        while (receivedQueue.TryDequeue(out var envelope))
            OnBytesReceived?.Invoke(envelope);
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }
}
