using UnityEngine;
using UnityEngine.SceneManagement;

namespace JocyfUtils
{
    public class EscapeKeyDetector : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SceneManager.GetActiveScene().buildIndex == 0) { Application.Quit(); }
                else { SceneManager.LoadScene(0); }
            }
        }
    }
}
