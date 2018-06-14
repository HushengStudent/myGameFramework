using UnityEngine;
using UnityEditor;

namespace Pathfinding {
	[CustomEditor(typeof(Seeker))]
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(Seeker))]
	public class SeekerEditor : Editor {
		static bool tagPenaltiesOpen;

		public override void OnInspectorGUI () {
			DrawDefaultInspector();

			var script = target as Seeker;

			Undo.RecordObject(script, "modify settings on Seeker");

			// Show a dropdown selector for the tags that this seeker can traverse
			// A callback is necessary because Unity's GenericMenu uses callbacks
			EditorGUILayoutx.TagMaskField(new GUIContent("Valid Tags"), script.traversableTags, result => script.traversableTags = result);

	#if !ASTAR_NoTagPenalty
			EditorGUI.indentLevel = 0;
			tagPenaltiesOpen = EditorGUILayout.Foldout(tagPenaltiesOpen, new GUIContent("Tag Penalties", "Penalties for each tag"));
			if (tagPenaltiesOpen) {
				EditorGUI.indentLevel = 2;
				string[] tagNames = AstarPath.FindTagNames();
				for (int i = 0; i < script.tagPenalties.Length; i++) {
					int tmp = EditorGUILayout.IntField((i < tagNames.Length ? tagNames[i] : "Tag "+i), (int)script.tagPenalties[i]);
					if (tmp < 0) tmp = 0;

					// If the new value is different than the old one
					// Update the value and mark the script as dirty
					if (script.tagPenalties[i] != tmp) {
						script.tagPenalties[i] = tmp;
						EditorUtility.SetDirty(target);
					}
				}
				if (GUILayout.Button("Edit Tag Names...")) {
					AstarPathEditor.EditTags();
				}
			}
			EditorGUI.indentLevel = 1;
	#endif

			if (GUI.changed) {
				EditorUtility.SetDirty(target);
			}
		}
	}
}
