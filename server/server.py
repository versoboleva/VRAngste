import socket
import threading
import json
import struct
from influxdb_client import InfluxDBClient, Point, WritePrecision
from datetime import datetime
import os

HOST = "0.0.0.0"
PORT = 8080
DB_HOST = os.getenv("DB_HOST", "localhost")
DB_PORT = int(os.getenv("DB_PORT", 8086))
DB_TOKEN = os.getenv("DB_TOKEN", "YOUR_API_TOKEN")
DB_ORG = os.getenv("DB_ORG", "YOUR_ORG_NAME")
DB_BUCKET = os.getenv("DB_BUCKET", "YOUR_BUCKET_NAME")

USERS = {
    "user": {"password": "password"},
}


# Example connection
client = InfluxDBClient(url=f"http://{DB_HOST}:{DB_PORT}", token=DB_TOKEN, org=DB_ORG)

write_api = client.write_api()
query_api = client.query_api()


def recv_exact(conn: socket.socket, length: int) -> bytes:
    data = b""
    while len(data) < length:
        chunk = conn.recv(length - len(data))
        if not chunk:
            raise ConnectionError("Connection closed while receiving data")
        data += chunk
    return data


def recv_message(conn: socket.socket) -> bytes:
    raw_len = recv_exact(conn, 4)
    msg_len = struct.unpack("!I", raw_len)[0]
    return recv_exact(conn, msg_len)


def handle_client(conn: socket.socket, addr):
    try:
        init_data = conn.recv(1024)
        if not init_data:
            print(f"[DISCONNECT] {addr} sent no data.")
            conn.close()
            return

        try:
            init_json = json.loads(init_data.decode("utf-8"))
        except json.JSONDecodeError:
            print(f"[ERROR] {addr} sent invalid JSON: {init_data!r}")
            conn.close()
            return

        user = init_json.get("user")
        password = init_json.get("password")
        kind = init_json.get("kind")

        user_entry = USERS.get(user)
        if not user_entry or user_entry["password"] != password:
            conn.sendall(b"AUTH_FAIL")
            conn.close()
            return

        conn.sendall(b"AUTH_OK")
        while True:
            try:
                msg = recv_message(conn)
                msg = msg.decode("utf-8")
            except ConnectionError:
                break
            # TODO: parse msg

            point = (
                Point(kind)
                .tag("user", user)
                .field("value", msg)
                .time(datetime.utcnow(), WritePrecision.NS)
            )

            write_api.write(bucket=DB_BUCKET, record=point)

            print(f"[{user}] Received({len(msg)} bytes): {msg!r}")

    except Exception as e:
        print(f"[ERROR] {addr}: {e}")

    finally:
        conn.close()


def main():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server.bind((HOST, PORT))
    server.listen()

    print(f"[STARTED] Server listening on {HOST}:{PORT}")

    try:
        while True:
            conn, addr = server.accept()
            thread = threading.Thread(
                target=handle_client, args=(conn, addr), daemon=True
            )
            thread.start()
    except KeyboardInterrupt:
        print("\n[SHUTDOWN] Server stopping...")
    finally:
        server.close()


if __name__ == "__main__":
    main()
