using UnityEngine;
using System.Collections;

namespace FSP_Samples {
		
	public class RollingCubes : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		void OnGUI() {

			if (GUI.Button(new Rect(10, 10, 100, 30), "Roll !")) {
				CubeMove[] cubes = (CubeMove[])GameObject.FindObjectsOfType(typeof(CubeMove));

				for (int n = 0; n <cubes.Length; n++) {
					cubes[n].Reset();
				}
			}
		}
	}

}