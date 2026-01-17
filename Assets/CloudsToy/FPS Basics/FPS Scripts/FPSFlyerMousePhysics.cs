using UnityEngine;

namespace FPSBasics
{
	[RequireComponent(typeof(Rigidbody))]
	public class FPSFlyerMousePhysics : MonoBehaviour
	{
		public float fwdForce, sideForce, upwardsForce = 6f;

        private float forwardInput, lateralInput, upwardsInput = 0f;

        private Rigidbody myRigidbody;
        private Transform cameraTransform;

		private void Start()
		{
			myRigidbody = GetComponent<Rigidbody>();
            cameraTransform = GetComponentInChildren<Camera>()?.transform;

            if (myRigidbody != null) { myRigidbody.freezeRotation = true; }
        }

        private void Update() => ReadInput();

        private void FixedUpdate()
		{
			myRigidbody.AddForce(cameraTransform.forward * fwdForce * forwardInput);
			myRigidbody.AddForce(cameraTransform.right * sideForce * lateralInput);
            myRigidbody.AddForce(cameraTransform.up * sideForce * upwardsInput);
        }

        private void ReadInput()
        {
            forwardInput = Input.GetAxis("Vertical");
            lateralInput = Input.GetAxis("Horizontal");
            upwardsInput = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E) ? 1f : 0f;
            if (upwardsInput == 0f) { upwardsInput = Input.GetKey(KeyCode.Q) ? -1f : 0f; }
        }
    }
}