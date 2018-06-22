using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Name("Invert")]
	[Category("Decorators")]
	[Description("Inverts Success to Failure and Failure to Success")]
	[Icon("Remap")]
	public class Inverter : BTDecorator {

		protected override Status OnExecute(Component agent, IBlackboard blackboard){

			if (decoratedConnection == null)
				return Status.Resting;

			status = decoratedConnection.Execute(agent, blackboard);
			
			switch(status)
            {
			    case Status.Success:
			        return Status.Failure;
			    case Status.Failure:
			        return Status.Success;
			}

		    return status;
		}
	}
}