using UnityEngine;
using System.Collections;

namespace FSP_Samples {

	public class ScalingSphere : MonoBehaviour {
		
		ShadowTrigger shadowAreaTrigger;
		
		bool inShadow = false;

		void Start () {
			shadowAreaTrigger = GetComponent<ShadowTrigger>();
			shadowAreaTrigger.OnShadowEnter = OnShadowEnter;
			shadowAreaTrigger.OnShadowStay = OnShadowStay;
			shadowAreaTrigger.OnShadowExit = OnShadowExit;
		}
		
		void Update () {
			if (inShadow) {
				transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.5f, 1.5f, 1.5f), Time.deltaTime * 3.0f);
			} else { 
				transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), Time.deltaTime * 3.0f);
			}
		}
		
		void OnShadowEnter() {
			inShadow = true;
		}
		
		void OnShadowStay() {
			// Might want to do something...
		}	
		
		void OnShadowExit() {
			inShadow = false;
		}
	}

}
