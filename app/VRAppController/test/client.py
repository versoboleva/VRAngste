import socket
import struct

HOST = "192.168.178.97"
NONCE = "xwlS"

PORT = 35614


def recv_exact(sock, n):
    """Receive exactly n bytes or raise."""
    buf = b""
    while len(buf) < n:
        chunk = sock.recv(n - len(buf))
        if not chunk:
            raise ConnectionError("Connection closed")
        buf += chunk
    return buf


def main():
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect((HOST, PORT))

    # ---- send nonce ----
    s.sendall(NONCE.encode("ascii"))
    print("Nonce sent. Waiting for data...")

    while True:
        try:
            size_bytes = recv_exact(s, 4)
        except Exception as e:
            print("Disconnected:", e)
            break

        msg_len = struct.unpack("!I", size_bytes)[0]

        # ---- read <msg_len> bytes ----
        data = recv_exact(s, msg_len)

        print("Received size bytes:", size_bytes)
        print("Received payload:", data)
        print("-" * 40)


if __name__ == "__main__":
    main()