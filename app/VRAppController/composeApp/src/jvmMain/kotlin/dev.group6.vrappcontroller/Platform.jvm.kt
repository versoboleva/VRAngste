package dev.group6.vrappcontroller

import androidx.compose.foundation.HorizontalScrollbar
import androidx.compose.foundation.LocalScrollbarStyle
import androidx.compose.foundation.ScrollState
import androidx.compose.foundation.ScrollbarStyle
import androidx.compose.foundation.VerticalScrollbar
import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.rememberScrollbarAdapter
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import java.net.Inet4Address
import java.net.NetworkInterface
import dev.group6.vrappcontroller.ui.theme.*

class JVMPlatform : Platform {
    override val name: String = "Desktop"
}


actual fun getPlatform(): Platform = JVMPlatform()

actual fun getLocalIP(): String? {
    return NetworkInterface.getNetworkInterfaces()
        .toList()
        .filter { iface ->
            iface.isUp &&
                    !iface.isLoopback &&
                    !iface.displayName.contains("docker", ignoreCase = true) &&
                    !iface.displayName.contains("br-", ignoreCase = true) &&
                    !iface.displayName.contains("veth", ignoreCase = true)
        }
        .flatMap { it.inetAddresses.toList() }
        .filterIsInstance<Inet4Address>()
        .firstOrNull { addr ->
            !addr.isLoopbackAddress &&
                    addr.hostAddress.startsWith("192.") || addr.hostAddress.startsWith("10.") || addr.hostAddress.startsWith(
                "172."
            )
        }?.hostAddress
}

@Composable
actual fun VerticalScrollbar(scrollState: ScrollState) {
    VerticalScrollbar(
        modifier = Modifier,
        adapter = rememberScrollbarAdapter(scrollState),
        style = scrollbarStyle()
    )
}

@Composable
actual fun HorizontalScrollbar(scrollState: ScrollState) {
    HorizontalScrollbar(
        modifier = Modifier,
        adapter = rememberScrollbarAdapter(scrollState),
        style = scrollbarStyle()
    )
}

@Composable
fun scrollbarStyle(): ScrollbarStyle {
    val dark = isSystemInDarkTheme()

    val railColor = if (dark) outlineVariantDark else outlineVariantLight
    val thumbColorRaw = if (dark) onSurfaceDark else onSurfaceLight

    val rail = railColor.copy(alpha = 0.45f)
    val thumb = thumbColorRaw.copy(alpha = 0.75f)

    val style = ScrollbarStyle(
        minimalHeight = 18.dp,
        thickness = 8.dp,
        shape = LocalScrollbarStyle.current.shape,
        unhoverColor = rail,
        hoverColor = thumb,
        hoverDurationMillis = 150,
    )

    return style
}