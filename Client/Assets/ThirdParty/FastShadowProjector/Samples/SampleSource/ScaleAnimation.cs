using UnityEngine;
using System.Collections;

namespace FSP_Samples {
	
	public class ScaleAnimation : MonoBehaviour {

		bool _scaleUp = true;
		float _curScale;
		const float MaxScale = 0.7f;
		const float AnimationSpeed = 3.0f;

		void Start() {
			_curScale = transform.localScale.x;
		}

		void Update () {
			if (_scaleUp) {
				_curScale = Mathf.SmoothStep(_curScale, MaxScale, Time.deltaTime * AnimationSpeed);
				if (_curScale > MaxScale - 0.05f) {
					_scaleUp = false;
				}
			} else {
				_curScale = Mathf.SmoothStep(_curScale, 0.2f, Time.deltaTime * AnimationSpeed);
				if (_curScale < 0.25f) {
					_scaleUp = true;
				}
			}

			transform.localScale = new Vector3(_curScale, _curScale, _curScale);
		}
	}
}
