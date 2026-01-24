using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.IO;
using System.Net.Http;

public class LightningController : MonoBehaviour
{
    public ParticleSystem lightningPS;
    public Skylight skyLight;
    public SoundSystem soundSystem;
    public StormSystem stormSystem;
    public Vector3 position;
    public Vector3 scale = new Vector3(50, 50, 1);
    private bool isSceneReady = false;

    
    public TimeSpan lightningInterval = TimeSpan.FromSeconds(100);
    public DateTime nextLightningTime { get; private set; }

    private float timer = 0f;

    private void Start()
    {
        lightningPS ??= GameObject.FindGameObjectWithTag("Lightning")?.GetComponent<ParticleSystem>();
        skyLight ??= FindAnyObjectByType<Skylight>();
        soundSystem ??= FindAnyObjectByType<SoundSystem>();
        stormSystem ??= FindAnyObjectByType<StormSystem>();

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

        lightningPS.transform.position = stormSystem.position;

        var shape = lightningPS.shape;
        shape.scale = scale;
    }
    private void ScheduleNextLightning()
    {
        // Schedule the next lightning locally
        nextLightningTime = DateTime.UtcNow + lightningInterval;

        // Convert nextLightningTime to Unix milliseconds
        ulong nextLightningTimestampMs = (ulong)new DateTimeOffset(nextLightningTime).ToUnixTimeMilliseconds();

        Debug.Log($"Next lightning timestamp in ms: {nextLightningTimestampMs}");

        // Prepare the report
        var report = new AnnounceLightningReport
        {
            Distance = 10UL, // example distance
            Timestamp = nextLightningTimestampMs
        };
        //Master master = Master.Instance;
        // Serialize and send
        //master.SendLightningReport(report);
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
        stormSystem = null;

        lightningPS = GameObject.FindGameObjectWithTag("Lightning")?.GetComponent<ParticleSystem>();
        skyLight = FindAnyObjectByType<Skylight>();
        soundSystem = FindAnyObjectByType<SoundSystem>();
        stormSystem ??= FindAnyObjectByType<StormSystem>();

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
