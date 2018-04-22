using UnityEngine;
using System.Collections;

namespace FSP_Samples {
	
	public class Menu : MonoBehaviour {
		
		void Start() {
			Application.targetFrameRate = 60;
		}
		
		void OnGUI() {
			
			float y = Screen.height - 50;
			if (GUI.Button(new Rect(10, y, 100, 40), "General")) {
				Application.LoadLevel("GeneralScene");
			}

			if (GUI.Button(new Rect(130, y, 100, 40), "Many shadows")) {
				Application.LoadLevel("ManyShadowsScene");
			}
			
			if (GUI.Button(new Rect(250, y, 100, 40), "Terrain")) {
				Application.LoadLevel("TerrainScene");
			}
			
			if (GUI.Button(new Rect(370, y, 110, 40), "Shadow triggers")) {
				Application.LoadLevel("ShadowTriggerScene");
			}
		}
	}

}