using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetter : MonoBehaviour
{
    public static SceneSetter Instance;

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
    }

    
    // Call this function to switch scene by number
    public void ChangeScene(int sceneNumber)
    {
        string sceneName = GetSceneName(sceneNumber);
        Debug.Log("Scenesettings called");
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
            case 1: return "Inside";
            case 2: return "Outside";
            case 3: return "Car";
            default: return "Safespace"; // fallback
        }
    }
}
