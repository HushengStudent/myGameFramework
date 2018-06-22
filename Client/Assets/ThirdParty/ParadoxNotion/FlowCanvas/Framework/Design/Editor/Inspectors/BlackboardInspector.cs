#if UNITY_EDITOR

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Editor{

	[CustomEditor(typeof(Blackboard))]
	public class BlackboardInspector : UnityEditor.Editor {

		private Blackboard bb{
			get {return (Blackboard)target;}
		}

		public override void OnInspectorGUI(){
			BlackboardEditor.ShowVariables(bb, bb);
			EditorUtils.EndOfInspector();
			if (Application.isPlaying || Event.current.isMouse){
				Repaint();
			}
		}
	}
}

#endif