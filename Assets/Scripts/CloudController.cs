using UnityEngine;

public class CloudController : MonoBehaviour
{
    public ParticleSystem cloudParticles;

    [Range(0, 3)]
    public int cloudLevel = 0;

    // Fixed cloud color
    public Color cloudColor = new Color(0.9f, 0.9f, 0.9f, 1f);

    private void Awake()
    {
        if (cloudParticles == null)
            cloudParticles = GetComponent<ParticleSystem>();

        if (cloudParticles == null)
        {
            Debug.LogError("CloudController: No ParticleSystem assigned!");
            enabled = false;
            return;
        }

        if (!cloudParticles.isPlaying)
            cloudParticles.Play();
    }

    void LateUpdate()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[cloudParticles.main.maxParticles];
        int count = cloudParticles.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            particles[i].rotation3D = new Vector3(0, particles[i].rotation3D.y, 0);
        }

        cloudParticles.SetParticles(particles, count);
    }

    public void SetCloudDensity(int level)
    {
        if (cloudParticles == null) return;

        cloudLevel = Mathf.Clamp(level, 0, 3);
        float t = cloudLevel / 3f;   

        var emission = cloudParticles.emission;
        var main = cloudParticles.main;
        var velocity = cloudParticles.velocityOverLifetime;

        // Level 0 → NO CLOUDS
        if (cloudLevel == 0)
        {
            emission.rateOverTime = 0f;
            cloudParticles.Clear();
            return;
        }

        // Emission levels
        float minEmission = 0.01f;
        float maxEmission = 2.5f;
        emission.rateOverTime = Mathf.Lerp(minEmission, maxEmission, t);

        // Velocity (wind speed)
        float minVelocity = 10f;
        float maxVelocity = 15f;
        velocity.x = Mathf.Lerp(minVelocity, maxVelocity, t);
        velocity.y = 0f;
        velocity.z = 0f;

        // Lifetime
        float minLifetime = 30f;
        float maxLifetime = 100f;
        main.startLifetime = Mathf.Lerp(minLifetime, maxLifetime, t);

        // Size randomization
        main.startSize = new ParticleSystem.MinMaxCurve(100f, 200f); 

        // Keep color fixed
        main.startColor = cloudColor;

        if (!cloudParticles.isPlaying)
            cloudParticles.Play();

        Debug.Log(
            $"Cloud Level: {cloudLevel} | " +
            $"Emission: {emission.rateOverTime.constant} | " +
            $"VelX: {velocity.x.constant} | " +
            $"Lifetime: {main.startLifetime.constant} | " +
            $"Size: {main.startSize.constantMin}-{main.startSize.constantMax}"
        );
    }
}
