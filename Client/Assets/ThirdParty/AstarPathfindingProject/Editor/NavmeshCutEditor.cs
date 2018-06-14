using UnityEngine;
using UnityEditor;

namespace Pathfinding {
	[CustomEditor(typeof(NavmeshCut))]
	[CanEditMultipleObjects]
	public class NavmeshCutEditor : Editor {
		SerializedProperty type, mesh, rectangleSize, circleRadius, circleResolution, height, meshScale, center, updateDistance, isDual, cutsAddedGeom, updateRotationDistance, useRotationAndScale;

		void OnEnable () {
			type = serializedObject.FindProperty("type");
			mesh = serializedObject.FindProperty("mesh");
			rectangleSize = serializedObject.FindProperty("rectangleSize");
			circleRadius = serializedObject.FindProperty("circleRadius");
			circleResolution = serializedObject.FindProperty("circleResolution");
			height = serializedObject.FindProperty("height");
			meshScale = serializedObject.FindProperty("meshScale");
			center = serializedObject.FindProperty("center");
			updateDistance = serializedObject.FindProperty("updateDistance");
			isDual = serializedObject.FindProperty("isDual");
			cutsAddedGeom = serializedObject.FindProperty("cutsAddedGeom");
			updateRotationDistance = serializedObject.FindProperty("updateRotationDistance");
			useRotationAndScale = serializedObject.FindProperty("useRotationAndScale");
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(type);

			if (!type.hasMultipleDifferentValues) {
				switch ((NavmeshCut.MeshType)type.intValue) {
				case NavmeshCut.MeshType.Circle:
					EditorGUILayout.PropertyField(circleRadius);
					EditorGUILayout.PropertyField(circleResolution);

					if (circleResolution.intValue >= 20) {
						EditorGUILayout.HelpBox("Be careful with large values. It is often better with a relatively low resolution since it generates cleaner navmeshes with fewer nodes.", MessageType.Warning);
					}
					break;
				case NavmeshCut.MeshType.Rectangle:
					EditorGUILayout.PropertyField(rectangleSize);
					break;
				case NavmeshCut.MeshType.CustomMesh:
					EditorGUILayout.PropertyField(mesh);
					EditorGUILayout.PropertyField(meshScale);
					EditorGUILayout.HelpBox("This mesh should be a planar surface. Take a look at the documentation for an example.", MessageType.Info);
					break;
				}
			}

			EditorGUILayout.PropertyField(height);
			if (!height.hasMultipleDifferentValues) {
				height.floatValue = Mathf.Max(height.floatValue, 0);
			}

			EditorGUILayout.PropertyField(center);

			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(updateDistance);
			EditorGUILayout.PropertyField(useRotationAndScale);
			if (useRotationAndScale.boolValue) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(updateRotationDistance);
				if (!updateRotationDistance.hasMultipleDifferentValues) {
					updateRotationDistance.floatValue = Mathf.Clamp(updateRotationDistance.floatValue, 0, 180);
				}
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.PropertyField(isDual);
			EditorGUILayout.PropertyField(cutsAddedGeom);

			serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck()) {
				foreach (NavmeshCut tg in targets) {
					tg.ForceUpdate();
				}
			}

			// Only the TileHandlerHelper uses the OnEnableCallback, so use that as a quick test to see if one exists
			if (Application.isPlaying && !NavmeshClipper.AnyEnableListeners) {
				EditorGUILayout.HelpBox("No active TileHandlerHelper component exists in the scene. NavmeshCut components will not affect any graphs", MessageType.Error);
			}
		}
	}
}
