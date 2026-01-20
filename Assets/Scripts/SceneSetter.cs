using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetter : MonoBehaviour
{
    public static SceneSetter Instance;
    public Master master;

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
    }

    
    // Call this function to switch scene by number
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
            default: return "Safespace"; // fallback
        }
    }
}
