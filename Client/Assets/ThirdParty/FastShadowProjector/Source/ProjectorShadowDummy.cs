using UnityEngine;
using System.Collections;

public class ProjectorShadowDummy : MonoBehaviour {

	public Vector3 _ShadowLocalOffset;
	public Quaternion _RotationAngleOffset;

	public bool _freezeXRot;
	public bool _freezeYRot;
	public bool _freezeZRot;
	
	Quaternion _AngleOffset;
	
	public void OnPreRenderShadowDummy(Camera camera) {

		Vector3 offsetEuler = _RotationAngleOffset.eulerAngles;

		transform.rotation = Quaternion.identity;

		if (!_freezeXRot) {
			transform.rotation *= Quaternion.AngleAxis(transform.parent.rotation.eulerAngles.x + offsetEuler.x, camera.transform.right);
		}

		if (!_freezeYRot) {
			transform.rotation *= Quaternion.AngleAxis(transform.parent.rotation.eulerAngles.y + offsetEuler.y, -camera.transform.forward);
		}

		if (!_freezeZRot) {
			transform.rotation *= Quaternion.AngleAxis(transform.parent.rotation.eulerAngles.z + offsetEuler.z, camera.transform.up);
		}

		transform.rotation *= Quaternion.LookRotation(camera.transform.up,  camera.transform.forward);

		transform.localPosition = _ShadowLocalOffset;
	}
}

