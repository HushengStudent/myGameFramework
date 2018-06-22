#if UNITY_EDITOR

using System.Linq;
using NodeCanvas.DialogueTrees;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Editor{

	[CustomEditor(typeof(DialogueTree))]
	public class DialogueTreeInspector : GraphInspector{

		private DialogueTree dialogue{
			get {return target as DialogueTree;}
		}

		public override void OnInspectorGUI(){

			base.OnInspectorGUI();

			ShowActorParameters(dialogue);

			EditorUtils.EndOfInspector();

			if (GUI.changed){
				EditorUtility.SetDirty(dialogue);
			}
		}

		//static because it's also used from DialogueTreeController
		public static void ShowActorParameters(DialogueTree dialogue){
            EditorUtils.TitledSeparator("Dialogue Actor Parameters");
			EditorGUILayout.HelpBox("Enter the Key-Value pair for Dialogue Actors involved in the Dialogue.\nReferencing a DialogueActor is optional.", MessageType.Info);

			GUILayout.BeginVertical("box");

			if (GUILayout.Button("Add Actor Parameter")){
				dialogue.actorParameters.Add(new DialogueTree.ActorParameter("actor parameter name"));
			}
			
			EditorGUILayout.LabelField("INSTIGATOR <--Replaced by the Actor starting the Dialogue");

			for (var i = 0; i < dialogue.actorParameters.Count; i++){
				var reference = dialogue.actorParameters[i];
				GUILayout.BeginHorizontal();
				if (dialogue.actorParameters.Where(r => r != reference).Select(r => r.name).Contains(reference.name)){
					GUI.backgroundColor = Color.red;
				}
				reference.name = EditorGUILayout.TextField(reference.name);
				GUI.backgroundColor = Color.white;
				reference.actor = (IDialogueActor)EditorGUILayout.ObjectField(reference.actor as Object, typeof(DialogueActor), true);
				if (GUILayout.Button("X", GUILayout.Width(18))){
					dialogue.actorParameters.Remove(reference);
				}
				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();			
		}
	}
}

#endif