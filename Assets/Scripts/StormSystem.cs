using TMPro;
using UnityEngine;
using UnityEngine.Android;
using System.Collections;
using UnityEngine.SceneManagement;

public class StormSystem : MonoBehaviour
{
    private LightningController LightningController;
    private ParticleSystem Rain;
    private ParticleSystem PorchDroplets;
    private Skylight Flash;
    private SoundSystem Sound;
    private CloudController clouds;
    private Coroutine rainCorutine;
    
    
    public Vector3 position = new Vector3(0,40,0);
    private Vector3 positionCheck = new Vector3(0,40,0);

    public int scale = 1;
    private int scaleCheck = 1;

    public int cloudLevel = 0;        // 0–3
    private int cloudLevelCheck = 0;

    public int emitionLightning = 100;
    private int emitionLightningCheck = 100;

    public int emitionRain = 0;
    private int emitionRainCheck = 0;

    public float flashIntencity = 1;
    private float flashIntencityCheck = 1;

    public float speed = 5f;

    private void Start()
    {
        if(Flash == null)
        {
            Flash = FindAnyObjectByType<Skylight>();
        }
        if (Rain == null)
        {
            Rain = GameObject.FindGameObjectWithTag("RainSystem")?.GetComponent<ParticleSystem>();
        }
        if (PorchDroplets == null)
        {
            PorchDroplets = GameObject.FindGameObjectWithTag("PorchDroplets")?.GetComponent<ParticleSystem>();
        }
        if (Sound == null)
        {
            Sound = FindAnyObjectByType<SoundSystem>();
        }
        if (LightningController == null)
        {
            LightningController = FindAnyObjectByType<LightningController>();
        }
        if (clouds == null)
        {
            clouds = GameObject.FindGameObjectWithTag("Clouds")?.GetComponent<CloudController>();
        }
        LightningController.position = position;
        
        LightningController.scale = new Vector3(scale * 10, scale * 10, 1);

        LightningController.SetLightningInterval(emitionLightning);

        SetRainEmition();

        Flash.flashIntensity = flashIntencity;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (rainCorutine != null)
        {
            StopCoroutine(rainCorutine);
            rainCorutine = null;
        }

        Flash = FindAnyObjectByType<Skylight>();
        Rain = GameObject.FindGameObjectWithTag("RainSystem")?.GetComponent<ParticleSystem>();
        PorchDroplets = GameObject.FindGameObjectWithTag("PorchDroplets")?.GetComponent<ParticleSystem>();
        LightningController = FindAnyObjectByType<LightningController>();
        Sound = FindAnyObjectByType<SoundSystem>();
        clouds = GameObject.FindGameObjectWithTag("Clouds")?.GetComponent<CloudController>();
        scale = 1;
        cloudLevel = 0;
        emitionLightning = 100;
        emitionRain = 0;
        flashIntencity = 1;
    }
    void FixedUpdate()
    {
        if(position != positionCheck)
        {
            MoveToPosition();
        }

        if (scale != scaleCheck)
        {
            LightningController.scale = new Vector3(scale * 10, scale * 10, 1);
            scaleCheck = scale;
        }

        if (cloudLevel != cloudLevelCheck)
        {
            if (clouds != null)
                clouds.SetCloudDensity(cloudLevel);
            cloudLevelCheck = cloudLevel;
        }


        if (emitionLightning != emitionLightningCheck)
        {
            LightningController.SetLightningInterval(emitionLightning);
        }

        if (emitionRain != emitionRainCheck)
        {
            SetRainEmition();
            emitionRainCheck = emitionRain;
        }

        if (flashIntencity != flashIntencityCheck)
        {
            Flash.flashIntensity = flashIntencity;
            flashIntencityCheck = flashIntencity;
        }

    }
    public void SetStorm(float distance, int wolken, int intervalBlitz, int regen, int helligkeitBlitz)
    {
        position = new Vector3(0, 40, distance*100);

        cloudLevel = wolken;
        
        emitionLightning = intervalBlitz/1;
        
        emitionRain = regen;

        flashIntencity = helligkeitBlitz /2;
    }

    public void SetDistance( float distance)
    {
        position = new Vector3(0, 40, distance*100);
    }

    public void SetWolken(int wolken)
    {
        cloudLevel = wolken;

        if (clouds != null)
        {
            clouds.SetCloudDensity(wolken);   
        }
    }

    public void SetInterval(int interval)
    {
        emitionLightning = interval;
    }

    public void SetHelligkeit(float helligkeitBlitz)
    {
        flashIntencity = helligkeitBlitz / 2;
    }

    public void SetRegen(int regen)
    {
        emitionRain = regen;
    }

    private void MoveToPosition()
    {
        if (LightningController != null && LightningController.position != position) 
        {
            LightningController.position = Vector3.MoveTowards(LightningController.position, position, speed * Time.deltaTime);
        }
        positionCheck = LightningController.position;
    }

    private void SetRainEmition()
    {
        Sound.SetRainIntencity(emitionRain);

        float rate = 0;

        if (emitionRain == 1) rate = 100;
        if (emitionRain == 2) rate = 500;
        if (emitionRain == 3) rate = 1000;

        rainCorutine = StartCoroutine(LerpRainEmission(rate, 5f));  
    }

    private IEnumerator LerpRainEmission(float targetRate, float duration)
    {
        var emission = Rain.emission;
        float startRate = emission.rateOverTime.constant;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float newRate = Mathf.Lerp(startRate, targetRate, t);
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(newRate);

            yield return null;
        }

        emission.rateOverTime = new ParticleSystem.MinMaxCurve(targetRate);
    }

}
