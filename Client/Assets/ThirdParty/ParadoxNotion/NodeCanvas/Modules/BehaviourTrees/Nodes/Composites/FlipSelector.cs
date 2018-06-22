using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Category("Composites")]
	[Description("Works like a normal Selector, but when a child node returns Success, that child will be moved to the end.\nAs a result, previously Failed children will always be checked first and recently Successful children last")]
	[Icon("FlipSelector")]
	[Color("b3ff7f")]
	public class FlipSelector : BTComposite {

		private int current;

		public override string name{
			get{return base.name.ToUpper();}
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard){
		
			for (var i = current; i < outConnections.Count; i++){

				status = outConnections[i].Execute(agent, blackboard);
				
				if (status == Status.Running){
					current = i;
					return Status.Running;
				}

				if (status == Status.Success){
					SendToBack(i);
					return Status.Success;
				}
			}

			return Status.Failure;
		}

		void SendToBack(int i){
			var c = outConnections[i];
			outConnections.RemoveAt(i);
			outConnections.Add(c);
		}

		protected override void OnReset(){
			current = 0;
		}
	}
}