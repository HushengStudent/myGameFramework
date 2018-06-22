using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("✫ Blackboard")]
	public class SetEnum : ActionTask {

		[BlackboardOnly] [RequiredField]
		public BBObjectParameter valueA = new BBObjectParameter(typeof(System.Enum));
		public BBObjectParameter valueB = new BBObjectParameter(typeof(System.Enum));

		protected override string info{
			get {return valueA + " = " + valueB;}
		}

		protected override void OnExecute(){
			valueA.value = valueB.value;
			EndAction();
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		protected override void OnTaskInspectorGUI(){
			DrawDefaultInspector();
			if (GUI.changed && valueB.varType != valueA.refType){
				valueB.SetType( valueA.refType );
			}
		}
		
		#endif
	}
}