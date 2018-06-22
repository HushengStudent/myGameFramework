using System.Collections;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using ParadoxNotion;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Name("Iterate")]
	[Category("Decorators")]
	[Description("Iterate any type of list and execute the child node for each element in the list. Keeps iterating until the Termination Condition is met or the whole list is iterated and return the child node status")]
	[Icon("List")]
	public class Iterator : BTDecorator{

		public enum TerminationConditions
		{
			None,
			FirstSuccess,
			FirstFailure
		}

		[RequiredField] [BlackboardOnly]
		public BBParameter<IList> targetList;
		[BlackboardOnly]
		public BBObjectParameter current;
		[BlackboardOnly]
		public BBParameter<int> storeIndex;

		public BBParameter<int> maxIteration = -1;

		public TerminationConditions terminationCondition = TerminationConditions.None;
		public bool resetIndex = true;

		private int currentIndex;

		private IList list{
			get {return targetList != null? targetList.value : null;}
		}
		
		protected override Status OnExecute(Component agent, IBlackboard blackboard){

			if (decoratedConnection == null){
				return Status.Resting;
			}

			if (list == null || list.Count == 0){
				return Status.Failure;
			}

			for (int i = currentIndex; i < list.Count; i++){

				current.value = list[i];
				storeIndex.value = i;
				status = decoratedConnection.Execute(agent, blackboard);
				
				if (status == Status.Success && terminationCondition == TerminationConditions.FirstSuccess){
					return Status.Success;
				}
				
				if (status == Status.Failure && terminationCondition == TerminationConditions.FirstFailure){
					return Status.Failure;
				}

				if (status == Status.Running){
					currentIndex = i;
					return Status.Running;
				}

				
				if (currentIndex == list.Count-1 || currentIndex == maxIteration.value-1){
					if (resetIndex){
						currentIndex = 0;
					}
					return status;
				}

				decoratedConnection.Reset();
				currentIndex ++;				
			}

			return Status.Running;
		}


		protected override void OnReset(){
			if (resetIndex){
				currentIndex = 0;
			}
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){

			var leftLabelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
			leftLabelStyle.richText = true;
			leftLabelStyle.alignment = TextAnchor.UpperLeft;

			GUILayout.Label("For Each \t" + current + "\nIn \t" + targetList, leftLabelStyle);
			if (terminationCondition != TerminationConditions.None)
				GUILayout.Label("Exit on " + terminationCondition.ToString());

			if (Application.isPlaying)
				GUILayout.Label("Index: " + currentIndex.ToString() + " / " + (list != null && list.Count != 0? (list.Count -1).ToString() : "?") );
		}

		protected override void OnNodeInspectorGUI(){
			DrawDefaultInspector();
			if (GUI.changed){
				var argType = targetList.refType != null? targetList.refType.GetEnumerableElementType() : null;
				if (current.varType != argType){
					current.SetType(argType);
				}
			}
		}

		#endif
	}
}