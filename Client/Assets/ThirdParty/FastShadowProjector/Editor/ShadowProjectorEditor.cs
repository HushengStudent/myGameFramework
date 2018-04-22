using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ShadowProjector))] 
public class ShadowProjectorEditor : Editor {

	string[] _ShadowResOptions = new string[] { "Very Low", "Low", "Medium", "High", "Very High" };

	string[] _CullingOptions = new string[] { "None", "Caster bounds", "Projection volume" };

	Rect UVRect;
	
	public override void OnInspectorGUI() {
		serializedObject.Update ();

		ShadowProjector shadowProj = (ShadowProjector) target;

		shadowProj.GlobalProjectionDir = EditorGUILayout.Vector3Field("Global light direction", shadowProj.GlobalProjectionDir, null);
	
		shadowProj.GlobalShadowResolution = EditorGUILayout.Popup("Global shadow resolution", shadowProj.GlobalShadowResolution, _ShadowResOptions); 
	
		shadowProj.GlobalShadowCullingMode = (GlobalProjectorManager.ProjectionCulling)EditorGUILayout.Popup("Global culling mode", (int)shadowProj.GlobalShadowCullingMode, _CullingOptions); 
		

		
		shadowProj.EnableCutOff = EditorGUILayout.BeginToggleGroup("Cutoff shadow by distance?", shadowProj.EnableCutOff);
		shadowProj.GlobalCutOffDistance = EditorGUILayout.Slider("Global cutoff distance", shadowProj.GlobalCutOffDistance, 1.0f, 10000.0f);
		EditorGUILayout.EndToggleGroup();


		shadowProj.GlobalFlipX = EditorGUILayout.Toggle("Global flip shadows X:", shadowProj.GlobalFlipX);
		shadowProj.GlobalFlipY = EditorGUILayout.Toggle("Global flip shadows Y:", shadowProj.GlobalFlipY);

		shadowProj.ShadowSize = EditorGUILayout.FloatField("Shadow size", shadowProj.ShadowSize);
		
		shadowProj.ShadowColor = EditorGUILayout.ColorField("Shadow color", shadowProj.ShadowColor);
		
		shadowProj.ShadowOpacity = EditorGUILayout.Slider("Shadow opacity", shadowProj.ShadowOpacity, 0.0f, 1.0f);
		
		shadowProj.IsLight = EditorGUILayout.Toggle("Is light?", shadowProj.IsLight);

		shadowProj._Material = (Material)EditorGUILayout.ObjectField("Shadow material", (Object)shadowProj._Material, typeof(Material), false, null);
		
		EditorGUILayout.LabelField("Shadow UV Rect");

		UVRect = shadowProj.UVRect;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("X:", GUILayout.MaxWidth(15));
		UVRect.x = EditorGUILayout.FloatField(UVRect.x, GUILayout.ExpandWidth(true));
		EditorGUILayout.LabelField("Y:", GUILayout.MaxWidth(15));
		UVRect.y = EditorGUILayout.FloatField(UVRect.y, GUILayout.ExpandWidth(true));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("W:", GUILayout.MaxWidth(15));
		UVRect.width = EditorGUILayout.FloatField(UVRect.width , GUILayout.ExpandWidth(true));
		EditorGUILayout.LabelField("H:", GUILayout.MaxWidth(15));
		UVRect.height = EditorGUILayout.FloatField(UVRect.height, GUILayout.ExpandWidth(true));
		
		EditorGUILayout.EndHorizontal();
		
		shadowProj.ShadowLocalOffset = EditorGUILayout.Vector3Field("Shadow local offset", shadowProj.ShadowLocalOffset, null);
		
	
		shadowProj.RotationAngleOffset = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation angle offsets", shadowProj.RotationAngleOffset.eulerAngles));
		
		EditorGUILayout.LabelField("Freeze rotation:");
		shadowProj.FreezeXRot = EditorGUILayout.Toggle("  X", shadowProj.FreezeXRot);
		shadowProj.FreezeYRot = EditorGUILayout.Toggle("  Y", shadowProj.FreezeYRot);
		shadowProj.FreezeZRot = EditorGUILayout.Toggle("  Z", shadowProj.FreezeZRot);
		
		if (GUILayout.Button("Open UV Editor")) {
			ShadowTextureUVEditor.Open(shadowProj);
		}
		
		shadowProj.AutoSizeOpacity = EditorGUILayout.BeginToggleGroup("Auto opacity/size:", shadowProj.AutoSizeOpacity);
		shadowProj.AutoSORaycastLayer = EditorGUILayout.LayerField("Raycast layer", shadowProj.AutoSORaycastLayer);
		shadowProj.AutoSORayOriginOffset = EditorGUILayout.FloatField("Ray origin offset", shadowProj.AutoSORayOriginOffset);
		shadowProj.AutoSOCutOffDistance = EditorGUILayout.FloatField("Cutoff distance", shadowProj.AutoSOCutOffDistance);
		shadowProj.AutoSOMaxScaleMultiplier = EditorGUILayout.FloatField("Max scale multiplier", shadowProj.AutoSOMaxScaleMultiplier);

		EditorGUILayout.EndToggleGroup();

		shadowProj.UVRect = UVRect;

		if (!GlobalProjectorLayerExists()) {
			CheckGlobalProjectorLayer();
		}

		serializedObject.ApplyModifiedProperties();

		ApplyGlobalSettings();
		
		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}

