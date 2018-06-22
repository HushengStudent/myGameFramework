using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Name("Remap")]
	[Category("Decorators")]
	[Description("Remap the child node's status to another status. Used to either invert the child's return status or to always return a specific status.")]
	[Icon("Remap")]
	public class Remapper : BTDecorator{

		public enum RemapStatus
		{
			Failure  = 0,
			Success  = 1,
		}

		public RemapStatus successRemap = RemapStatus.Success;
		public RemapStatus failureRemap = RemapStatus.Failure;

		protected override Status OnExecute(Component agent, IBlackboard blackboard){

			if (decoratedConnection == null)
				return Status.Resting;
			
			status = decoratedConnection.Execute(agent, blackboard);
			
			switch(status)
            {
			    case Status.Success:
			        return (Status)successRemap;
			    case Status.Failure:
			        return (Status)failureRemap;
			}

		    return status;
		}

		/////////////////////////////////////////
		/////////GUI AND EDITOR STUFF////////////
		/////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){

			if ((int)successRemap != (int)Status.Success)
				GUILayout.Label("Success = " + successRemap);

			if ((int)failureRemap != (int)Status.Failure)
				GUILayout.Label("Failure = " + failureRemap);
		}

		#endif
	}
}