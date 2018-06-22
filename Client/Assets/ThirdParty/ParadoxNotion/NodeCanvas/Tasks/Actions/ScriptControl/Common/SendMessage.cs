using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("✫ Script Control/Common")]
	[Description("SendMessage to the agent, optionaly with an argument")]
	public class SendMessage : ActionTask<Transform> {

		[RequiredField]
		public BBParameter<string> methodName;

		protected override string info{
			get {return string.Format("Message {0}()", methodName);}
		}

		protected override void OnExecute(){
			agent.SendMessage(methodName.value);
			EndAction();
		}
	}


	[Category("✫ Script Control/Common")]
	[Description("SendMessage to the agent, optionaly with an argument")]
	public class SendMessage<T> : ActionTask<Transform> {

		[RequiredField]
		public BBParameter<string> methodName;
		public BBParameter<T> argument;

		protected override string info{
			get {return string.Format("Message {0}({1})", methodName, argument.ToString() );}
		}

		protected override void OnExecute(){
			if (argument.isNull){
				agent.SendMessage(methodName.value);
			} else {
				agent.SendMessage(methodName.value, argument.value);
			}

			EndAction();
		}
	}
}