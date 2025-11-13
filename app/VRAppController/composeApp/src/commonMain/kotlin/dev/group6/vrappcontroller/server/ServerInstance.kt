package dev.group6.vrappcontroller.server

object ServerInstance {
    private val server: Server

    init {
        val randomNonce = generateNonce()
        server = Server(port = 35614, nonce = randomNonce)
        println("Server created on port 35614 with nonce: $randomNonce")
    }

    private fun generateNonce(): String {
        val chars = ('A'..'Z') + ('a'..'z') + ('0'..'9')
        return (1..4).map { chars.random() }.joinToString("")
    }

    fun start() = server.start()

    fun stop() = server.stop()

    suspend fun broadcast(msg: Envelope) = server.broadcast(msg)
}