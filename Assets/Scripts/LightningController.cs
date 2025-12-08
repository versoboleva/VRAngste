using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LightningController : MonoBehaviour
{
    public ParticleSystem lightningPS;
    public Skylight skyLight;
    public SoundSystem soundSystem;
    public Vector3 position = new Vector3(0, 40, 0);
    public Vector3 scale = new Vector3(10, 10, 1);
    private bool isSceneReady = false;

    
    public int emissionRate = 100;

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
        LightningController[] controllers = Object.FindObjectsByType<LightningController>(FindObjectsSortMode.None);

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

        if (timer >= emissionRate && emissionRate > 0)
        {
            EmitLightning();
            timer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (lightningPS == null) return;

        lightningPS.transform.position = position;

        var shape = lightningPS.shape;
        shape.scale = scale;
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

        Debug.Log("Lightning emitted at: " + spawnPos);
    }

    private IEnumerator SetupSceneObjects()
    {
        // Wait one frame to ensure scene objects are initialized
        yield return null;

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

        isSceneReady = true;
    }
}
