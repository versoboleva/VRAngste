package dev.group6.vrappcontroller.model

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import dev.group6.vrappcontroller.server.*
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.launch
import kotlin.math.roundToInt
import kotlin.time.Clock
import kotlin.time.ExperimentalTime

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
    var _stormSize = MutableStateFlow(1.0f)
    val stormSize: MutableStateFlow<Float> = _stormSize

    var _rain = MutableStateFlow(0)
    val rain: MutableStateFlow<Int> = _rain
    var _wind = MutableStateFlow(0)
    val wind: MutableStateFlow<Int> = _wind
    var _clouds = MutableStateFlow(0)
    val clouds: MutableStateFlow<Int> = _clouds

    var _lightningInterval = MutableStateFlow(100)
    val lightningInterval: MutableStateFlow<Int> = _lightningInterval

    var _selectedScene = MutableStateFlow(0)
    val selectedScene: MutableStateFlow<Int> = _selectedScene

    var _nextThunderTimestamp = MutableStateFlow(0L)
    val nextThunderTimestamp: MutableStateFlow<Long> = _nextThunderTimestamp

    var _maxThunderCountdown: MutableStateFlow<Long> = MutableStateFlow(1L)
    val maxThunderCountdown: MutableStateFlow<Long> = _maxThunderCountdown

    private var thunderCountdownJob: Job? = null

    private val _remainingThunderMs = MutableStateFlow(0L)
    val remainingThunderMs: MutableStateFlow<Long> = _remainingThunderMs

    init {
        _thunderVolume.value = 1.0f
        _lightningBrightness.value = 1.0f
        _lightningDistance.value = 1.0f
    }

    fun setThunderVolume() {
        sendEnvelope(
            Envelope(
                thunder_setting = ThunderSetting(_thunderVolume.value)
            )
        )
    }

    fun setThunderVolumeWithoutEnv(value: Float) {
        _thunderVolume.value = value
    }

    fun setLightningBrightness() {
        sendEnvelope(
            Envelope(
                lightning_brightness_setting = LightningBrightnessSetting(_lightningBrightness.value)
            )
        )
    }

    fun setLightningBrightnessWithoutEnv(value: Float) {
        _lightningBrightness.value = value
    }

    fun setLightningDistance() {
        sendEnvelope(
            Envelope(
                lightning_distance_setting = LightningDistanceSetting(_lightningDistance.value)
            )
        )
    }

    fun setLightningDistanceWithoutEnv(value: Float) {
        _lightningDistance.value = value
    }

    fun resetToDefault() {
        _thunderVolume.value = 1.0f
        _lightningBrightness.value = 1.0f
        _lightningDistance.value = 1.0f
        _stormSize.value = 1.0f
        _rain.value = 0
        _wind.value = 0
        _clouds.value = 0
        _lightningInterval.value = 100
    }

    fun setStormSize(value: Float) {
        _stormSize.value = value
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

    fun setLightningInterval() {
        sendEnvelope(
            Envelope(
                lightning_frequency_setting = LightningFrequencySetting(_lightningInterval.value.toFloat())
            )
        )
    }

    fun setLightningIntervalWithoutEnv(value: Int) {
        _lightningInterval.value = value
    }

    fun setSelectedScene(value: Int) {
        _selectedScene.value = value
        sendEnvelope(
            Envelope(
                scene_change_setting = SceneChangeSetting(_selectedScene.value.toUInt())
            )
        )
    }

    fun sendEnvelope(envelope: Envelope) {
        viewModelScope.launch {
            server.broadcast(envelope)
        }
    }

    @OptIn(ExperimentalTime::class)
    fun setNextTimerTimestamp(timestamp: Long) {
        _nextThunderTimestamp.value = timestamp
        _maxThunderCountdown.value = timestamp - Clock.System.now().toEpochMilliseconds()

        thunderCountdownJob?.cancel()
        thunderCountdownJob = viewModelScope.launch {
            while (true) {
                val now = Clock.System.now().toEpochMilliseconds()
                val remaining = timestamp - now

                if (remaining <= 0) {
                    _remainingThunderMs.value = 0L
                    break
                }

                _remainingThunderMs.value = remaining
                delay(1000) // 1s Takt
            }
        }
    }

    fun stopThunderCountdown() {
        thunderCountdownJob?.cancel()
        _remainingThunderMs.value = 0L
        _nextThunderTimestamp.value = 0L
    }
}