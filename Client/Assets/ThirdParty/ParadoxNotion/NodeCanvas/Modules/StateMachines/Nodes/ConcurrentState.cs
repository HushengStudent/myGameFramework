using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.StateMachines
{

    [Name("Concurrent")]
    [Description("Execute a number of Actions with optional conditional requirement and in parallel to any other state, as soon as the FSM is started.\nAll actions will prematurely be stoped as soon as the FSM stops as well.\nThis is not a state per-se and thus can have neither incomming, nor outgoing transitions.")]
    [Color("ff64cb")]
    public class ConcurrentState : FSMState, IUpdatable, ISubTasksContainer
    {

        [SerializeField]
        private ConditionList _conditionList;
        [SerializeField]
        private ActionList _actionList;
        [SerializeField]
        private bool _repeatStateActions;

        private bool accessed;

        public ConditionList conditionList {
            get { return _conditionList; }
            set { _conditionList = value; }
        }

        public ActionList actionList {
            get { return _actionList; }
            set { _actionList = value; }
        }

        public Task[] GetSubTasks() {
            return new Task[] { _conditionList, _actionList };
        }

        public bool repeatStateActions {
            get { return _repeatStateActions; }
            set { _repeatStateActions = value; }
        }

        public override string name {
            get { return base.name.ToUpper(); }
        }

        public override int maxInConnections { get { return 0; } }
        public override int maxOutConnections { get { return 0; } }
        public override bool allowAsPrime { get { return false; } }

        public override void OnValidate(Graph assignedGraph) {
            if ( conditionList == null ) {
                conditionList = (ConditionList)Task.Create(typeof(ConditionList), assignedGraph);
                conditionList.checkMode = ConditionList.ConditionsCheckMode.AllTrueRequired;
            }

            if ( actionList == null ) {
                actionList = (ActionList)Task.Create(typeof(ActionList), assignedGraph);
                actionList.executionMode = ActionList.ActionsExecutionMode.ActionsRunInParallel;
            }
        }

        protected override void OnEnter() {
            conditionList.Enable(graphAgent, graphBlackboard);
            accessed = false;
            Update();
        }

        new public void Update() {
            if ( status == Status.Resting || status == Status.Running ) {
                if ( conditionList.CheckCondition(graphAgent, graphBlackboard) ) {
                    accessed = true;
                }
                if ( accessed && actionList.ExecuteAction(graphAgent, graphBlackboard) != Status.Running ) {
                    accessed = false;
                    if ( !repeatStateActions ) {
                        Finish();
                    }
                }
            }
        }

        protected override void OnExit() {
            conditionList.Disable();
            actionList.EndAction(null);
        }

        protected override void OnPause() {
            actionList.PauseAction();
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            if ( repeatStateActions ) {
                GUILayout.Label("<b>[REPEAT]</b>");
            }
            if ( conditionList.conditions.Count > 0 ) {
                GUILayout.BeginVertical(Styles.roundedBox);
                GUILayout.Label(conditionList.summaryInfo);
                GUILayout.EndVertical();
            }

            GUILayout.BeginVertical(Styles.roundedBox);
            GUILayout.Label(actionList.summaryInfo);
            GUILayout.EndVertical();

            base.OnNodeGUI();
        }

        protected override void OnNodeInspectorGUI() {

            repeatStateActions = UnityEditor.EditorGUILayout.ToggleLeft("Repeat", repeatStateActions);
            EditorUtils.Separator();

            EditorUtils.CoolLabel("Conditions (optional)");
            conditionList.ShowListGUI();
            conditionList.ShowNestedConditionsGUI();

            EditorUtils.BoldSeparator();

            EditorUtils.CoolLabel("Actions");
            actionList.ShowListGUI();
            actionList.ShowNestedActionsGUI();
        }

#endif
    }
}