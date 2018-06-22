using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.StateMachines{

	[Name("Action State")]
	[Description("Execute a number of Action Tasks OnEnter. All actions will be stoped OnExit. This state is Finished when all Actions are finished as well")]
	public class ActionState : FSMState, ITaskAssignable{

		[SerializeField]
		private ActionList _actionList;
		[SerializeField]
		private bool _repeatStateActions;

		public Task task{
			get {return actionList;}
			set {actionList = (ActionList)value;}
		}
		
		public ActionList actionList{
			get { return _actionList; }
			set { _actionList = value; }
		}

		public bool repeatStateActions{
			get {return _repeatStateActions;}
			set {_repeatStateActions = value;}
		}

		public override void OnValidate(Graph assignedGraph){
			if (actionList == null){
				actionList = (ActionList)Task.Create(typeof(ActionList), assignedGraph);
				actionList.executionMode = ActionList.ActionsExecutionMode.ActionsRunInParallel;
			}
		}

		protected override void OnEnter(){ OnUpdate(); }

		protected override void OnUpdate(){
			var actionListStatus = actionList.ExecuteAction(graphAgent, graphBlackboard);
			if (!repeatStateActions && actionListStatus != Status.Running){
				Finish(actionListStatus == Status.Success? true : false);
			}
		}

		protected override void OnExit(){
			actionList.EndAction(null);
		}

		protected override void OnPause(){
			actionList.PauseAction();
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){
			if (repeatStateActions){
				GUILayout.Label("<b>[REPEAT]</b>");
			}
			base.OnNodeGUI();
		}

		protected override void OnNodeInspectorGUI(){

			ShowBaseFSMInspectorGUI();

			if (actionList == null){
				return;
			}

			EditorUtils.CoolLabel("Actions");
			GUI.color = repeatStateActions? GUI.color : new Color(1,1,1,0.5f);
			repeatStateActions = UnityEditor.EditorGUILayout.ToggleLeft("Repeat State Actions", repeatStateActions);
			GUI.color = Color.white;
			actionList.ShowListGUI();
			actionList.ShowNestedActionsGUI();
		}

		#endif
	}
}