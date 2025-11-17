package dev.group6.vrappcontroller.view

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.HorizontalDivider
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Slider
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import dev.group6.vrappcontroller.model.ControlModel
import kotlin.math.roundToInt

@Composable
fun ControlView(
    viewModel: ControlModel
) {

    val thunderVolume by viewModel.thunderVolume.collectAsState()
    val lightningBrightness by viewModel.lightningBrightness.collectAsState()
    val lightningDistance by viewModel.lightningDistance.collectAsState()

    val rain by viewModel.rain.collectAsState()
    val wind by viewModel.wind.collectAsState()
    val clouds by viewModel.clouds.collectAsState()

    val lightningInterval by viewModel.lightningInterval.collectAsState()

    Column(
        modifier = Modifier.fillMaxHeight().fillMaxWidth()
            .verticalScroll(rememberScrollState())
            .padding(32.dp),
    ) {
        Category("Blitz/Donner") {
            SubCategory("Lautstärke Donner") {
                Slider(
                    value = thunderVolume,
                    onValueChange = viewModel::setThunderVolume,
                )
                Text("${(thunderVolume * 100).roundToInt()}%")
            }
            SubCategory("Helligkeit Blitz") {
                Slider(
                    value = lightningBrightness,
                    onValueChange = viewModel::setLightningBrightness,
                )
                Text("${(lightningBrightness * 100).roundToInt()}%")
            }
            SubCategory("Distanz Blitz") {
                Slider(
                    value = lightningDistance,
                    onValueChange = viewModel::setLightningDistance,
                )
                Text("${(lightningDistance * 100).roundToInt()}m")
            }

        }
        Category("Wetter") {
            SubCategory("Regen") {
                StrengthSlider(
                    value = rain,
                    onValueChange = viewModel::setRain,
                )
            }
            SubCategory("Wind") {
                StrengthSlider(
                    value = wind,
                    onValueChange = viewModel::setWind,
                )
            }
            SubCategory("Wolken") {
                StrengthSlider(
                    value = clouds,
                    onValueChange = viewModel::setClouds,
                )
            }
        }
        Category("Interval") {
            SubCategory("Intervall Blitz") {
                Slider(
                    value = lightningInterval.toFloat(),
                    onValueChange = { viewModel.setLightningInterval(it.roundToInt()) },
                    valueRange = 0f..100f,
                )
                Text("Blitzschläge alle ${lightningInterval}s")
            }
        }
    }
}

@Composable
fun Category(name: String, content: @Composable () -> Unit) {
    Column {
        Text(
            name,
            style = MaterialTheme.typography.titleMedium,
            modifier = Modifier.fillMaxWidth(),
            textAlign = TextAlign.Center,
        )

        Spacer(modifier = Modifier.height(8.dp))

        HorizontalDivider(
            modifier = Modifier
                .fillMaxWidth()
                .padding(horizontal = 16.dp),
            color = MaterialTheme.colorScheme.outline,
            thickness = 1.dp
        )

        Spacer(modifier = Modifier.height(16.dp))

        content()

        Spacer(modifier = Modifier.height(32.dp))
    }
}

@Composable
fun SubCategory(name: String, content: @Composable () -> Unit) {
    Column {
        Text(
            name,
            style = MaterialTheme.typography.titleSmall,
            modifier = Modifier.fillMaxWidth(),
            textAlign = TextAlign.Left,
        )

        Spacer(modifier = Modifier.height(4.dp))
        Column(
            modifier = Modifier.padding(horizontal = 16.dp)
        ) {
            content()
        }

        Spacer(modifier = Modifier.height(16.dp))
    }
}

@Composable
fun StrengthSlider(
    value: Int,
    onValueChange: (Int) -> Unit
) {
    val labels = listOf("Aus", "Wenig", "Mittel", "Stark")

    Column(
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Slider(
            value = value.toFloat(),
            onValueChange = { newValue ->
                onValueChange(newValue.roundToInt().coerceIn(0, 3))
            },
            valueRange = 0f..3f,
            steps = 2,
            modifier = Modifier.fillMaxWidth()
        )

        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween
        ) {
            labels.forEach { label ->
                Text(text = label, style = MaterialTheme.typography.labelSmall)
            }
        }
    }
}