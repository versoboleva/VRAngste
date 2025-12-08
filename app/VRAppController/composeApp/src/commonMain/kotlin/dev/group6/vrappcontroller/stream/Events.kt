package dev.group6.vrappcontroller.stream

import io.ktor.websocket.*
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.*
import kotlin.collections.mutableSetOf
import kotlin.time.Clock
import kotlin.time.ExperimentalTime

@Serializable
data class Answer @OptIn(ExperimentalTime::class) constructor(val sdp: String, val datetime: Long = Clock.System.now().toEpochMilliseconds())

@Serializable
data class Offer @OptIn(ExperimentalTime::class) constructor(val sdp: String, val datetime: Long = Clock.System.now().toEpochMilliseconds(), var polite: Boolean = false)

@Serializable
data class Candidate @OptIn(ExperimentalTime::class) constructor(
    val candidate: String,
    val sdpMLineIndex: Int,
    val sdpMid: String,
    val datetime: Long = Clock.System.now().toEpochMilliseconds()
)

val clients = mutableMapOf<WebSocketSession, MutableSet<String>>()

fun reset() {
    clients.clear()
}

fun add(ws: WebSocketSession) {
    clients[ws] = mutableSetOf()
}

fun remove(ws: WebSocketSession) {
    clients.remove(ws)
}

suspend fun onConnect(ws: WebSocketSession, connectionId: String) {
    val polite = true

    clients.getOrPut(ws) { mutableSetOf() }.add(connectionId)
    sendJson(ws, connectionId, "connect", JsonPrimitive(polite), true)
}

suspend fun onDisconnect(ws: WebSocketSession, connectionId: String) {
    clients[ws]?.remove(connectionId)
    sendJson(ws, connectionId, "disconnect")
}

suspend fun onOffer(ws: WebSocketSession, connectionId: String, sdp: String) {
    val offer = Offer(sdp)
    sendJson(ws, connectionId, "offer", Json.encodeToJsonElement(offer))
}

suspend fun onAnswer(ws: WebSocketSession, connectionId: String, sdp: String) {
    val answer = Answer(sdp)
    clients.getOrPut(ws) { mutableSetOf() }.add(connectionId)
    sendJson(ws, connectionId, "answer", Json.encodeToJsonElement(answer))
}

suspend fun onCandidate(
    ws: WebSocketSession,
    connectionId: String,
    candidateStr: String,
    sdpMLineIndex: Int,
    sdpMid: String
) {
    val candidate = Candidate(candidateStr, sdpMLineIndex, sdpMid)
    sendJson(ws, connectionId, "candidate", Json.encodeToJsonElement(candidate))
}


private suspend fun sendJson(
    ws: WebSocketSession,
    connectionId: String,
    type: String,
    extra: JsonElement? = null,
    answer: Boolean = false
) {
    val json = buildJsonObject {
        put("connectionId", Json.parseToJsonElement("\"$connectionId\""))
        put("from", Json.parseToJsonElement("\"$connectionId\""))
        put("to", Json.parseToJsonElement("\"\""))
        put("type", Json.parseToJsonElement("\"$type\""))
        extra?.let { put("data", it) }
    }

    clients.forEach {

        if ((it.key == ws) == answer) {
            it.key.send(Frame.Text(json.toString()))
        }
    }
}