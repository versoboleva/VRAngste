package dev.group6.vrappcontroller.server

import dev.group6.vrappcontroller.model.ControlModel
import dev.group6.vrappcontroller.view.ControlView
import kotlinx.serialization.ExperimentalSerializationApi

@OptIn(ExperimentalSerializationApi::class)
fun fromEnvelope(msg: Envelope, controlModel: ControlModel?) {
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

        msg.announce_lightning_report != null -> {
            controlModel?.setNextTimerTimestamp(msg.announce_lightning_report.timestamp.toLong())
        }

        else -> throw IllegalArgumentException("illegal payload")
    }
}