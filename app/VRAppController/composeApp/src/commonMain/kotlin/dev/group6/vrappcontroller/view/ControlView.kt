package dev.group6.vrappcontroller.view

import androidx.compose.foundation.border
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.HorizontalDivider
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Slider
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import dev.group6.vrappcontroller.HorizontalScrollbar
import dev.group6.vrappcontroller.VerticalScrollbar
import dev.group6.vrappcontroller.model.ControlModel
import kotlinx.coroutines.delay
import kotlin.math.roundToInt
import kotlin.time.Clock
import kotlin.time.ExperimentalTime

@OptIn(ExperimentalTime::class)
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

    val horizontalScrollState = rememberScrollState(0)
    val verticalScrollState = rememberScrollState(0)

    val selectedScene by viewModel.selectedScene.collectAsState()
    val remainingThunderMs by viewModel.remainingThunderMs.collectAsState()
    val nextThunderTimestamp by viewModel.nextThunderTimestamp.collectAsState()
    val maxThunderCountdown by viewModel.maxThunderCountdown.collectAsState()

    Row {
        Column(
            modifier = Modifier
                .weight(1f)
                .fillMaxHeight()
                .verticalScroll(verticalScrollState)
                .padding(32.dp),
        ) {
            Category("Timer") {

                ThunderDisplay(remainingThunderMs, maxThunderCountdown)
            }
            Category("Szenen Wechsel") {
                FlowRow(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.spacedBy(16.dp),
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    SceneCard(
                        "Leere Szene",
                        { viewModel.setSelectedScene(0); viewModel.resetToDefault() },
                        selectedScene == 0
                    )
                    SceneCard("Haus", { viewModel.setSelectedScene(1); viewModel.resetToDefault() }, selectedScene == 1)
                    SceneCard(
                        "Terrasse",
                        { viewModel.setSelectedScene(2); viewModel.resetToDefault() },
                        selectedScene == 2
                    )
                    SceneCard("Auto", { viewModel.setSelectedScene(3); viewModel.resetToDefault() }, selectedScene == 3)
                }
            }

            Category("Interval") {
                SubCategory("Intervall Blitz") {
                    Slider(
                        value = lightningInterval.toFloat(),
                        onValueChange = { viewModel.setLightningIntervalWithoutEnv(it.roundToInt()) },
                        onValueChangeFinished = viewModel::setLightningInterval,
                        valueRange = 0f..100f,
                    )
                    Text("Blitzschläge alle ${lightningInterval}s")
                }
            }

            Category("Wetter") {
                SubCategory("Regen") { StrengthSlider(rain, viewModel::setRain) }
                SubCategory("Wind") { StrengthSlider(wind, viewModel::setWind) }
                SubCategory("Wolken") { StrengthSlider(clouds, viewModel::setClouds) }
            }

            Category("Blitz/Donner") {
                SubCategory("Lautstärke Donner") {
                    Slider(thunderVolume, viewModel::setThunderVolumeWithoutEnv, onValueChangeFinished = viewModel::setThunderVolume)
                    Text("${(thunderVolume * 100).roundToInt()}%")
                }
                SubCategory("Helligkeit Blitz") {
                    Slider(lightningBrightness, viewModel::setLightningBrightnessWithoutEnv, onValueChangeFinished = viewModel::setLightningBrightness)
                    Text("${(lightningBrightness * 100).roundToInt()}%")
                }
                SubCategory("Distanz Blitz") {
                    Slider(lightningDistance, viewModel::setLightningDistanceWithoutEnv, onValueChangeFinished = viewModel::setLightningDistance)
                    Text("${(lightningDistance * 100).roundToInt()}m")
                }
            }
        }
        VerticalScrollbar(verticalScrollState)
        Spacer(modifier = Modifier.width(8.dp))
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
            modifier = Modifier.fillMaxWidth().padding(horizontal = 16.dp),
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
    value: Int, onValueChange: (Int) -> Unit
) {
    val labels = listOf("Aus", "Wenig", "Mittel", "Stark")

    Column(
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Slider(
            value = value.toFloat(), onValueChange = { newValue ->
                onValueChange(newValue.roundToInt().coerceIn(0, 3))
            }, valueRange = 0f..3f, steps = 2, modifier = Modifier.fillMaxWidth()
        )

        Row(
            modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween
        ) {
            labels.forEach { label ->
                Text(text = label, style = MaterialTheme.typography.labelSmall)
            }
        }
    }
}

@Composable
fun SceneCard(name: String, onClick: () -> Unit, selected: Boolean = false) {
    val borderColor = if (selected) MaterialTheme.colorScheme.primary else Color.Transparent
    val borderWidth = if (selected) 2.dp else 0.dp
    Card(
        modifier = Modifier.border(borderWidth, borderColor, shape = MaterialTheme.shapes.medium),
        onClick = onClick,
    ) {
        Box(modifier = Modifier.fillMaxSize().padding(48.dp), contentAlignment = Alignment.Center) {
            Text(text = name)
        }
    }
}

@Composable
fun ThunderDisplay(
    remainingThunderMs: Long,
    maxThunderCountdown: Long
) {
    Column(
        modifier = Modifier.padding(16.dp),
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        LinearProgressIndicator(
            progress = remainingThunderMs.toFloat() / maxThunderCountdown.toFloat(),
            modifier = Modifier
                .fillMaxWidth()
                .height(12.dp),
            color = MaterialTheme.colorScheme.primary,
            trackColor = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.1f)
        )
        Text(
            text = "Nächster Donner in ${remainingThunderMs / 1000}s",
            style = MaterialTheme.typography.bodyMedium
        )
    }
}