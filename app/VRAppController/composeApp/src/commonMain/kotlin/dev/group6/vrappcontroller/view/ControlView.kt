package dev.group6.vrappcontroller.view

import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.border
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.HorizontalDivider
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Slider
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import dev.group6.vrappcontroller.HorizontalScrollbar
import dev.group6.vrappcontroller.VerticalScrollbar
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

    val horizontalScrollState = rememberScrollState(0)
    val verticalScrollState = rememberScrollState(0)

    val selectedScene by viewModel.selectedScene.collectAsState()

    Row {
        Column(
            modifier = Modifier.weight(1f).fillMaxHeight().verticalScroll(verticalScrollState).padding(32.dp),
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
            Category("Szenen Wechsel") {
                Column {
                    Row(
                        modifier = Modifier.fillMaxWidth().horizontalScroll(horizontalScrollState),
                    ) {
                        SceneCard(
                            "Leere Szene",
                            onClick = { viewModel.setSelectedScene(0) },
                            selected = selectedScene == 0
                        )
                        Spacer(modifier = Modifier.width(16.dp))
                        SceneCard(
                            "Haus",
                            onClick = { viewModel.setSelectedScene(1) },
                            selected = selectedScene == 1
                        )
                        Spacer(modifier = Modifier.width(16.dp))
                        SceneCard(
                            "Terrasse",
                            onClick = { viewModel.setSelectedScene(2) },
                            selected = selectedScene == 2
                        )
                        Spacer(modifier = Modifier.width(16.dp))
                        SceneCard(
                            "Auto",
                            onClick = { viewModel.setSelectedScene(3) },
                            selected = selectedScene == 3
                        )
                    }
                    Spacer(modifier = Modifier.height(8.dp))
                    HorizontalScrollbar(horizontalScrollState)
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