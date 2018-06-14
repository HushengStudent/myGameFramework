using UnityEditor;
using UnityEngine;

namespace Pathfinding {
	[CustomEditor(typeof(Pathfinding.RVO.RVOSimulator))]
	public class RVOSimulatorEditor : UnityEditor.Editor {
		SerializedProperty desiredSimulationFPS, workerThreads, doubleBuffering, symmetryBreakingBias, movementPlane;

		void OnEnable () {
			desiredSimulationFPS = serializedObject.FindProperty("desiredSimulationFPS");
			workerThreads = serializedObject.FindProperty("workerThreads");
			doubleBuffering = serializedObject.FindProperty("doubleBuffering");
			symmetryBreakingBias = serializedObject.FindProperty("symmetryBreakingBias");
			movementPlane = serializedObject.FindProperty("movementPlane");
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();

			EditorGUI.indentLevel = 0;
			EditorGUILayout.PropertyField(desiredSimulationFPS);
			if (desiredSimulationFPS.intValue < 1 && !desiredSimulationFPS.hasMultipleDifferentValues) {
				desiredSimulationFPS.intValue = 1;
			}

			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			EditorGUILayout.PropertyField(movementPlane);
			EditorGUILayout.PropertyField(workerThreads);
			if ((ThreadCount)workerThreads.intValue != ThreadCount.None) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(doubleBuffering);
				EditorGUI.indentLevel--;
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.PropertyField(symmetryBreakingBias);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
