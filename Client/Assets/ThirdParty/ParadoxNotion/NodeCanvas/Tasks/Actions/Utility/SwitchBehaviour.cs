using NodeCanvas.Framework;
using ParadoxNotion.Design;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.StateMachines;

namespace NodeCanvas.Tasks.Actions{

	[Category("✫ Utility")]
	[Description("Switch the entire Behaviour Tree of BehaviourTreeOwner")]
	public class SwitchBehaviourTree : ActionTask<BehaviourTreeOwner> {

		[RequiredField]
		public BBParameter<BehaviourTree> behaviourTree;

		protected override string info{
			get {return string.Format("Switch Behaviour {0}", behaviourTree);}
		}


		protected override void OnExecute(){
			agent.SwitchBehaviour(behaviourTree.value);
			EndAction();
		}
	}

	[Category("✫ Utility")]
	[Description("Switch the entire FSM of FSMTreeOwner")]
	public class SwitchFSM : ActionTask<FSMOwner> {
		
		[RequiredField]
		public BBParameter<FSM> fsm;

		protected override string info{
			get {return string.Format("Switch FSM {0}", fsm);}
		}

		protected override void OnExecute(){
			agent.SwitchBehaviour(fsm.value);
			EndAction();
		}
	}
}