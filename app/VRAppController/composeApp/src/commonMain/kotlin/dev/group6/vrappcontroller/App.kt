package dev.group6.vrappcontroller

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.WindowInsets
import androidx.compose.foundation.layout.systemBars
import androidx.compose.foundation.layout.windowInsetsPadding
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Surface
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import dev.group6.vrappcontroller.model.ControlModel
import dev.group6.vrappcontroller.model.DataModel
import dev.group6.vrappcontroller.model.ServerPopupModel
import dev.group6.vrappcontroller.model.StreamModel
import dev.group6.vrappcontroller.ui.theme.AppTheme
import dev.group6.vrappcontroller.view.NavigationView
import dev.group6.vrappcontroller.view.ServerPopupView

@Composable
fun App() {

    val controlModel = ControlModel()
    val dataModel = DataModel()
    val streamModel = StreamModel()
    val serverPopupModel = ServerPopupModel()

    var isServerPopupVisible by remember { mutableStateOf(true) }

    AppTheme {
        Surface {
            Scaffold(
                modifier = Modifier.windowInsetsPadding(WindowInsets.systemBars)
            ) { contentPadding ->
                Box(
                    contentAlignment = Alignment.Center,
                ) {
                    if (isServerPopupVisible)
                        ServerPopupView(
                            serverPopupModel,
                            onDismiss = {
                                isServerPopupVisible = false
                            }
                        )
                    NavigationView(
                        contentPadding,
                        controlModel,
                        dataModel,
                        streamModel,
                        reopenServerPopup = { isServerPopupVisible = true }
                    )
                }
            }
        }
    }
}