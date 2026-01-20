using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class SceneSetter : MonoBehaviour
{
    public static SceneSetter Instance;
    public Master master;

    private InputDevice rightHand;
    private bool lastTriggerPressed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
        }
        else
        {
            Destroy(gameObject);
        }
        if (master == null)
        {
            master = FindAnyObjectByType<Master>();
        }
        rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
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
        ChangeScene(0);
    }

    lastTriggerPressed = triggerPressed;
}
    
    public void ChangeScene(int sceneNumber)
    {
        string sceneName = GetSceneName(sceneNumber);
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("Scene changed to: " + sceneName);
        }
        else
        {
            Debug.LogWarning("Scene number " + sceneNumber + " not found. Make sure it's in Build Settings!");
        }
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
