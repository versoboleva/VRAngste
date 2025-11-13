package dev.group6.vrappcontroller.server

import io.ktor.network.selector.*
import io.ktor.network.sockets.*
import io.ktor.utils.io.*
import kotlinx.atomicfu.atomic
import kotlinx.atomicfu.update
import kotlinx.coroutines.*
import kotlinx.serialization.ExperimentalSerializationApi
import kotlinx.serialization.decodeFromByteArray
import kotlinx.serialization.encodeToByteArray
import kotlinx.serialization.protobuf.ProtoBuf
import kotlin.collections.emptyList
import kotlin.time.Duration.Companion.seconds


fun intToBytes(value: Int): ByteArray = byteArrayOf(
    ((value ushr 24) and 0xFF).toByte(),
    ((value ushr 16) and 0xFF).toByte(),
    ((value ushr 8) and 0xFF).toByte(),
    (value and 0xFF).toByte()
)

fun bytesToInt(bytes: ByteArray): Int {
    require(bytes.size == 4) { "ByteArray must be exactly 4 bytes long" }
    return (bytes[0].toInt() and 0xFF shl 24) or
            (bytes[1].toInt() and 0xFF shl 16) or
            (bytes[2].toInt() and 0xFF shl 8) or
            (bytes[3].toInt() and 0xFF)
}

/**
 * @param port The port number to listen on. Must be a 16-bit unsigned integer (U16).
 * @param nonce A 4-character ASCII string used for handshake.
 */
class Server(private val port: Int, private val nonce: String) {
    private var serverJob: Job? = null
    private val clients = atomic<List<ClientHandler>>(emptyList())
    private val scope = CoroutineScope(Dispatchers.IO + SupervisorJob())
    private var serverSocket: ServerSocket? = null
    @OptIn(ExperimentalSerializationApi::class)
    fun start() {
        serverJob = scope.launch {
            val selectorManager = SelectorManager(Dispatchers.IO)
            serverSocket = aSocket(selectorManager).tcp().bind(hostname = "0.0.0.0", port = port)
            while (isActive && serverSocket != null) {
                val socket = serverSocket!!.accept()
                val handler = ClientHandler(socket)
                if (handler.validateNonce()) {
                    clients.update { it + handler }
                    handler.start()
                } else {
                    val msg = ProtoBuf.encodeToByteArray(Envelope(login_failed = LoginFailed()))
                    handler.send(msg)
                    socket.close()
                }
            }
        }
    }

    fun stop() {
        serverJob?.cancel()
        clients.value.forEach { it.stop() }
        clients.getAndSet(emptyList())
        scope.cancel()
        serverSocket?.close()
    }

    @OptIn(ExperimentalSerializationApi::class)
    suspend fun broadcast(msg: Envelope) {
        val outBytes = ProtoBuf.encodeToByteArray(msg)


        clients.value.forEach { client ->
            client.send(outBytes)
        }
    }

    private inner class ClientHandler(private val socket: Socket) {
        private val input: ByteReadChannel = socket.openReadChannel()
        private val output: ByteWriteChannel = socket.openWriteChannel(autoFlush = true)
        private var job: Job? = null

        fun start() {
            job = scope.launch {
                receiveMessages()
            }
        }

        suspend fun validateNonce(): Boolean {
            return try {
                val received = ByteArray(4)
                withTimeout(1.seconds) {
                    input.readFully(received, 0, 4)
                }
                val receivedSecret = received.decodeToString()
                receivedSecret == nonce
            } catch (_: Exception) {
                false
            }
        }

        suspend fun send(data: ByteArray) {
            try {
                withContext(Dispatchers.IO) {
                    val sizeBuf = intToBytes(data.size)
                    output.writeFully(sizeBuf)
                    output.writeFully(data)
                }
            } catch (_: Exception) {
                stop()
            }
        }

        @OptIn(ExperimentalSerializationApi::class)
        private suspend fun receiveMessages() {
            try {
                while (true) {
                    val sizeBuf = ByteArray(4)
                    input.readFully(sizeBuf, 0, 4)
                    val size = bytesToInt(sizeBuf)
                    val data = ByteArray(size)
                    input.readFully(data)

                    val msg = ProtoBuf.decodeFromByteArray<Envelope>(data)

                    fromEnvelope(msg)
                }
            } catch (_: Exception) {
            } finally {
                stop()
            }
        }

        fun stop() {
            job?.cancel()
            clients.update { it - this }
            socket.close()
        }
    }
}