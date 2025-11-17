using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class StormSystem : MonoBehaviour
{
    private LightningController LightningController;
    private ParticleSystem Rain;
    private Skylight Flash;
    private SoundSystem Sound;
    //add cloud system
    
    
    public Vector3 position = new Vector3(0,40,0);
    private Vector3 positionCheck = new Vector3(0,40,0);

    public int scale = 1;
    private int scaleCheck = 1;

    public int emitionLightning = 0;
    private int emitionLightningCheck = 0;

    public int emitionRain = 1;
    private int emitionRainCheck = 1;

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
        if (Sound == null)
        {
            Sound = FindAnyObjectByType<SoundSystem>();
        }
        if (LightningController == null)
        {
            LightningController = FindAnyObjectByType<LightningController>();
        }

        LightningController.position = position;
        
        LightningController.scale = new Vector3(scale * 10, scale * 10, 1);

        LightningController.emissionRate = emitionLightning;

        SetRainEmition();

        Flash.flashIntensity = flashIntencity;

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

        if (emitionLightning != emitionLightningCheck)
        {
            LightningController.emissionRate = emitionLightning;
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

        scale = wolken;
        
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
        scale = wolken;
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
        var emission = Rain.emission;    // copy of struct
        Sound.SetRainIntencity(emitionRain);

        float rate = 0;

        if (emitionRain == 1) rate = 100;
        if (emitionRain == 2) rate = 500;
        if (emitionRain == 3) rate = 1000;

        emission.rateOverTime = new ParticleSystem.MinMaxCurve(rate);
    }
}
