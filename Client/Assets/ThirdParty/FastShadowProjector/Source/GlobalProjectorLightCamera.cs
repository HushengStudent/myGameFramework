using UnityEngine;
using System;
using System.Collections;

public class GlobalProjectorLightCamera : MonoBehaviour {
	
	public Action PreCullCallback;
	public Action PostRenderCallback;

	void OnPreCull() {
		if (PreCullCallback != null) {
			PreCullCallback();
		}
	}

	void OnPostRender() {
		if (PostRenderCallback != null) {
			PostRenderCallback();
		}
	}
}
