using UnityEngine;

public class CloudController : MonoBehaviour
{
    public ParticleSystem cloudParticles;

    [Range(0, 100)]
    public int cloudDensity = 0;

    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.MainModule main;
    private ParticleSystem.VelocityOverLifetimeModule velocity;

    private void Awake()
    {
        emission = cloudParticles.emission;
        main = cloudParticles.main;
        velocity = cloudParticles.velocityOverLifetime;
    }

    public void SetCloudDensity(int level)
    {
        cloudDensity = level;

        //  Adjust emission rate
        emission.rateOverTime = Mathf.Lerp(0f, 200f, level / 100f);

        // Adjust color (darker for stormy clouds)
        var color = main.startColor;
        float darkness = Mathf.Lerp(1f, 0.4f, level / 100f);
        color.color = new Color(darkness, darkness, darkness, 1f);
        main.startColor = color;

        // Adjust velocity (faster for denser clouds)
        velocity.x = Mathf.Lerp(0.5f, 3f, level / 100f);

        // Adjust lifetime (particles stay longer when dense)
         main.startLifetime = Mathf.Lerp(5f, 30f, level / 100f);
    }
}
