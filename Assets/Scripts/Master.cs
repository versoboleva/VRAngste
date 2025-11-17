using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using NUnit;

public class Master : MonoBehaviour 
{
    public SoundSystem sound;
    public StormSystem storm;
    public ApiClient api;
    public Envelope envelope;
    public string host = "127.0.0.1";
    public string nonce = "ABCD";

    private int SceneNr;
    private int windIntencity;
    private int thunderVolume;
    private float distance;
    private int rainIntencity;
    private int clouds;
    private float lightningIntencity;
    private float lightningFrequency;


    private void Start()
    {
        if(sound == null)
        {
            sound = FindAnyObjectByType<SoundSystem>();
        }
        if(storm == null)
        {
            storm = FindAnyObjectByType<StormSystem>();
        }
        if (api == null)
        {
            api = FindAnyObjectByType<ApiClient>();
        }
        ConnectToServer();
        ApiClient.Instance.OnBytesReceived += HandleEnvelope; // <- call function on event/does the same as https://developer.mozilla.org/en-US/docs/Web/API/EventTarget/addEventListener
    }

    private void HandleEnvelope(Envelope envelope)
    {
        switch (envelope.PayloadCase)
        {
            case Envelope.PayloadOneofCase.RainSetting:
                rainIntencity = (int) envelope.RainSetting.Level;
                storm.SetRegen(rainIntencity);
                Debug.Log("Rain level:" + rainIntencity);
                break;

            case Envelope.PayloadOneofCase.WindSetting: 
                windIntencity = (int) envelope.WindSetting.Level;
                sound.SetWind(windIntencity);
                Debug.Log("Wind level:" + windIntencity);
                break;

            case Envelope.PayloadOneofCase.ThunderSetting: 
                thunderVolume = (int) envelope.ThunderSetting.Level;
                sound.SetThunder(thunderVolume);
                Debug.Log("Thunder level:" +thunderVolume);
                break;

            case Envelope.PayloadOneofCase.CloudDensitySetting: 
                clouds = (int) envelope.CloudDensitySetting.Level;
                storm.SetWolken(clouds);
                Debug.Log("Cloud density:" +clouds);
                break;

            /*case Envelope.PayloadOneofCase.SceneChangeSetting:
                sceneNr = envelope....
                sound.SetSceneNr(sceneNr);
                Debug.Log($"Scene index: {scene.Index}");
                break;*/

            case Envelope.PayloadOneofCase.LightningBrightnessSetting:
                lightningIntencity = envelope.LightningFrequencySetting.Scale;
                storm.SetHelligkeit(lightningIntencity);
                Debug.Log("Lightning brightness:"+ lightningIntencity);
                break;

            case Envelope.PayloadOneofCase.LightningFrequencySetting: 
                lightningFrequency = envelope.LightningFrequencySetting.Scale;
                Debug.Log("Lightning frequency"+ lightningFrequency);
                break;

            case Envelope.PayloadOneofCase.LightningDistanceSetting:
                distance = envelope.LightningDistanceSetting.Scale;
                storm.SetDistance(distance);
                Debug.Log("Distance: "+ distance);
                break;

            case Envelope.PayloadOneofCase.PanicReport: //heist irgendwann mal PanicEvent
                //add panic event
                Debug.Log("Panic event!");
                break;

            case Envelope.PayloadOneofCase.LoginFailed:
                Debug.Log("Login failed!");
                break;

            case Envelope.PayloadOneofCase.None:
            default:
                Debug.LogWarning("Envelope had no payload");
                break;
        }
        
    }

    public void ConnectToServer() // connect to server

    {
        ApiClient.Instance.Connect(this.nonce, this.host);
    }

    


}
