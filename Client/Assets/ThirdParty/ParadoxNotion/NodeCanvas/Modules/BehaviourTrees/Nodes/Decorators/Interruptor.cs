using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Name("Interrupt")]
	[Category("Decorators")]
	[Description("Interrupt the child node and return Failure if the condition is or becomes true while running. Otherwise execute and return the child Status")]
	[Icon("Interruptor")]
	public class Interruptor : BTDecorator, ITaskAssignable<ConditionTask>{

		[SerializeField]
		private ConditionTask _condition;

		public ConditionTask condition{
			get {return _condition;}
			set {_condition = value;}
		}

		public Task task{
			get {return condition;}
			set {condition = (ConditionTask)value;}
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard){

			if (decoratedConnection == null)
				return Status.Resting;

			if (condition == null || condition.CheckCondition(agent, blackboard) == false)
				return decoratedConnection.Execute(agent, blackboard);

			if (decoratedConnection.status == Status.Running)
				decoratedConnection.Reset();
			
			return Status.Failure;
		}
	}
}