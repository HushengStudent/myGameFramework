using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Category("Decorators")]
	[Description("Returns Running until the assigned condition becomes true")]
	[Icon("Halt")]
	public class WaitUntil : BTDecorator, ITaskAssignable<ConditionTask> {

		[SerializeField]
		private ConditionTask _condition;
		private bool accessed;

		public Task task{
			get {return condition;}
			set {condition = (ConditionTask)value;}
		}

		private ConditionTask condition{
			get {return _condition;}
			set {_condition = value;}
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard){

			if (decoratedConnection == null)
				return Status.Resting;

			if (condition == null)
				return decoratedConnection.Execute(agent, blackboard);

		    if ( accessed ) return decoratedConnection.Execute(agent, blackboard);
		    
            if (condition.CheckCondition(agent, blackboard))
		        accessed = true;
		    
            return accessed? decoratedConnection.Execute(agent, blackboard) : Status.Running;
		}

		protected override void OnReset(){
			accessed = false;
		}
	}
}