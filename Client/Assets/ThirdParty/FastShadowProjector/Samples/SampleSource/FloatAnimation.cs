using UnityEngine;
using System.Collections;

namespace FSP_Samples {

	public class FloatAnimation : MonoBehaviour {

		Vector3 _initialPos;
		float _angle;
		float _angleY;
		
		public float _xFloatDist = 1.0f;
		
		void Start () {	
			_angle = Random.Range (0.0f, 360.0f);
			_angleY = Random.Range (0.0f, 360.0f);
			_initialPos = transform.position;
		}
		
		void Update () {
			_angle += Time.deltaTime * 100.0f;
			_angleY += Time.deltaTime;
			
			transform.position = _initialPos + 
								 Quaternion.AngleAxis(_angle, new Vector3(0.0f, 1.0f, 0.0f)) * new Vector3(_xFloatDist, 0.0f, 0.0f) + 
					Mathf.Sin(_angleY) * new Vector3(0.0f, 1.0f, 0.0f);
								
		}
	}
}
