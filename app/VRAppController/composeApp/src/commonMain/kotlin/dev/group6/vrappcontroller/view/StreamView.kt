package dev.group6.vrappcontroller.view

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import dev.group6.vrappcontroller.model.StreamModel

@Composable
fun StreamView(
    viewModel: StreamModel,
) {
    Box(
        modifier = Modifier.fillMaxHeight().fillMaxWidth(0.66f),
    ) {
        Text(text = "Stream View")
    }
}