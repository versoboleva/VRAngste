//==============================================================================================================================
//
/// FPSMouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the FPSMouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a FPSMouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
///   
//==============================================================================================================================


using UnityEngine;

namespace FPSBasics
{
    public class FPSMouseLook : MonoBehaviour
    {

        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 15F;
        public float sensitivityY = 15F;

        public float minimumX = -360F;
        public float maximumX = 360F;

        public float minimumY = -60F;
        public float maximumY = 60F;

        private float rotationX = 0f;
        private float rotationY = 0f;
        private Transform _myTransform;

        private void Start()
        {
            // Make the rigid body not change rotation
            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().freezeRotation = true;
            }

            _myTransform = transform;
        }

        private void Update()
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                rotationX = _myTransform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
                rotationY = _myTransform.localEulerAngles.x + -Input.GetAxis("Mouse Y") * sensitivityY;

                if (rotationY > 180) { rotationY -= 360; }
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                _myTransform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX = _myTransform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
                _myTransform.localEulerAngles = new Vector3(_myTransform.localEulerAngles.x, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseY)
            {
                rotationY = _myTransform.localEulerAngles.x + -Input.GetAxis("Mouse Y") * sensitivityY;

                if (rotationY > 180) { rotationY -= 360; }
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                _myTransform.localEulerAngles = new Vector3(rotationY, _myTransform.localEulerAngles.y, 0);
            }
        }
    }

}