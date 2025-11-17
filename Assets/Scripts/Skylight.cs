using UnityEngine;
using System.Collections;

public class Skylight : MonoBehaviour
{
    public float flashIntensity = 1.5f; 
    public float fadeSpeed = 25f;       
    public Camera cam;

    float originalIntensity;

    void Start()
    {
        originalIntensity = RenderSettings.ambientIntensity;

        if (cam == null)
            cam = Camera.main;
    }

    public void TriggerFlash(Vector3 lightningPos)
    {
        float distance = Vector3.Distance(cam.transform.position, lightningPos);

        float distanceFactor = 1f / (1f + distance * 0.01f);

        float finalIntensity = originalIntensity + flashIntensity * distanceFactor;

        StartCoroutine(FlashRoutine(finalIntensity));
    }

    IEnumerator FlashRoutine(float targetIntensity)
    {
        RenderSettings.ambientIntensity = targetIntensity;

        yield return new WaitForSeconds(0.15f); 

        while (RenderSettings.ambientIntensity > originalIntensity + 0.01f)
        {
            RenderSettings.ambientIntensity = Mathf.Lerp(
                RenderSettings.ambientIntensity,
                originalIntensity,
                Time.deltaTime * fadeSpeed
            );
            yield return null;
        }

        RenderSettings.ambientIntensity = originalIntensity;
    }
}
