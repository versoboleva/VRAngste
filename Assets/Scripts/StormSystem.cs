using TMPro;
using UnityEngine;
using UnityEngine.Android;
using System.Collections;

public class StormSystem : MonoBehaviour
{
    private LightningController LightningController;
    private ParticleSystem Rain;
    private Skylight Flash;
    private SoundSystem Sound;
    private CloudController clouds;
    
    
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
        if (clouds == null)
        {
            clouds = FindAnyObjectByType<CloudController>();
            
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

        // instead of setting instantly:
        // emission.rateOverTime = new MinMaxCurve(rate);

        StartCoroutine(LerpRainEmission(rate, 10f));  // 1f = duration of blend
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

        // Ensure final exact value
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(targetRate);
    }

    // inside StormSystem class

    [ContextMenu("Test Clouds Level 0")]
private void TestClouds0()
{
    SetWolken(0);
}

[ContextMenu("Test Clouds Level 1")]
private void TestClouds1()
{
    SetWolken(1);
}

[ContextMenu("Test Clouds Level 2")]
private void TestClouds2()
{
    SetWolken(2);
}

[ContextMenu("Test Clouds Level 3")]
private void TestClouds3()
{
    SetWolken(3);
}

[ContextMenu("Test Clouds Level 4")]
private void TestClouds4()
{
    SetWolken(4);
}


}
