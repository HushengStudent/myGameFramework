using UnityEditor;
using UnityEngine;

namespace Pathfinding {
	[CustomEditor(typeof(AIBase), true)]
	[CanEditMultipleObjects]
	public class BaseAIEditor : Editor {
		protected SerializedProperty gravity, groundMask, centerOffset, rotationIn2D;
		float lastSeenCustomGravity = float.NegativeInfinity;

		void OnEnable () {
			gravity = serializedObject.FindProperty("gravity");
			groundMask = serializedObject.FindProperty("groundMask");
			centerOffset = serializedObject.FindProperty("centerOffset");
			rotationIn2D = serializedObject.FindProperty("rotationIn2D");
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();

			// Iterate over all properties of the script
			var p = serializedObject.GetIterator();
			p.Next(true);
			while (p.NextVisible(false)) {
				if (!SerializedProperty.EqualContents(p, groundMask) && !SerializedProperty.EqualContents(p, centerOffset) && !SerializedProperty.EqualContents(p, gravity) && !SerializedProperty.EqualContents(p, rotationIn2D)) {
					EditorGUILayout.PropertyField(p, true);
				}
			}

			EditorGUILayout.PropertyField(rotationIn2D);

			var mono = target as MonoBehaviour;
			var rigid = mono.GetComponent<Rigidbody>();
			var rigid2D = mono.GetComponent<Rigidbody2D>();
			var controller = mono.GetComponent<CharacterController>();
			var canUseGravity = (controller != null && controller.enabled) || ((rigid == null || rigid.isKinematic) && (rigid2D == null || rigid2D.isKinematic));

			if (canUseGravity) {
				EditorGUI.BeginChangeCheck();
				int grav = gravity.hasMultipleDifferentValues ? -1 : (gravity.vector3Value == Vector3.zero ? 0 : (float.IsNaN(gravity.vector3Value.x) ? 1 : 2));
				var ngrav = EditorGUILayout.Popup("Gravity", grav, new [] { "None", "Use Project Settings", "Custom" });
				if (EditorGUI.EndChangeCheck()) {
					if (ngrav == 0) gravity.vector3Value = Vector3.zero;
					else if (ngrav == 1) gravity.vector3Value = new Vector3(float.NaN, float.NaN, float.NaN);
					else if (float.IsNaN(gravity.vector3Value.x) || gravity.vector3Value == Vector3.zero) gravity.vector3Value = Physics.gravity;
					lastSeenCustomGravity = float.NegativeInfinity;
				}

				if (!gravity.hasMultipleDifferentValues) {
					// A sort of delayed Vector3 field (to prevent the field from dissappearing if you happen to enter zeroes into x, y and z for a short time)
					// Note: cannot use != in this case because that will not give the correct result in case of NaNs
					if (!(gravity.vector3Value == Vector3.zero)) lastSeenCustomGravity = Time.realtimeSinceStartup;
					if (Time.realtimeSinceStartup - lastSeenCustomGravity < 2f) {
						EditorGUI.indentLevel++;
						if (!float.IsNaN(gravity.vector3Value.x)) {
							EditorGUILayout.PropertyField(gravity, true);
						}

						if (controller == null || !controller.enabled) {
							EditorGUILayout.PropertyField(groundMask, new GUIContent("Raycast Ground Mask"));
							EditorGUILayout.PropertyField(centerOffset, new GUIContent("Raycast Center Offset"));
						}

						EditorGUI.indentLevel--;
					}
				}
			} else {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.Popup(new GUIContent(gravity.displayName, "Disabled because a non-kinematic rigidbody is attached"), 0, new [] { new GUIContent("Handled by Rigidbody") });
				EditorGUI.EndDisabledGroup();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
