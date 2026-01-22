using UnityEngine;

namespace JocyfUtils
{
	public class CopyTransformv2 : MonoBehaviour
	{
		[SerializeField] private Transform from;
		private Transform to;
		public Vector3 offset = Vector3.zero;

		private Vector3 Pos;
        //private Quaternion Rot;


        private void Start()
		{
			if (from == null) { from = GameObject.FindGameObjectWithTag("Player").transform; }
			to = transform;
		}

        private void LateUpdate()
		{
			if (from == null) { from = GameObject.FindGameObjectWithTag("Player").transform; }
			if (from == null) { return; }

			Pos = from.position + offset;
			if (Pos != to.position) { to.position = Pos; }
		}
	}
}
