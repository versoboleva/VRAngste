package dev.group6.vrappcontroller

import android.os.Build
import androidx.compose.foundation.ScrollState
import androidx.compose.runtime.Composable
import java.net.Inet4Address
import java.net.NetworkInterface

class AndroidPlatform : Platform {
    override val name: String = "Android ${Build.VERSION.SDK_INT}"
}

actual fun getPlatform(): Platform = AndroidPlatform()

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
 //Nothing
}

@Composable
actual fun HorizontalScrollbar(scrollState: ScrollState) {
 //Nothing
}