using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.DialogueTrees{

	[Icon("Selector")]
	[Name("Multiple Task Condition")]
	[Category("Branch")]
	[Description("Will continue with the first child node which condition returns true. The Dialogue Actor selected will be used for the checks")]
	[Color("b3ff7f")]
	public class MultipleConditionNode : DTNode, ISubTasksContainer{

		[SerializeField]
		private List<ConditionTask> conditions = new List<ConditionTask>();

		public override int maxOutConnections{
			get {return -1;}
		}

		public Task[] GetSubTasks(){
			return conditions.ToArray();
		}

		public override void OnChildConnected(int index){
			conditions.Insert(index, null);
		}

		public override void OnChildDisconnected(int index){
			conditions.RemoveAt(index);
		}

		protected override Status OnExecute(Component agent, IBlackboard bb){

			if (outConnections.Count == 0)
				return Error("There are no connections on the Dialogue Condition Node");

			for (var i = 0; i < outConnections.Count; i++){
				if (conditions[i] == null || conditions[i].CheckCondition(finalActor.transform, graphBlackboard)){
					DLGTree.Continue(i);
					return Status.Success;
				}
			}

			Debug.LogWarning("No condition is true. Dialogue Stops");
			DLGTree.Stop(false);
			return Status.Failure;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		public override void OnConnectionInspectorGUI(int i){
			NodeCanvas.Editor.TaskEditor.TaskFieldMulti<ConditionTask>(conditions[i], DLGTree, (c)=> { conditions[i] = c; });
		}

		public override string GetConnectionInfo(int i){
			return conditions[i] != null? conditions[i].summaryInfo : "TRUE";
		}

		protected override void OnNodeGUI(){
			if (outConnections.Count == 0){
				GUILayout.Label("No Outcomes Connected");
			}
		}

		#endif
	}
}