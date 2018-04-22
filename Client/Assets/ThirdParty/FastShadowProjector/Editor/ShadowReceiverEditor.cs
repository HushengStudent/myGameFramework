using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ShadowReceiver))] 
public class ShadowReceiverEditor : Editor {

	Material _terrainMaterial = null;
	
	public override void OnInspectorGUI() {

		if (!Application.isPlaying) {
			serializedObject.Update ();
			serializedObject.ApplyModifiedProperties();
		}
	}

	public void OnEnable() {
		if (!Application.isPlaying) {
			SetTerrainMaterial(_terrainMaterial);
		}
	}

	public void OnDisable() {
		if (!Application.isPlaying) {
			SetTerrainMaterial(_terrainMaterial);	
		}
	}
	
	void SetTerrainMaterial() {
		SetTerrainMaterial(null);
	}
	
	void SetTerrainMaterial(Material terrainMaterial) {
		ShadowReceiver shadowReceiver = (ShadowReceiver) target;

		if (shadowReceiver != null && shadowReceiver.IsTerrain()) {

			serializedObject.Update ();
			
			Terrain terrain = shadowReceiver.GetComponent<Terrain>();
			
			if (terrainMaterial == null && terrain != null) {
				 shadowReceiver = (ShadowReceiver) target;
#if UNITY_5_0 || UNITY_5_1
				terrain.materialType = Terrain.MaterialType.Custom;
#endif
				terrain.materialTemplate = terrainMaterial;
				
				TerrainCollider terrainCollider = terrain.GetComponent<TerrainCollider>();
				terrainCollider.enabled = false;
				terrain.castShadows = false;
				terrain.enabled = false;
			}
			serializedObject.ApplyModifiedProperties();

		}
	}
}
