using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("✫ Script Control/Common")]
	[Description("Send a Unity message to all game objects with a component of the specified type.\nNotice: This is slow and should not be called per-fame.")]
	public class SendMessageToType<T> : ActionTask where T:Component{

		[RequiredField]
		public BBParameter<string> message;
		[BlackboardOnly]
		public BBParameter<object> argument;
		
		protected override string info{
			get {return string.Format("Message {0}({1}) to all {2}s", message, argument, typeof(T).Name );}
		}

		protected override void OnExecute(){
			
			var objects = Object.FindObjectsOfType<T>() as T[];
			if (objects.Length == 0){
				EndAction(false);
				return;
			}

			foreach (var o in objects){
				o.gameObject.SendMessage(message.value, argument.value);
			}
			
			EndAction(true);
		}
	}
}