using UnityEngine;
using UnityEngine.SceneManagement;

namespace JocyfUtils
{
    public class MenuLoadLevel : MonoBehaviour
    {
        public void LoadLevel(int sceneToLoad) { SceneManager.LoadScene(sceneToLoad); }
    }
}