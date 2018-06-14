using UnityEditor;

namespace Pathfinding {
	[CustomEditor(typeof(RaycastModifier))]
	[CanEditMultipleObjects]
	public class RaycastModifierEditor : Editor {
		SerializedProperty iterations, useRaycasting, thickRaycast, thickRaycastRadius, raycastOffset, useGraphRaycasting, subdivideEveryIter, mask;

		void OnEnable () {
			iterations = serializedObject.FindProperty("iterations");
			useRaycasting = serializedObject.FindProperty("useRaycasting");
			thickRaycast = serializedObject.FindProperty("thickRaycast");
			thickRaycastRadius = serializedObject.FindProperty("thickRaycastRadius");
			raycastOffset = serializedObject.FindProperty("raycastOffset");
			useGraphRaycasting = serializedObject.FindProperty("useGraphRaycasting");
			subdivideEveryIter = serializedObject.FindProperty("subdivideEveryIter");
			mask = serializedObject.FindProperty("mask");
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();

			EditorGUI.indentLevel = 0;

			EditorGUILayout.PropertyField(iterations);
			if (iterations.intValue < 0 && !iterations.hasMultipleDifferentValues) iterations.intValue = 0;

			EditorGUILayout.PropertyField(useRaycasting);

			if (useRaycasting.boolValue) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(thickRaycast);

				if (thickRaycast.boolValue) {
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(thickRaycastRadius);
					if (thickRaycastRadius.floatValue < 0 && !thickRaycastRadius.hasMultipleDifferentValues) thickRaycastRadius.floatValue = 0;
					EditorGUI.indentLevel--;
				}

				EditorGUILayout.PropertyField(raycastOffset);
				EditorGUILayout.PropertyField(mask);
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.PropertyField(useGraphRaycasting);
			EditorGUILayout.PropertyField(subdivideEveryIter);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
