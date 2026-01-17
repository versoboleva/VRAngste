using UnityEngine;
using System.Collections;

namespace JocyfUtils
{
	public class TimedObjectDestructorv2 : MonoBehaviour
	{

        [SerializeField] private float timeOut = 1.0f;
        [SerializeField] private bool onlyDisable = false;
        [SerializeField] private bool detachChildren = false;

		private void Awake()
		{
			StartCoroutine("_DestroyNow");
		}

		private  IEnumerator _DestroyNow()
		{
			yield return new WaitForSeconds(timeOut);

			if (detachChildren) { transform.DetachChildren(); }

			if (onlyDisable) { _DeactivateObjectInternal(); }
			else { _DestroyObjectInternal(); }
		}

		private void _DeactivateObjectInternal() 
		{ 
			gameObject.SetActive(false); 
		}

        private void _DestroyObjectInternal() 
		{  
			#if UNITY_5 || UNITY_2017
			DestroyObject(gameObject);
			#else
			Destroy(gameObject);
			#endif
		}
    }
}
