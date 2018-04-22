using UnityEngine;
using System.Collections;

namespace FSP_Samples {
	
	public class MovementAnimation : MonoBehaviour {
		
		const float _animSpeed = 0.7f;
		
		public float _movementLength = 1.0f;
		
		Vector3 _initialPos;
		float _angle;

		void Start () {	
			_initialPos = transform.position;
		}

		void Update () {
			_angle += Time.deltaTime;

			transform.position = _initialPos + new Vector3(0.0f, 0.0f, Mathf.Sin(_angle) * _movementLength);
		}
	}

}
