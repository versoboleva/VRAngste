package dev.group6.vrappcontroller.model

import androidx.lifecycle.ViewModel
import dev.group6.vrappcontroller.getLocalIP
import dev.group6.vrappcontroller.server.ServerInstance
import kotlinx.coroutines.flow.MutableStateFlow

class ServerPopupModel() : ViewModel() {
    var server: ServerInstance = ServerInstance

    var _nonce: MutableStateFlow<String> = MutableStateFlow("")
    val nonce: MutableStateFlow<String> = _nonce

    var _ip: MutableStateFlow<String> = MutableStateFlow("")
    val ip: MutableStateFlow<String> = _ip

    init {
        startServer()
    }

    private fun startServer() {
        server.start()
        _nonce.value = server.nonce
        _ip.value = getLocalIP() ?: ""
    }
}