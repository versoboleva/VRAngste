using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class LightningController : MonoBehaviour
{
    public ParticleSystem lightningPS;
    public Skylight skyLight;
    public SoundSystem soundSystem;
    public Vector3 position = new Vector3(0, 40, 0);
    public Vector3 scale = new Vector3(10, 10, 1);
    private bool isSceneReady = false;

    
    public TimeSpan lightningInterval = TimeSpan.FromSeconds(100);
    public DateTime nextLightningTime { get; private set; }

    private float timer = 0f;

    private void Start()
    {
        lightningPS ??= GameObject.FindGameObjectWithTag("Lightning")?.GetComponent<ParticleSystem>();
        skyLight ??= FindAnyObjectByType<Skylight>();
        soundSystem ??= FindAnyObjectByType<SoundSystem>();

        if (lightningPS != null)
        {
            var emission = lightningPS.emission;
            emission.enabled = false;
            lightningPS.Play();
        }
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
        LightningController[] controllers = LightningController.FindObjectsByType<LightningController>(FindObjectsSortMode.None);

        if (controllers.Length > 1)
        {
            Destroy(gameObject); // destroy duplicate
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SetupSceneObjects());
    }

    private void Update()
    {
        if (!isSceneReady || lightningPS == null || skyLight == null) return;

        timer += Time.deltaTime;

        if (DateTime.UtcNow >= nextLightningTime)
        {
            EmitLightning();
            ScheduleNextLightning();
        }
    }

    private void FixedUpdate()
    {
        if (lightningPS == null) return;

        lightningPS.transform.position = position;

        var shape = lightningPS.shape;
        shape.scale = scale;
    }
    private void ScheduleNextLightning()
    {
        nextLightningTime = DateTime.UtcNow + lightningInterval;
        Debug.Log($"Lightning interval set to {lightningInterval}s. Next strike at {nextLightningTime}");
        //send time
    }
    public void SetLightningInterval(int seconds)
    {
        if (seconds <= 0)
        {
            Debug.LogWarning("Lightning interval must be greater than 0 seconds.");
            return;
        }
        if (lightningInterval != TimeSpan.FromSeconds(seconds))
        {
            lightningInterval = TimeSpan.FromSeconds(seconds);
            ScheduleNextLightning();
            Debug.Log($"Lightning interval set to {seconds}s. Next strike at {nextLightningTime}");
        }
    }
    private void EmitLightning()
    {
        if (lightningPS == null) return;

        lightningPS.Emit(1);

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];
        int count = lightningPS.GetParticles(particles);

        Vector3 spawnPos;
        if (count > 0)
            spawnPos = lightningPS.transform.TransformPoint(particles[count - 1].position);
        else
            spawnPos = lightningPS.transform.position;

        // Only trigger flash if skyLight exists
        if (skyLight != null)
            skyLight.TriggerFlash(spawnPos);
        else
            Debug.LogWarning("SkyLight not assigned yet!");

        if (soundSystem != null)
            soundSystem.PlayThunder(spawnPos);

        Debug.Log("Lightning emitted at: " + spawnPos + "Time:" + DateTime.UtcNow);
    }

    private IEnumerator SetupSceneObjects()
    {
        // Wait one frame to ensure scene objects are initialized then reset
        yield return null;
        lightningPS = null;
        skyLight = null;
        soundSystem = null;

        lightningPS = GameObject.FindGameObjectWithTag("Lightning")?.GetComponent<ParticleSystem>();
        skyLight = FindAnyObjectByType<Skylight>();
        soundSystem = FindAnyObjectByType<SoundSystem>();

        timer = 0f;

        if (lightningPS != null)
        {
            var emission = lightningPS.emission;
            emission.enabled = false;
            lightningPS.Play();
        }

        if (skyLight == null) Debug.LogWarning("SkyLight not found in scene");
        if (soundSystem == null) Debug.LogWarning("SoundSystem not found in scene");
        nextLightningTime = DateTime.UtcNow + lightningInterval;


        isSceneReady = true;
    }
}
