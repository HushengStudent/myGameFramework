using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Name("Conditional")]
	[Category("Decorators")]
	[Description("Execute and return the child node status if the condition is true, otherwise return Failure. The condition is evaluated only once in the first Tick and when the node is not already Running unless it is set as 'Dynamic' in which case it will revaluate even while running")]
	[Icon("Accessor")]
	public class ConditionalEvaluator : BTDecorator, ITaskAssignable<ConditionTask> {

		public bool isDynamic;

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

			if (decoratedConnection == null){
				return Status.Resting;
			}

			if (condition == null){
				return decoratedConnection.Execute(agent, blackboard);
			}

			if (isDynamic)
			{
				if (condition.CheckCondition(agent, blackboard)){
					return decoratedConnection.Execute(agent, blackboard);
				}
				decoratedConnection.Reset();
				return Status.Failure;
			}
			else
			{
				if (status != Status.Running && condition.CheckCondition(agent, blackboard)){
					accessed = true;
				}

				return accessed? decoratedConnection.Execute(agent, blackboard) : Status.Failure;
			}
		}

		protected override void OnReset(){
			accessed = false;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		protected override void OnNodeGUI(){
			if (isDynamic)
				GUILayout.Label("<b>DYNAMIC</b>");
		}

		protected override void OnNodeInspectorGUI(){
			isDynamic = UnityEditor.EditorGUILayout.Toggle("Dynamic", isDynamic);
			EditorUtils.Separator();
		}
		
		#endif
	}
}