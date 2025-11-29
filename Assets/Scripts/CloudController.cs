using UnityEngine;

public class CloudController : MonoBehaviour
{
    public ParticleSystem cloudParticles;

    [Range(0, 4)]
    public int cloudLevel = 0;

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

        // Shape so clouds spawn over an area
        var shape = cloudParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 50f; 
        shape.arc = 360f;

        if (!cloudParticles.isPlaying)
            cloudParticles.Play();
    }

    public void SetCloudDensity(int level)
    {
        if (cloudParticles == null) return;

        cloudLevel = Mathf.Clamp(level, 0, 4);
        float t = cloudLevel / 4f;

        var emission = cloudParticles.emission;
        var main = cloudParticles.main;
        var velocity = cloudParticles.velocityOverLifetime;

        if (cloudLevel == 0)
        {
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
            cloudParticles.Clear();
            return;
        }

        // emission
        float minEmission = 0.01f;
        float maxEmission = 2.5f; 
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(Mathf.Lerp(minEmission, maxEmission, t));

        // Color 
        float shade = Mathf.Lerp(1f, 0.85f, t);
        main.startColor = new Color(shade, shade, shade, 1f);


        // velocity
        float minVelocity = 10f;
        float maxVelocity = 15f; 
        velocity.x = Mathf.Lerp(minVelocity, maxVelocity, t);
        velocity.y = 0f;
        velocity.z = 0f;

        // Lifetime
        float minLifetime = 30f;
        float maxLifetime = 100f;
        main.startLifetime = Mathf.Lerp(minLifetime, maxLifetime, t);

        if (!cloudParticles.isPlaying)
            cloudParticles.Play();

        Debug.Log($"Cloud Level: {cloudLevel}, Emission: {emission.rateOverTime.constant}, VelocityX: {velocity.x.constant}");
    }

    // Context menu for testing
    [ContextMenu("Cloud Level 0")] private void Test0() => SetCloudDensity(0);
    [ContextMenu("Cloud Level 1")] private void Test1() => SetCloudDensity(1);
    [ContextMenu("Cloud Level 2")] private void Test2() => SetCloudDensity(2);
    [ContextMenu("Cloud Level 3")] private void Test3() => SetCloudDensity(3);
    [ContextMenu("Cloud Level 4")] private void Test4() => SetCloudDensity(4);
}
