using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[DoNotList]
	[Category("Mutators (beta)")]
	[Name("Node Toggler")]
	[Description("Enable, Disable or Toggle one or more nodes with provided tag. In practise their incomming connections are disabled\nBeta Feature!")]
	public class NodeToggler : BTNode {

		public enum ToggleMode
		{
			Enable,
			Disable,
			Toggle
		}

		public ToggleMode toggleMode = ToggleMode.Toggle;
		public string targetNodeTag;
		
		private List<Node> targetNodes;

		public override void OnGraphStarted(){
			targetNodes = graph.GetNodesWithTag<Node>(targetNodeTag);
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard){

			if (string.IsNullOrEmpty(targetNodeTag))
				return Status.Failure;

		    if ( targetNodes.Count == 0 ) return Status.Failure;

		    if (toggleMode == ToggleMode.Enable){
		        foreach (var node in targetNodes)
		            node.inConnections[0].isActive = true;
		    }

		    if (toggleMode == ToggleMode.Disable){
		        foreach (var node in targetNodes)
		            node.inConnections[0].isActive = false;
		    }

		    if (toggleMode == ToggleMode.Toggle){
		        foreach (var node in targetNodes)
		            node.inConnections[0].isActive = !node.inConnections[0].isActive;
		    }

		    return Status.Success;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		protected override void OnNodeGUI(){
			GUILayout.Label(string.Format("{0} '{1}'", toggleMode.ToString(), targetNodeTag));
		}

		protected override void OnNodeInspectorGUI(){
			targetNodeTag = EditorUtils.StringPopup("Node Tag", targetNodeTag, graph.GetAllTagedNodes<Node>().Select(n => n.tag).ToList());
			toggleMode = (ToggleMode)UnityEditor.EditorGUILayout.EnumPopup("Toggle Mode", toggleMode);
		}
		
		#endif
	}
}