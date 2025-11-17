package dev.group6.vrappcontroller

interface Platform {
    val name: String
}

expect fun getPlatform(): Platform

expect fun getLocalIP(): String?