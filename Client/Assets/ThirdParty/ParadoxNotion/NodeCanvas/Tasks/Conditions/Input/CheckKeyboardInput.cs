using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

	[Category("Input")]
	public class CheckKeyboardInput : ConditionTask{

		public PressTypes pressType = PressTypes.Down;
		public KeyCode key = KeyCode.Space;

		protected override string info{
			get {return pressType.ToString() + " " + key.ToString();}
		}

		protected override bool OnCheck(){

			if (pressType == PressTypes.Down)
				return Input.GetKeyDown(key);

			if (pressType == PressTypes.Up)
				return Input.GetKeyUp(key);

			if (pressType == PressTypes.Pressed)
				return Input.GetKey(key);

			return false;
		}
		

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnTaskInspectorGUI(){

			UnityEditor.EditorGUILayout.BeginHorizontal();
			pressType = (PressTypes)UnityEditor.EditorGUILayout.EnumPopup(pressType);
			key = (KeyCode)UnityEditor.EditorGUILayout.EnumPopup(key);
			UnityEditor.EditorGUILayout.EndHorizontal();
		}

		#endif
	}
}