package dev.group6.vrappcontroller.server

import kotlinx.serialization.ExperimentalSerializationApi

@OptIn(ExperimentalSerializationApi::class)
fun fromEnvelope(msg: Envelope) {
    when {
        msg.lightning_report != null -> {
            println(msg.lightning_report)
        }

        msg.thunder_report != null -> {
            println(msg.thunder_report)
        }

        msg.panic_report != null -> {
            println("PANIC BUTTON PRESSED")
        }

        else -> throw IllegalArgumentException("illegal payload")
    }
}