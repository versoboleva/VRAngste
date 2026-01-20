package dev.group6.vrappcontroller.server

import dev.group6.vrappcontroller.model.ControlModel
import dev.group6.vrappcontroller.stream.module
import io.ktor.server.application.Application
import io.ktor.server.cio.CIO
import io.ktor.server.cio.CIOApplicationEngine
import io.ktor.server.engine.EmbeddedServer
import io.ktor.server.engine.embeddedServer

object ServerInstance {
    private val server: Server
    private var isRunning = false
    val nonce: String
    private var ktorEngine: EmbeddedServer<CIOApplicationEngine, CIOApplicationEngine.Configuration>? = null

    public var ControlModel: ControlModel? = null
    init {
        val randomNonce = generateNonce()
        nonce = randomNonce
        server = Server(port = 35614, nonce = randomNonce, ControlModel)
        println("Server created on port 35614 with nonce: $randomNonce")
    }

    private fun generateNonce(): String {
        val chars = ('A'..'Z') + ('a'..'z') + ('0'..'9')
        return (1..4).map { chars.random() }.joinToString("")
    }

    fun start() {
        if (isRunning) return
        server.start()

        ktorEngine = embeddedServer(
            CIO,
            port = 35615,
            module = Application::module
        ).also { it.start(wait = false) }
        isRunning = true
    }

    fun stop() {
        if (!isRunning) return
        ktorEngine?.stop(
            gracePeriodMillis = 1000,
            timeoutMillis = 5000
        )
        ktorEngine = null
        server.stop()
        isRunning = false
    }

    suspend fun broadcast(msg: Envelope) = server.broadcast(msg)
}