	public void OnEnable() {
		ShadowProjector targetShadowProj = (ShadowProjector)target;
		Object[] shadowProjectors = GameObject.FindObjectsOfType(typeof(ShadowProjector));
		
		foreach (ShadowProjector shadowProj in shadowProjectors) {
			if (shadowProj.GetInstanceID() != targetShadowProj.GetInstanceID()) {
				targetShadowProj.GlobalProjectionDir = shadowProj.GlobalProjectionDir;
				targetShadowProj.GlobalShadowResolution = shadowProj.GlobalShadowResolution;
				targetShadowProj.GlobalShadowCullingMode = shadowProj.GlobalShadowCullingMode;
				targetShadowProj.GlobalCutOffDistance = shadowProj.GlobalCutOffDistance;
				targetShadowProj.GlobalFlipX = shadowProj.GlobalFlipX;
				targetShadowProj.GlobalFlipY = shadowProj.GlobalFlipY;
				EditorUtility.SetDirty(shadowProj);
				break;
			}
		}
	}

	public void OnDisable() {
		ApplyGlobalSettings();
	}

	void ApplyGlobalSettings() {
		ShadowProjector targetShadowProj = (ShadowProjector)target;
		Object[] shadowProjectors = GameObject.FindObjectsOfType(typeof(ShadowProjector));
		
		foreach (ShadowProjector shadowProj in shadowProjectors) {
			if (shadowProj.GetInstanceID() != targetShadowProj.GetInstanceID()) {
				shadowProj.GlobalProjectionDir = targetShadowProj.GlobalProjectionDir;
				shadowProj.GlobalShadowResolution = targetShadowProj.GlobalShadowResolution;
				shadowProj.GlobalShadowCullingMode = targetShadowProj.GlobalShadowCullingMode;
				shadowProj.GlobalCutOffDistance = targetShadowProj.GlobalCutOffDistance;
				shadowProj.GlobalFlipX = targetShadowProj.GlobalFlipX;
				shadowProj.GlobalFlipY = targetShadowProj.GlobalFlipY;

				EditorUtility.SetDirty(shadowProj);
			}
		}
	}

	public static void CheckGlobalProjectorLayerObsolete() {

		SerializedObject tagManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset")[0]);
		
		if (GlobalProjectorLayerExists()) {
			return;
		}
		
		SerializedProperty it = tagManager.GetIterator ();
		while (it.Next(true)) {
			
			if (it.name.Contains("Layer") && it.name.Contains("User") && it.stringValue == "") {
				it.stringValue = GlobalProjectorManager.GlobalProjectorLayer;
				
				tagManager.ApplyModifiedProperties();
				break;
			}
		}
	}

	public static bool GlobalProjectorLayerExistsObsolete() {
		SerializedObject tagManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset")[0]);
		
		SerializedProperty it = tagManager.GetIterator ();
		while (it.Next(true)) {
			if (it.name.Contains("Layer") && it.name.Contains("User") && it.stringValue.Contains(GlobalProjectorManager.GlobalProjectorLayer)) {
				return true;
			}
		}

		return false;
	}
	
	public static void CheckGlobalProjectorLayer() {
		if (int.Parse(Application.unityVersion[0].ToString ()) > 4) {
			SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty layersProp = tagManager.FindProperty("layers");
	
			for (int i = 8; i < layersProp.arraySize; i++)
			{
				string layerName = layersProp.GetArrayElementAtIndex(i).stringValue;
				if (layerName == GlobalProjectorManager.GlobalProjectorLayer)
				{
					layersProp.GetArrayElementAtIndex(i).stringValue  = "";
				}
			}
	
			for (int i = 8; i < layersProp.arraySize; i++)
			{
				string layerName = layersProp.GetArrayElementAtIndex(i).stringValue;
				if (layerName == "") { 
					layersProp.GetArrayElementAtIndex(i).stringValue  = GlobalProjectorManager.GlobalProjectorLayer;
					break;
				}
			}
	
			tagManager.ApplyModifiedProperties();
		} else {
			CheckGlobalProjectorLayerObsolete();	
		}
	}

	public static bool GlobalProjectorLayerExists() {
		if (int.Parse(Application.unityVersion[0].ToString ()) > 4) {
			SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty layersProp = tagManager.FindProperty("layers");
	
			for (int i = 8; i < layersProp.arraySize; i++)
			{
				string layerName = layersProp.GetArrayElementAtIndex(i).stringValue;
				if (layerName == GlobalProjectorManager.GlobalProjectorLayer)
				{
					return true;
				}
			}
	
			return false;
		} else {
			return GlobalProjectorLayerExistsObsolete ();
		}
	}
}
