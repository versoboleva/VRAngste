package dev.group6.vrappcontroller

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.WindowInsets
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.systemBars
import androidx.compose.foundation.layout.windowInsetsPadding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Timeline
import androidx.compose.material.icons.filled.Vrpano
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.ElevatedButton
import androidx.compose.material3.HorizontalDivider
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.NavigationRail
import androidx.compose.material3.NavigationRailItem
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.material3.VerticalDivider
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.dropShadow
import androidx.compose.ui.graphics.Shadow
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import dev.group6.vrappcontroller.model.ControlModel
import dev.group6.vrappcontroller.model.DataModel
import dev.group6.vrappcontroller.model.StreamModel
import dev.group6.vrappcontroller.ui.theme.AppTheme
import dev.group6.vrappcontroller.view.ControlView
import dev.group6.vrappcontroller.view.DataView
import dev.group6.vrappcontroller.view.StreamView

@Composable
fun App() {
    val controlModel = ControlModel()
    val dataModel = DataModel()
    val streamModel = StreamModel()

    var selected by remember { mutableIntStateOf(0) }
    AppTheme {
        Surface {

            Scaffold(
                modifier = Modifier.windowInsetsPadding(WindowInsets.systemBars)
            ) { contentPadding ->
                Row {
                    NavigationRail(
                        modifier = Modifier.padding(contentPadding)
                    ) {
                        NavigationRailItem(
                            selected = selected == 0,
                            onClick = { selected = 0 },
                            icon = { Icon(Icons.Default.Vrpano, null) },
                            label = { Text("Steuerung") }
                        )
                        NavigationRailItem(
                            selected = selected == 1,
                            onClick = { selected = 1 },
                            icon = { Icon(Icons.Default.Timeline, null) },
                            label = { Text("Daten") }
                        )
                    }

                    //Change screen based on selection
                    when (selected) {
                        0 -> Box(
                            modifier = Modifier.fillMaxSize(),
                            contentAlignment = Alignment.TopCenter
                        ) {
                            Row(modifier = Modifier.fillMaxSize()) {
                                StreamView(streamModel)
                                VerticalDivider(thickness = 2.dp)
                                ControlView(controlModel)
                            }
                            Column {
                                Spacer(Modifier.fillMaxHeight(0.85f))
                                ElevatedButton(
                                    colors = ButtonDefaults.elevatedButtonColors().copy(
                                        containerColor = MaterialTheme.colorScheme.error,
                                        contentColor = MaterialTheme.colorScheme.onError,
                                    ),
                                    onClick = {}
                                ) {
                                    Text(
                                        modifier = Modifier.padding(16.dp, 8.dp),
                                        text = "STOP",
                                        style = if (getPlatform().name == "Desktop")
                                            MaterialTheme.typography.bodyLarge.copy(fontSize = 24.sp)
                                        else MaterialTheme.typography.bodyLarge
                                    )
                                }
                            }
                        }

                        1 -> DataView(dataModel)
                    }
                }
            }
        }
    }
}