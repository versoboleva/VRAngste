package dev.group6.vrappcontroller.model

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import dev.group6.vrappcontroller.server.*
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.launch

/**
 * Model for handling the slider values.
 * Use the _<variableName> MutableStateFlow to update the variable value.
 * Outside the model use the given methods.
 * To watch for changes use val <variable> by viewModel.<variable>.collectAsState()
 */
class ControlModel() : ViewModel() {

    val server: ServerInstance = ServerInstance

    var _thunderVolume = MutableStateFlow(1.0f)
    val thunderVolume: MutableStateFlow<Float> = _thunderVolume
    var _lightningBrightness = MutableStateFlow(1.0f)
    val lightningBrightness: MutableStateFlow<Float> = _lightningBrightness
    var _lightningDistance = MutableStateFlow(1.0f)
    val lightningDistance: MutableStateFlow<Float> = _lightningDistance

    var _rain = MutableStateFlow(0)
    val rain: MutableStateFlow<Int> = _rain
    var _wind = MutableStateFlow(0)
    val wind: MutableStateFlow<Int> = _wind
    var _clouds = MutableStateFlow(0)
    val clouds: MutableStateFlow<Int> = _clouds

    var _lightningInterval = MutableStateFlow(10)
    val lightningInterval: MutableStateFlow<Int> = _lightningInterval

    init {
        _thunderVolume.value = 1.0f
        _lightningBrightness.value = 1.0f
        _lightningDistance.value = 1.0f
    }

    fun setThunderVolume(value: Float) {
        _thunderVolume.value = value
        sendEnvelope(
            Envelope(
                thunder_setting = ThunderSetting(_thunderVolume.value.toUInt())
            )
        )
    }

    fun setLightningBrightness(value: Float) {
        _lightningBrightness.value = value
        sendEnvelope(
            Envelope(
                lightning_brightness_setting = LightningBrightnessSetting(_lightningBrightness.value)
            )
        )
    }

    fun setLightningDistance(value: Float) {
        _lightningDistance.value = value
        sendEnvelope(
            Envelope(
                lightning_distance_setting = LightningDistanceSetting(_lightningDistance.value)
            )
        )
    }

    fun setRain(value: Int) {
        _rain.value = value
        sendEnvelope(
            Envelope(
                rain_setting = RainSetting(_rain.value.toUInt())
            )
        )
    }

    fun setWind(value: Int) {
        _wind.value = value
        sendEnvelope(
            Envelope(
                wind_setting = WindSetting(_wind.value.toUInt())
            )
        )
    }

    fun setClouds(value: Int) {
        _clouds.value = value
        sendEnvelope(
            Envelope(
                cloud_density_setting = CloudDensitySetting(_clouds.value.toUInt())
            )
        )
    }

    fun setLightningInterval(value: Int) {
        _lightningInterval.value = value
        sendEnvelope(
            Envelope(
                lightning_frequency_setting = LightningFrequencySetting(_lightningInterval.value.toFloat())
            )
        )
    }

    fun sendEnvelope(envelope: Envelope) {
        viewModelScope.launch {
            server.broadcast(envelope)
        }
    }
}