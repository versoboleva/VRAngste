package dev.group6.vrappcontroller.server

import kotlinx.serialization.ExperimentalSerializationApi
import kotlinx.serialization.Serializable
import kotlinx.serialization.protobuf.ProtoNumber

@Serializable
@OptIn(ExperimentalSerializationApi::class)
data class Envelope(
    @ProtoNumber(1) val rain_setting: RainSetting? = null,
    @ProtoNumber(2) val wind_setting: WindSetting? = null,
    @ProtoNumber(3) val thunder_setting: ThunderSetting? = null,
    @ProtoNumber(4) val cloud_density_setting: CloudDensitySetting? = null,
    @ProtoNumber(5) val lightning_brightness_setting: LightningBrightnessSetting? = null,
    @ProtoNumber(6) val lightning_frequency_setting: LightningFrequencySetting? = null,
    @ProtoNumber(7) val lightning_distance_setting: LightningDistanceSetting? = null,
    @ProtoNumber(8) val lightning_report: LightningReport? = null,
    @ProtoNumber(9) val thunder_report: ThunderReport? = null,
    @ProtoNumber(10) val panic_report: PanicReport? = null,
    @ProtoNumber(11) val login_failed: LoginFailed? = null,
    @ProtoNumber(12) val login_success: LoginSuccess? = null,
    @ProtoNumber(13) val scene_change_setting: SceneChangeSetting? = null,
    @ProtoNumber(14) val announce_lightning_report: AnnounceLightningReport? = null
)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class RainSetting(@ProtoNumber(1) val level: UInt)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class SceneChangeSetting(@ProtoNumber(1) val index: UInt)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class AnnounceLightningReport(
    @ProtoNumber(1) val distance: ULong,
    @ProtoNumber(2) val duration: ULong,
    @ProtoNumber(3) val duration_until_start: ULong
)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class WindSetting(@ProtoNumber(1) val level: UInt)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class ThunderSetting(@ProtoNumber(1) val level: UInt)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class CloudDensitySetting(@ProtoNumber(1) val level: UInt)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class LightningBrightnessSetting(@ProtoNumber(1) val scale: Float)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class LightningFrequencySetting(@ProtoNumber(1) val scale: Float)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class LightningDistanceSetting(@ProtoNumber(1) val scale: Float)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class LightningReport(
    @ProtoNumber(1) val distance: ULong,
    @ProtoNumber(2) val duration: ULong
)

@OptIn(ExperimentalSerializationApi::class)
@Serializable
data class ThunderReport(@ProtoNumber(1) val intensity: Float)

@Serializable
class PanicReport

@Serializable
class LoginFailed

@Serializable
class LoginSuccess
