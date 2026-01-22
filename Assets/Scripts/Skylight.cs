using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Skylight : MonoBehaviour
{
    public float flashIntensity = 1.5f; 
    public float fadeSpeed = 25f;       
    public Camera cam;
    public Light lightningLight;
    public Light cloudLight;

    float originalIntensity;

    void Start()
    {
        originalIntensity = RenderSettings.ambientIntensity;

        if (cam == null)
            cam = Object.FindAnyObjectByType<Camera>();
    }
    private void Awake()
    {
        if (cam == null)
            cam = Object.FindAnyObjectByType<Camera>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (cam == null || cam.gameObject.scene != scene)
        {
            cam = Object.FindAnyObjectByType<Camera>();
            if (cam == null)
                Debug.LogWarning("Skylight: No MainCamera found in scene " + scene.name);
            originalIntensity = RenderSettings.ambientIntensity;
        }
    }
    public void TriggerFlash(Vector3 lightningPos)
    {
        if (cam == null)
        {
            Debug.LogWarning("Skylight: No camera assigned! Flash skipped.");
            return;
        }
        cloudLight.transform.position = lightningPos;

        float distance = Vector3.Distance(cam.transform.position, lightningPos);
        float distanceFactor = 1f / (1f + distance * 0.001f);
        float finalIntensity = originalIntensity + flashIntensity * distanceFactor;
        StartCoroutine(CloudRoutine(flashIntensity));
        StartCoroutine(FlashRoutine(finalIntensity));
    }

    IEnumerator FlashRoutine(float targetIntensity)
    {
        if (lightningLight == null) yield break;

        float original = lightningLight.intensity;
        lightningLight.intensity = targetIntensity;

        yield return new WaitForSeconds(0.15f);

        while (lightningLight.intensity > original + 0.01f)
        {
            lightningLight.intensity = Mathf.Lerp(
                lightningLight.intensity,
                original,
                Time.deltaTime * fadeSpeed
            );
            yield return null;
        }

        lightningLight.intensity = original;
    }

    IEnumerator CloudRoutine(float targetIntensity)
    {
        if (cloudLight == null) yield break;

        cloudLight.intensity = targetIntensity * 10000;

        while (cloudLight.intensity > 0)
        {
            cloudLight.intensity = Mathf.Lerp(
                cloudLight.intensity,
                0,
                Time.deltaTime * fadeSpeed
            );
            yield return null;
        }

        cloudLight.intensity = 0;
    }
}
