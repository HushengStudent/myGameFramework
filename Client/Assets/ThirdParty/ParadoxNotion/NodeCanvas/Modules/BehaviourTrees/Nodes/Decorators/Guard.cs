using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Name("Guard")]
	[Category("Decorators")]
	[Icon("Shield")]
	[Description("Protect the decorated child from running if another Guard with the same token is already guarding (Running) that token.\nGuarding is global for all of the agent's Behaviour Trees.")]
	public class Guard : BTDecorator {

		public enum GuardMode
		{
			ReturnFailure,
			WaitUntilReleased
		}

		public BBParameter<string> token;
		public GuardMode ifGuarded = GuardMode.ReturnFailure;

		private bool isGuarding;

		private static readonly Dictionary<GameObject, List<Guard>> guards = new Dictionary<GameObject, List<Guard>>();
		private static List<Guard> AgentGuards(Component agent){
			return guards[agent.gameObject];
		}

		public override void OnGraphStarted(){
			SetGuards(graphAgent);
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard){

			if (decoratedConnection == null)
				return Status.Failure;

			if (agent != graphAgent)
				SetGuards(agent);

			for (var i = 0; i < AgentGuards(agent).Count; i++){
				var guard = AgentGuards(agent)[i];
				if (guard != this && guard.isGuarding && guard.token.value == this.token.value)
					return ifGuarded == GuardMode.ReturnFailure? Status.Failure : Status.Running;
			}

			status = decoratedConnection.Execute(agent, blackboard);
			if (status == Status.Running){
				isGuarding = true;
				return Status.Running;
			}

			isGuarding = false;
			return status;
		}

		protected override void OnReset(){
			isGuarding = false;
		}

		void SetGuards(Component guardAgent){
			if (!guards.ContainsKey(guardAgent.gameObject))
				guards[guardAgent.gameObject] = new List<Guard>();

			if (!AgentGuards(guardAgent).Contains(this) && !string.IsNullOrEmpty(token.value))
				AgentGuards(guardAgent).Add(this);
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		protected override void OnNodeGUI(){
			GUILayout.Label(string.Format("<b>' {0} '</b>", string.IsNullOrEmpty(token.value)? "NONE" : token.value));
		}
		
		#endif
	}
}