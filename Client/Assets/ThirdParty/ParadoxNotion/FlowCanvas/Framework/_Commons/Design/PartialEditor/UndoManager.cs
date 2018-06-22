#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ParadoxNotion.Design{

	//Simplified version of:
	//http://wiki.unity3d.com/index.php?title=EditorUndoManager

	/// <summary>
	/// A simple Undo manager
	/// </summary>
    public static class UndoManager {
	
		public static void CheckUndo(Object target, string name){

			if (Application.isPlaying || target == null){
				return;
			}

			var e = Event.current;
			if (
				((e.type == EventType.MouseDown || e.type == EventType.MouseUp) && e.button == 0) ||
				(e.type == EventType.KeyDown) ||
				(e.type == EventType.DragPerform) ||
				(e.type == EventType.ExecuteCommand)
				)
			{
				Undo.RecordObject(target, name);
			}
		}

		public static void CheckDirty(Object target){

			if (Application.isPlaying || target == null){
				return;
			}

			if ( GUI.changed ){
				EditorUtility.SetDirty(target);
			}
		}
	}
}

#endif