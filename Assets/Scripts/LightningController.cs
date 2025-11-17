using UnityEngine;

public class LightningController : MonoBehaviour
{
    public ParticleSystem lightningPS;
    public Skylight skyLight;
    public SoundSystem soundSystem;
    public Vector3 position = new Vector3(0, 40, 0);
    public Vector3 scale = new Vector3(10, 10, 1);

    
    public int emissionRate = 100;

    private float timer = 0f;

    private void Start()
    {
        if (soundSystem == null)
            soundSystem = Object.FindAnyObjectByType<SoundSystem>();

        if (lightningPS != null)
        {
            var emission = lightningPS.emission;
            emission.enabled = false; 
            lightningPS.Play();
        }
    }

    private void Update()
    {
        if (lightningPS == null) return;

        timer += Time.deltaTime;

        if (timer >=  emissionRate && emissionRate>0)
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
        lightningPS.Emit(1);

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];
        int count = lightningPS.GetParticles(particles);

        Vector3 spawnPos;
        if (count > 0)
            spawnPos = lightningPS.transform.TransformPoint(particles[count - 1].position);
        else
            spawnPos = lightningPS.transform.position;

        if (skyLight != null)
            skyLight.TriggerFlash(spawnPos);

        if (soundSystem != null)
            soundSystem.PlayThunder(spawnPos);

        Debug.Log("Lightning emitted at: " + spawnPos);
    }
}
