using UnityEngine;
using NUnit;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Master : MonoBehaviour 
{
    public SoundSystem sound;
    public StormSystem storm;
    public ApiClient api;
    public Envelope envelope;
    public SceneSetter sceneSetter;
    public string host = "127.0.0.1";
    public string nonce = "ABCD";

    public TMP_InputField hostField;
    public TMP_InputField nonceField;
    public TMP_InputField portField;

    private int sceneNr;
    private float thunderVolume;
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
        if (sceneSetter == null)
        {
            sceneSetter = FindAnyObjectByType<SceneSetter>();
        }
        if (api == null)
        {
            api = FindAnyObjectByType<ApiClient>();
        }
        ConnectToServer();
        ApiClient.Instance.OnBytesReceived += HandleEnvelope; // <- call function on event/does the same as https://developer.mozilla.org/en-US/docs/Web/API/EventTarget/addEventListener
    }

    private void Awake()
    {
        if (FindObjectsByType<SoundSystem>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sound = FindAnyObjectByType<SoundSystem>();
        storm = FindAnyObjectByType<StormSystem>();
        sceneSetter = FindAnyObjectByType<SceneSetter>();
        api = FindAnyObjectByType<ApiClient>();

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

            case Envelope.PayloadOneofCase.ThunderSetting: 
                thunderVolume = envelope.ThunderSetting.Scale;
                sound.SetThunder(thunderVolume);
                Debug.Log("Thunder volume:" +thunderVolume);
                break;

            case Envelope.PayloadOneofCase.CloudDensitySetting: 
                clouds = (int) envelope.CloudDensitySetting.Level;
                storm.SetWolken(clouds);
                Debug.Log("Cloud density:" +clouds);
                break;

            case Envelope.PayloadOneofCase.SceneChangeSetting:
                sceneNr = (int) envelope.SceneChangeSetting.Index; 
                sceneSetter.ChangeScene(sceneNr); 
                sound.currentScene = sceneNr;
                Debug.Log("Scene index received: " + sceneNr);
                break;


            case Envelope.PayloadOneofCase.LightningBrightnessSetting:
                lightningIntencity = envelope.LightningFrequencySetting.Scale;
                storm.SetHelligkeit(lightningIntencity);
                Debug.Log("Lightning brightness:"+ lightningIntencity);
                break;

            case Envelope.PayloadOneofCase.LightningFrequencySetting: 
                lightningFrequency = envelope.LightningFrequencySetting.Scale;
                storm.SetInterval((int)lightningFrequency);
                Debug.Log("Lightning frequency"+ lightningFrequency);
                break;

            case Envelope.PayloadOneofCase.LightningDistanceSetting:
                distance = envelope.LightningDistanceSetting.Scale;
                storm.SetDistance(distance);
                Debug.Log("Distance: "+ distance);
                break;

            case Envelope.PayloadOneofCase.PanicEvent: 
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

    public void SetHost(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            this.host = value;
            Debug.Log("Host set to: " + value);
        }

        Debug.Log("Host string null");
    }

    public void SetNunce(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            this.nonce = value;
        Debug.Log("Nonce set to: " + value);
        }

        Debug.Log("Nunce string null");
    }

    // Diese Methode verbinden wir mit dem Keyboard Submit Event
    public void OnKeyboardSubmit()
    {
        // Lies direkt aus dem InputField
        string hostText = hostField.text;
        string nonceText = nonceField.text;

        this.host = hostText;
        this.nonce = nonceText;

        Debug.Log("Host set to: " + hostText);
        Debug.Log("Nonce set to: " + nonceText);

        ApiClient.Instance.Connect(this.nonce, this.host);
    }

}

    
