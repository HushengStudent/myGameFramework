using UnityEngine;
using System.Collections;

namespace FSP_Samples {

	public class CubeGen : MonoBehaviour {
	
		public GameObject _cubePrefab;
		int _cubeCount = 100;
		public float _genWidth = 10.0f;
		float _angle = 0.0f;
		
		public float _groundOffset = 2.0f;
		public float _scale = 0.33f;
		public bool _rotate = true;
	
		GameObject _cubeGameObject;
		
		void Start () {	
			Generate();
		}
	
		void Generate() {
			int cubesGenerated = 0;
			float angle;
			Vector3 origin;
			while (cubesGenerated < _cubeCount) {
				angle = Random.Range(0.0f, 360.0f);
				origin = gameObject.transform.position + Quaternion.AngleAxis(angle, new Vector3(0.0f, 1.0f, 0.0f)) * new Vector3(Random.Range(0.1f, _genWidth), 0.0f, 0.0f) + new Vector3(0.0f, _groundOffset, 0.0f);
	
				if (Physics.Raycast(new Ray(origin, new Vector3(0.0f, -1.0f, 0.0f)))) {
					_cubeGameObject = (GameObject)GameObject.Instantiate(_cubePrefab, origin, Quaternion.identity);
					_cubeGameObject.GetComponent<SpinAnimation>().AnimationSpeed = Random.Range(-100.0f, 100.0f);
					_cubeGameObject.transform.parent = transform;
					_cubeGameObject.transform.localScale = new Vector3(_scale, _scale, _scale);
					cubesGenerated++;
				}
	
			}
		}
	
		void Update() {
			if (_rotate) { 
				_angle += Time.deltaTime * 10.0f;
				transform.rotation = Quaternion.AngleAxis(_angle, new Vector3(0.0f, 1.0f, 0.0f));
			}
		}
	}
}
