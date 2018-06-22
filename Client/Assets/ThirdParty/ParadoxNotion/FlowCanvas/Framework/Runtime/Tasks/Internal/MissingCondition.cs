using System.Linq;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;
using UnityEngine;


namespace NodeCanvas.Framework.Internal{

	/// <summary>
	/// Injected when a ConditionTask is missing. Recovers back when that condition is found.
	/// </summary>
    [DoNotList]
	[Description("Please resolve the MissingTask issue by either replacing the task or importing the missing task type in the project")]
	public class MissingCondition : ConditionTask, IMissingRecoverable {

		[fsProperty]
		public string missingType{get;set;}
		[fsProperty]
		public string recoveryState{get;set;}

		protected override string info{
			get { return string.Format("<color=#ff6457>* {0} *</color>", missingType.Split('.').Last()); }
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
			
		protected override void OnTaskInspectorGUI(){
			GUILayout.Label(missingType);
		}

		#endif
	}
}