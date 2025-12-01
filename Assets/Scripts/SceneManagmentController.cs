using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagementController : MonoBehaviour
{
    public static SceneManagementController Instance;

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
            case 0: return "0_Safe";
            case 1: return "1_Room";
            case 2: return "2_Porch";
            case 3: return "3_Car";
            default: return "0_Safe"; // fallback
        }
    }
}
