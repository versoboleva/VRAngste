using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;
using Google.Protobuf;

public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance { get; private set; }

    [Header("Connection Settings")]
    public string host = "127.0.0.1";
    public int port = 9000;
    public string kind = "game";

    public bool IsConnected => client != null && client.Connected;

    public event Action<byte[]> OnBytesReceived;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool running = false;

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

    /// <summary>
    /// Connects to the server with given credentials
    /// </summary>
    public void Connect(string username, string password)
    {
        if (IsConnected)
        {
            Debug.LogWarning("Already connected.");
            return;
        }

        try
        {
            client = new TcpClient();
            client.Connect(host, port);
            stream = client.GetStream();

            var authObj = new
            {
                user = username,
                password = password,
                kind = kind
            };
            string json = JsonUtility.ToJson(authObj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);

            byte[] resp = new byte[1024];
            int len = stream.Read(resp, 0, resp.Length);
            string respStr = Encoding.UTF8.GetString(resp, 0, len);

            if (respStr.Trim() != "AUTH_OK")
            {
                Debug.LogError("Authentication failed!");
                Disconnect();
                return;
            }

            Debug.Log("Authenticated successfully!");
            running = true;

            receiveThread = new Thread(ReceiveLoop)
            {
                IsBackground = true
            };
            receiveThread.Start();
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
        try { stream?.Close(); } catch { }
        try { client?.Close(); } catch { }
        stream = null;
        client = null;

        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Join(100);
        receiveThread = null;

        Debug.Log("Disconnected from server.");
    }

    /// <summary>
    /// Sends string message with a length prefix(32bit)
    /// </summary>
    public void SendString(string message)
    {
        if (!IsConnected || stream == null)
        {
            Debug.LogWarning("Not connected to server");
            return;
        }

        try
        {
            byte[] payload = Encoding.UTF8.GetBytes(message);
            byte[] lengthPrefix = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(payload.Length));
            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            stream.Write(payload, 0, payload.Length);
            stream.Flush();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Send error: {ex.Message}");
            Disconnect();
        }
    }

    private void ReceiveLoop()
    {
        try
        {
            while (running && client != null && client.Connected)
            {
                byte[] lengthBuffer = new byte[4];
                int bytesRead = stream.Read(lengthBuffer, 0, 4);
                if (bytesRead == 0)
                {
                    Debug.Log("Server closed connection");
                    break;
                }

                int length = System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBuffer, 0));
                byte[] data = new byte[length];
                int totalRead = 0;

                while (totalRead < length)
                {
                    int read = stream.Read(data, totalRead, length - totalRead);
                    if (read == 0) break;
                    totalRead += read;
                }

                if (totalRead == length)
                {
                    OnBytesReceived?.Invoke(data);
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

    private void OnApplicationQuit()
    {
        Disconnect();
    }
}
