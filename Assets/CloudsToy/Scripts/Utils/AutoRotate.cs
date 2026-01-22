using UnityEngine;

namespace JocyfUtils
{
	public class AutoRotate : MonoBehaviour
	{
		public float speed = 50;

		private void Update()
		{
			transform.Rotate(Vector3.up, Time.deltaTime * speed, Space.World);
		}
	}
}