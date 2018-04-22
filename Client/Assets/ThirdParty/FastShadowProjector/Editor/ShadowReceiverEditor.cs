using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ShadowReceiver))] 
public class ShadowReceiverEditor : Editor {

	public override void OnInspectorGUI() {

		if (!Application.isPlaying) {
			serializedObject.Update ();
			serializedObject.ApplyModifiedProperties();
		}
	}

	public void OnEnable() {
		if (!Application.isPlaying) {
			SetTerrainProperties();
		}
	}

	public void OnDisable() {
		if (!Application.isPlaying) {
			SetTerrainProperties();	
		}
	}

	void SetTerrainProperties() {
		ShadowReceiver shadowReceiver = (ShadowReceiver) target;

		if (shadowReceiver != null && shadowReceiver.IsTerrain()) {

			serializedObject.Update ();
			
			Terrain terrain = shadowReceiver.GetComponent<Terrain>();
			
			if (terrain != null) {
				shadowReceiver = (ShadowReceiver) target;

				TerrainCollider terrainCollider = terrain.GetComponent<TerrainCollider>();
				terrainCollider.enabled = false;
				terrain.castShadows = false;
				terrain.enabled = false;
			}
			serializedObject.ApplyModifiedProperties();

			if (GUI.changed) {
				EditorUtility.SetDirty(terrain);
			}
		}
	}
}
