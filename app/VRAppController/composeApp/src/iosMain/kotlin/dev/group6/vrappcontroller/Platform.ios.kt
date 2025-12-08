package dev.group6.vrappcontroller

import androidx.compose.foundation.ScrollState
import androidx.compose.runtime.Composable
import platform.UIKit.UIDevice

class IOSPlatform : Platform {
    override val name: String = UIDevice.currentDevice.systemName() + " " + UIDevice.currentDevice.systemVersion
}

actual fun getPlatform(): Platform = IOSPlatform()

actual fun getLocalIP(): String? {
    return null //TODO: Pls implement. I can't test
}

@Composable
actual fun VerticalScrollbar(scrollState: ScrollState) {
// Nothing
}

@Composable
actual fun HorizontalScrollbar(scrollState: ScrollState) {
// Nothing
}