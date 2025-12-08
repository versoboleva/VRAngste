using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
        }
    }
    public void TriggerFlash(Vector3 lightningPos)
    {
        if (cam == null)
        {
            Debug.LogWarning("Skylight: No camera assigned! Flash skipped.");
            return;
        }

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
