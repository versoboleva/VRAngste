//
// Change the FPC walker type (from standard grounded walker to fly mode)
// Pressing the 'f' key you can change betwen them.
//

using UnityEngine;
using JocyfAttributes;

namespace JocyfUtils
{
    public class ChangeWalker : MonoBehaviour
    {
        [SerializeField] private KeyCode changeKey = KeyCode.Y; // Key to change walker type (default is 'y').
        [Tooltip("Drag here the standard walker (it can walk, run and jump)")]
        [SerializeField] private Transform FPCStandard;
        [SerializeField][Disable] private Transform FPCStandardCamera;
        [Tooltip("Drag here the fly walker (it just fly)")]
        [SerializeField] private Transform FPCFly;
        [SerializeField] private Transform FPCFlyCamera;

        [Header("Runtime variable watcher")]
        [SerializeField]
        private bool flyMode = false; //Are we using FPC fly mode?"

        private void Start()
        {
            GetCameraReferences(); // Get fps walkers cameras.

            flyMode = false;    // Always start using the Standard Walker (you are grounded and can walk/run/jump).
            FPCStandard.position = FPCFly.position; // Both walkers start in the same 3d position in the scene.

            SwitchWalkers();    // Enable the Standard walker and disable the fly walker.
        }

        private void OnEnable()
        {
            if (FPCStandardCamera != null && FPCFlyCamera != null) { return; }
            GetCameraReferences();
        }

        // Check if 'y' is pressed. If so, change betwen both walkers.
        private void Update()
        {
            if (Input.GetKeyDown(changeKey))
            {
                flyMode = !flyMode;
                UpdateWalkersTransforms();
                SwitchWalkers();
            }
        }

        private void GetCameraReferences()
        {
            if (FPCStandardCamera == null)
            {
                FPCStandardCamera = Camera.main?.transform;
            }

            if (FPCStandardCamera == null)
            {
                FPCStandardCamera = FPCStandard.GetComponentInChildren<Camera>().transform;
            }

            if (FPCFlyCamera == null)
            {
                FPCFlyCamera = FPCFly.GetComponentInChildren<Camera>().transform; // Get the camera from the fly walker.
            }
        }

        private void UpdateWalkersTransforms()
        {
            UpdateWalkerPosition();
            UpdateWalkerRotation();
        }

        private void UpdateWalkerPosition()
        {
            if (flyMode)
            {
                FPCFly.position = FPCStandardCamera.position;
            }
            else
            {
                FPCStandard.position = FPCFlyCamera.position;
            }
        }

        private void UpdateWalkerRotation()
        {
            if (flyMode)
            {
                FPCFlyCamera.localEulerAngles = new Vector3(FPCStandardCamera.localEulerAngles.x, FPCStandard.localEulerAngles.y, 0f);
            }
            else
            {
                FPCStandard.localEulerAngles = new Vector3(0f, FPCFlyCamera.localEulerAngles.y, 0f);
                FPCStandardCamera.localEulerAngles = new Vector3(FPCFlyCamera.localEulerAngles.x, 0f, 0f);
            }
        }

        // Switch (enable/disable) walkers
        private void SwitchWalkers()
        {
            FPCStandard.gameObject.SetActive(!flyMode);
            FPCFly.gameObject.SetActive(flyMode);
        }
    }

}