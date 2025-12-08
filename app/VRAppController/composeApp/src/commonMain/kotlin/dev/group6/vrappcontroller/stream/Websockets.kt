package dev.group6.vrappcontroller.stream

import io.ktor.http.*
import io.ktor.server.application.*
import io.ktor.server.response.*
import io.ktor.server.routing.*
import io.ktor.server.websocket.*
import io.ktor.websocket.*
import kotlinx.coroutines.launch
import kotlinx.serialization.json.*
import vrappcontroller.composeapp.generated.resources.Res


fun Application.module() {
    install(WebSockets)
    reset()

    routing {
        route("/") {
            get("{...}") {
                val segments = call.parameters.getAll("...") ?: emptyList()
                val path = if (segments.isEmpty()) "index.html" else segments.joinToString("/")
                val resourcePath = "files/web/$path"
                try {
                    val bytes = Res.readBytes(resourcePath)
                    call.respondBytes(
                        bytes,
                        ContentType.defaultForFilePath(path)
                    )
                } catch (_: Exception) {
                    call.respond(HttpStatusCode.NotFound)

                }
            }
            webSocket("") {
                add(this)
                for (frame in incoming) {
                    if (frame is Frame.Text) {

                        val text = frame.readText()

                        val msg = Json.decodeFromString<Map<String, JsonElement>>(text)

                        val type = msg["type"]?.jsonPrimitive?.content ?: continue
                        val ws = this
                        when (type) {
                            "connect" -> {
                                val connectionId = msg["connectionId"]?.jsonPrimitive?.content ?: continue
                                launch { onConnect(ws, connectionId) }
                            }

                            "disconnect" -> {
                                val connectionId = msg["connectionId"]?.jsonPrimitive?.content ?: continue
                                launch { onDisconnect(ws, connectionId) }
                            }

                            "offer" -> {
                                val connectionId = msg["from"]?.jsonPrimitive?.content ?: continue
                                val sdp = (msg["data"] as JsonObject)["sdp"]?.jsonPrimitive?.content ?: continue

                                launch { onOffer(ws, connectionId, sdp) }
                            }

                            "answer" -> {
                                val connectionId = msg["from"]?.jsonPrimitive?.content ?: continue
                                val sdp = (msg["data"] as JsonObject)["sdp"]?.jsonPrimitive?.content ?: continue
                                launch { onAnswer(ws, connectionId, sdp) }
                            }

                            "candidate" -> {
                                val connectionId = msg["from"]?.jsonPrimitive?.content ?: continue
                                val data = (msg["data"] as JsonObject)
                                val candidate = data["candidate"]?.jsonPrimitive?.content ?: continue
                                val sdpMLineIndex = data["sdpMLineIndex"]?.jsonPrimitive?.intOrNull ?: continue
                                val sdpMid = data["sdpMid"]?.jsonPrimitive?.content ?: continue

                                launch { onCandidate(ws, connectionId, candidate, sdpMLineIndex, sdpMid) }
                            }

                            else -> {}
                        }
                    }
                }
            }
        }

    }
}