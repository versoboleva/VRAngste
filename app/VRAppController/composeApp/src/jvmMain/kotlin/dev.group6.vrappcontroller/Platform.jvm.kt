package dev.group6.vrappcontroller

import java.net.Inet4Address
import java.net.NetworkInterface

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