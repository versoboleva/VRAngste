using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using System.Collections;

public class SceneSetter : MonoBehaviour
{
    public static SceneSetter Instance;
    public Master master;
    public SoundSystem sound;

    private InputDevice rightHand;
    private bool lastTriggerPressed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (master == null)
        {
            master = FindAnyObjectByType<Master>();
        }
        
        if(sound == null)
        {
            sound = FindAnyObjectByType<SoundSystem>();
        }
        rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset sound reference after scene change
        if (sound == null)
            sound = FindAnyObjectByType<SoundSystem>();

        if (sound == null)
            Debug.LogWarning("SoundSystem not found in scene: " + scene.name);
    }

    void Update()
    {
        if (!rightHand.isValid)
            rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool triggerPressed = false;

        if (rightHand.isValid)
            rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed);

        if (triggerPressed && !lastTriggerPressed)
        {
            Debug.Log("Right Trigger pressed → loading scene 1");
            if (SceneManager.GetActiveScene().name != "Safespace")
            {
                Debug.Log("Right Trigger pressed → loading Safespace (0)");
                ChangeScene(0);
            }
        }

        lastTriggerPressed = triggerPressed;
    }
    
    public void ChangeScene(int sceneNumber)
    {
        if (Instance != null)
        {
            StartCoroutine(Instance.ChangeSceneCoroutine(sceneNumber));
        }
    }


    private IEnumerator ChangeSceneCoroutine(int sceneNumber)
    {
        if (sound != null)
        {
            sound.StopAudio();
        }
        
        yield return new WaitForSeconds(1);

        string sceneName = GetSceneName(sceneNumber);

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning("Scene number " + sceneNumber + " not found. Make sure it's in Build Settings!");
            yield break; // Stop coroutine
        }
        Debug.Log("Changing scene to: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }


    private string GetSceneName(int sceneNumber)
    {
        switch (sceneNumber)
        {
            case 0: return "Safespace";
            case 1: return "Innen";
            case 2: return "Ausen";
            case 3: return "Car";
            default: return "Safespace";
        }
    }
}
