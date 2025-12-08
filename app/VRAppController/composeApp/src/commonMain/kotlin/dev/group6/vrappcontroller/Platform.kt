package dev.group6.vrappcontroller

import androidx.compose.foundation.ScrollState
import androidx.compose.runtime.Composable

interface Platform {
    val name: String
}

expect fun getPlatform(): Platform

expect fun getLocalIP(): String?

@Composable
expect fun VerticalScrollbar(scrollState: ScrollState)

@Composable
expect fun HorizontalScrollbar(scrollState: ScrollState)