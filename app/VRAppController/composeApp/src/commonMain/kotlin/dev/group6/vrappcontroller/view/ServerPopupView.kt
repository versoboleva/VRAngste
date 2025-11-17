package dev.group6.vrappcontroller.view

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.unit.dp
import androidx.compose.ui.window.Dialog
import dev.group6.vrappcontroller.model.ServerPopupModel

@Composable
fun ServerPopupView(
    model: ServerPopupModel,
    onDismiss: () -> Unit,
) {

    val nonce by model.nonce.collectAsState()
    val ip by model.ip.collectAsState()

    Dialog(
        onDismissRequest = onDismiss,
    ) {
        ElevatedCard {
            Column {
                OutlinedTextField(
                    modifier = Modifier
                        .padding(24.dp)
                        .fillMaxWidth(),
                    value = ip,
                    readOnly = true,
                    onValueChange = {},
                    label = { Text("IP address") },
                )
                OutlinedTextField(
                    modifier = Modifier
                        .padding(24.dp, 0.dp, 24.dp, 24.dp)
                        .fillMaxWidth(),
                    value = nonce,
                    readOnly = true,
                    onValueChange = {},
                    textStyle = MaterialTheme.typography.bodyLarge.copy(fontFamily = FontFamily.Monospace),
                    label = { Text("Nonce") },
                )
                Button(
                    onClick = onDismiss,
                    modifier = Modifier
                        .padding(24.dp, 0.dp, 24.dp, 24.dp)
                        .fillMaxWidth(),
                ) {
                    Text("Okay")
                }
            }
        }
    }
}