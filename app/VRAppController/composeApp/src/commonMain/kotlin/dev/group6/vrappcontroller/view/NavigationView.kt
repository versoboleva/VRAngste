package dev.group6.vrappcontroller.view

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Timeline
import androidx.compose.material.icons.filled.Vrpano
import androidx.compose.material.icons.filled.Wifi
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import dev.group6.vrappcontroller.getPlatform
import dev.group6.vrappcontroller.model.ControlModel
import dev.group6.vrappcontroller.model.DataModel
import dev.group6.vrappcontroller.model.StreamModel

@Composable
fun NavigationView(
    contentPadding: PaddingValues,
    controlModel: ControlModel,
    dataModel: DataModel,
    streamModel: StreamModel,
    reopenServerPopup: () -> Unit
) {

    var selected by remember { mutableIntStateOf(0) }

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
            NavigationRailItem(
                selected = false,
                onClick = reopenServerPopup,
                icon = { Icon(Icons.Default.Wifi, null) },
                label = { Text("Server Info") }
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