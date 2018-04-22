using UnityEngine;
using System.Collections;

namespace FSP_Samples {

	public class SpinAnimation : MonoBehaviour {

		float _angle = 0.0f;
		public float AnimationSpeed;

		void Update () {
			_angle += Time.deltaTime * AnimationSpeed;
			transform.rotation = Quaternion.AngleAxis(_angle, new Vector3(0.0f, 1.0f, 0.0f));
		
		}
	}

}
