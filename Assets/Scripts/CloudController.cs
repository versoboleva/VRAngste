using UnityEngine;
using JocyfCloudsToy;

public class CloudController : MonoBehaviour
{
    private CloudsToy cloudSystem;

    [Range(0, 3)]
    public int cloudLevel = 0;

    private int lastCloudLevel = -1;

    private void Awake()
    {
        cloudSystem = GetComponent<CloudsToy>();

        if (cloudSystem == null)
        {
            Debug.LogError("CloudController: No CloudsToy component found on this GameObject!");
            enabled = false;
            return;
        }
        DisableCloudRoll();
    }
    private void Update()
    {
        if (cloudLevel != lastCloudLevel)
        {
            SetCloudDensity(cloudLevel);
            lastCloudLevel = cloudLevel;
        }
    }

    public void SetCloudDensity(int level)
    {
        if (cloudSystem == null) return;

        cloudLevel = Mathf.Clamp(level, 0, 3);

        int[] widthLevels = { 70, 178, 356, 534 };
        int targetWidth = widthLevels[cloudLevel];

        cloudSystem.MaxWidthCloud = targetWidth;

        Debug.Log($"[CloudController] Level: {cloudLevel} | Width set to: {targetWidth} | CloudsToy.MaxWidthCloud is now: {cloudSystem.MaxWidthCloud}");
    }

    private void DisableCloudRoll()
    {
        ParticleSystemRenderer[] renderers = cloudSystem.GetComponentsInChildren<ParticleSystemRenderer>();

        foreach (var psr in renderers)
        {
            psr.allowRoll = false;
        }

        Debug.Log("[CloudController] Disabled 'Allow Roll' on all cloud particle systems.");
    }
}