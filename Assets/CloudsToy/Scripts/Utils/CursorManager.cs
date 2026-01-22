//
// Show or hide the mouse cursor.
//
using UnityEngine;

namespace JocyfUtils
{
	public class CursorManager : MonoBehaviour
	{
        [SerializeField] private bool ShowCursor = true;

		private void Start()
		{
			UpdateCursor();
		}

        private void Update()
		{
			if (Cursor.visible != ShowCursor) { UpdateCursor(); }
		}


		private void UpdateCursor()
		{
			Cursor.visible = ShowCursor;
			Cursor.lockState = ShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
		}
	}
}