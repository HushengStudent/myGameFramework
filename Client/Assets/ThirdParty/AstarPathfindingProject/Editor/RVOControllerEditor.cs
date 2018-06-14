using UnityEngine;
using UnityEditor;

namespace Pathfinding.RVO {
	[CustomEditor(typeof(RVOController))]
	[CanEditMultipleObjects]
	public class RVOControllerEditor : Editor {
		SerializedProperty center, height;

		void OnEnable () {
			center = serializedObject.FindProperty("center");
			height = serializedObject.FindProperty("height");
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();
			DrawDefaultInspector();

			if ((target as RVOController).movementPlane == MovementPlane.XZ) {
				EditorGUILayout.PropertyField(height);
				EditorGUILayout.PropertyField(center);
			}

			bool maxNeighboursLimit = false;
			bool debugAndMultithreading = false;

			for (int i = 0; i < targets.Length; i++) {
				var controller = targets[i] as RVOController;
				if (controller.rvoAgent != null) {
					if (controller.rvoAgent.NeighbourCount >= controller.rvoAgent.MaxNeighbours) {
						maxNeighboursLimit = true;
					}
				}

				if (controller.simulator != null && controller.simulator.Multithreading && controller.debug) {
					debugAndMultithreading = true;
				}
			}

			if (maxNeighboursLimit) {
				EditorGUILayout.HelpBox("Limit of how many neighbours to consider (Max Neighbours) has been reached. Some nearby agents may have been ignored. " +
					"To ensure all agents are taken into account you can raise the 'Max Neighbours' value at a cost to performance.", MessageType.Warning);
			}

			if (debugAndMultithreading) {
				EditorGUILayout.HelpBox("Debug mode can only be used when no multithreading is used. Set the 'Worker Threads' field on the RVOSimulator to 'None'", MessageType.Error);
			}

			if (RVOSimulator.active == null && !EditorUtility.IsPersistent(target)) {
				EditorGUILayout.HelpBox("There is no enabled RVOSimulator component in the scene. A single RVOSimulator component is required for local avoidance.", MessageType.Warning);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
