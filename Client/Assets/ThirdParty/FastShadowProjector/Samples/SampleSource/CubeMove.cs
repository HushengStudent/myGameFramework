using UnityEngine;
using System.Collections;

namespace FSP_Samples {

	public class CubeMove : MonoBehaviour {

		Vector3 _initialPos;
		Quaternion _initialRot;

		// Use this for initialization
		void Start () {
			_initialPos = gameObject.transform.position;
			_initialRot = gameObject.transform.rotation;
			gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(300.0f, 0.0f, 0.0f), ForceMode.Force);
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void Reset() {
			gameObject.transform.position = _initialPos;
			gameObject.transform.rotation = _initialRot;
			gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(300.0f, 0.0f, 0.0f), ForceMode.Force);
		}
	}
}
