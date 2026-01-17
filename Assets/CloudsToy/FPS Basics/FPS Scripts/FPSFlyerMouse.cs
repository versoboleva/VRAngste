#pragma warning disable 0414

using UnityEngine;

namespace FPSBasics
{
	[RequireComponent(typeof(CharacterController))]
	public class FPSFlyerMouse : MonoBehaviour
	{
		public float speed = 6.0f;
		private Vector3 moveDirection = Vector3.zero;
        private float moveup = 0f;

        private CharacterController controller;
        private Transform cameraTransform;
        private CollisionFlags flags;

        private void Start()
		{
			controller = GetComponent<CharacterController>();
            cameraTransform = GetComponentInChildren<Camera>()?.transform;
        }

        private void Update()
        {
			ReadInput();
			CalculateMovementDirection();
        }

        private void FixedUpdate()
		{
			flags = controller.Move(moveDirection * Time.deltaTime);
		}

        private void ReadInput()
        {
            moveup = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E) ? 1f : 0f;
            if (moveup == 0f) { moveup = Input.GetKey(KeyCode.Q) ? -1f : 0f; }

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), moveup, Input.GetAxis("Vertical"));
        }

        private void CalculateMovementDirection()
        {
            moveDirection = cameraTransform.TransformDirection(moveDirection);
            moveDirection *= speed;
        }
    }
}

