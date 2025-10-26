import socket
import json
import time
import struct

HOST = "127.0.0.1"
PORT = 9000

USER = "user"
PASSWORD = "password"
KIND = "sensor"


def send_message(sock: socket.socket, data: bytes):
    """Send a message with a 4-byte length prefix."""
    sock.sendall(struct.pack("!I", len(data)) + data)


def main():
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect((HOST, PORT))

    init_msg = json.dumps({"user": USER, "password": PASSWORD, "kind": KIND})
    s.sendall(init_msg.encode("utf-8"))

    resp = s.recv(1024)
    if resp != b"AUTH_OK":
        print("Auth failed!")
        s.close()
        return
    print("Authenticated successfully!")

    for i in range(3):
        payload = f"message {i}".encode("utf-8")
        send_message(s, payload)
        send_message(s, payload)
        print(f"Sent: {payload}")
        time.sleep(1)

    s.close()


if __name__ == "__main__":
    main()
