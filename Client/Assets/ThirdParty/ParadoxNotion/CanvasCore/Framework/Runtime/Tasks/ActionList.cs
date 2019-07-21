#if UNITY_EDITOR
using UnityEditor;
using NodeCanvas.Editor;
#endif

using System.Collections.Generic;
using System.Linq;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using ParadoxNotion.Serialization;

namespace NodeCanvas.Framework
{

    ///ActionList is an ActionTask itself that holds multilple ActionTasks which can be executed either in parallel or in sequence.
    [DoNotList]
    public class ActionList : ActionTask, ISubTasksContainer
    {

        public enum ActionsExecutionMode
        {
            ActionsRunInSequence,
            ActionsRunInParallel
        }

        public ActionsExecutionMode executionMode;
        public List<ActionTask> actions = new List<ActionTask>();

        private List<ActionTask> initialActiveActions;

        private int currentActionIndex;
        private readonly List<int> finishedIndeces = new List<int>();

        protected override string info {
            get
            {
                if ( actions.Count == 0 ) {
                    return "No Actions";
                }

                var finalText = actions.Count > 1 ? ( string.Format("<b>({0})</b>\n", executionMode == ActionsExecutionMode.ActionsRunInSequence ? "In Sequence" : "In Parallel") ) : string.Empty;
                for ( var i = 0; i < actions.Count; i++ ) {

                    var action = actions[i];
                    if ( action == null ) {
                        continue;
                    }

                    if ( action.isActive || ( initialActiveActions != null && initialActiveActions.Contains(action) ) ) {
                        var prefix = action.isPaused ? "<b>||</b> " : action.isRunning ? "► " : "▪";
                        finalText += prefix + action.summaryInfo + ( i == actions.Count - 1 ? "" : "\n" );
                    }
                }

                return finalText;
            }
        }

        Task[] ISubTasksContainer.GetSubTasks() {
            return actions.ToArray();
        }

        ///ActionList overrides to duplicate listed actions correctly
        public override Task Duplicate(ITaskSystem newOwnerSystem) {
            var newList = (ActionList)base.Duplicate(newOwnerSystem);
            newList.actions.Clear();
            foreach ( var action in actions ) {
                newList.AddAction((ActionTask)action.Duplicate(newOwnerSystem));
            }

            return newList;
        }

        protected override string OnInit() {
            if ( initialActiveActions == null ) {
                initialActiveActions = actions.Where(a => a.isActive).ToList();
            }
            return null;
        }

        protected override void OnExecute() {
            finishedIndeces.Clear();
            currentActionIndex = 0;
        }

        protected override void OnUpdate() {

            if ( actions.Count == 0 ) {
                EndAction();
                return;
            }

            switch ( executionMode ) {
                case ( ActionsExecutionMode.ActionsRunInParallel ): {
                        for ( var i = 0; i < actions.Count; i++ ) {

                            if ( finishedIndeces.Contains(i) ) {
                                continue;
                            }

                            if ( !actions[i].isActive ) {
                                finishedIndeces.Add(i);
                                continue;
                            }

                            var status = actions[i].ExecuteAction(agent, blackboard);
                            if ( status == Status.Failure ) {
                                EndAction(false);
                                return;
                            }

                            if ( status == Status.Success ) {
                                finishedIndeces.Add(i);
                            }
                        }

                        if ( finishedIndeces.Count == actions.Count ) {
                            EndAction(true);
                        }
                    }
                    break;

                case ( ActionsExecutionMode.ActionsRunInSequence ): {
                        for ( var i = currentActionIndex; i < actions.Count; i++ ) {

                            if ( !actions[i].isActive ) {
                                continue;
                            }

                            var status = actions[i].ExecuteAction(agent, blackboard);
                            if ( status == Status.Failure ) {
                                EndAction(false);
                                return;
                            }

                            if ( status == Status.Running ) {
                                currentActionIndex = i;
                                return;
                            }
                        }

                        EndAction(true);
                    }
                    break;
            }
        }

        protected override void OnStop() {
            for ( var i = 0; i < actions.Count; i++ ) {
                if ( actions[i].isActive ) {
                    actions[i].EndAction(null);
                }
            }
        }

        protected override void OnPause() {
            for ( var i = 0; i < actions.Count; i++ ) {
                if ( actions[i].isActive ) {
                    actions[i].PauseAction();
                }
            }
        }

        public override void OnDrawGizmos() {
            for ( var i = 0; i < actions.Count; i++ ) {
                if ( actions[i].isActive ) {
                    actions[i].OnDrawGizmos();
                }
            }
        }

        public override void OnDrawGizmosSelected() {
            for ( var i = 0; i < actions.Count; i++ ) {
                if ( actions[i].isActive ) {
                    actions[i].OnDrawGizmosSelected();
                }
            }
        }

        public void AddAction(ActionTask action) {

            if ( action is ActionList ) {
                foreach ( var subAction in ( action as ActionList ).actions ) {
                    AddAction(subAction);
                }
                return;
            }

#if UNITY_EDITOR
            if ( !Application.isPlaying ) {
                Undo.RecordObject(ownerSystem.contextObject, "List Add Task");
                currentViewAction = action;
            }
#endif

            actions.Add(action);
            action.SetOwnerSystem(this.ownerSystem);
            // action.isActive = true;
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        private ActionTask currentViewAction;

        //...
        protected override void OnTaskInspectorGUI() {
            ShowListGUI();
            ShowNestedActionsGUI();
        }

        ///Show the sub-tasks list
        public void ShowListGUI() {

            if ( ownerSystem == null ) {
                GUILayout.Label("Owner System is null!");
                return;
            }

            TaskEditor.ShowCreateTaskSelectionButton<ActionTask>(ownerSystem, AddAction);

            ValidateList();

            if ( actions.Count == 0 ) {
                EditorGUILayout.HelpBox("No Actions", MessageType.None);
                return;
            }

            if ( actions.Count == 1 ) {
                return;
            }

            //show the actions
            EditorUtils.ReorderableList(actions, (i, picked) =>
            {
                var action = actions[i];
                GUI.color = Color.white.WithAlpha(action == currentViewAction ? 0.75f : 0.25f);
                EditorGUILayout.BeginHorizontal("box");

                GUI.color = Color.white.WithAlpha(action.isActive ? 0.8f : 0.25f);
                GUI.enabled = !Application.isPlaying;
                action.isActive = EditorGUILayout.Toggle(action.isActive, GUILayout.Width(18));
                GUI.enabled = true;

                GUILayout.Label(( action.isPaused ? "<b>||</b> " : action.isRunning ? "► " : "" ) + action.summaryInfo, GUILayout.MinWidth(0), GUILayout.ExpandWidth(true));

                if ( !Application.isPlaying && GUILayout.Button("X", GUILayout.Width(20)) ) {
                    Undo.RecordObject(ownerSystem.contextObject, "List Remove Task");
                    actions.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();

                var lastRect = GUILayoutUtility.GetLastRect();
                EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.Link);
                if ( Event.current.type == EventType.MouseDown && lastRect.Contains(Event.current.mousePosition) ) {
                    currentViewAction = action == currentViewAction ? null : action;
                    Event.current.Use();
                }

                GUI.color = Color.white;
            });

            executionMode = (ActionsExecutionMode)EditorGUILayout.EnumPopup(executionMode);
        }

        ///Show currently selected task inspector
        public void ShowNestedActionsGUI() {

            if ( actions.Count == 1 ) {
                currentViewAction = actions[0];
            }

            if ( currentViewAction != null ) {
                EditorUtils.Separator();
                TaskEditor.TaskFieldSingle(currentViewAction, (a) =>
                {
                    if ( a == null ) {
                        var i = actions.IndexOf(currentViewAction);
                        actions.RemoveAt(i);
                    }
                    currentViewAction = (ActionTask)a;
                });
            }
        }

        //Validate possible null tasks
        void ValidateList() {
            for ( var i = 0; i < actions.Count; i++ ) {
                if ( actions[i] == null ) {
                    actions.RemoveAt(i);
                }
            }
        }

        [ContextMenu("Save List Preset")]
        void DoSavePreset() {
            var path = EditorUtility.SaveFilePanelInProject("Save Preset", "", "actionList", "");
            if ( !string.IsNullOrEmpty(path) ) {
                System.IO.File.WriteAllText(path, JSONSerializer.Serialize(typeof(ActionList), this, true)); //true for pretyJson
                AssetDatabase.Refresh();
            }
        }

        [ContextMenu("Load List Preset")]
        void DoLoadPreset() {
            var path = EditorUtility.OpenFilePanel("Load Preset", "Assets", "actionList");
            if ( !string.IsNullOrEmpty(path) ) {
                var json = System.IO.File.ReadAllText(path);
                var list = JSONSerializer.Deserialize<ActionList>(json);
                this.actions = list.actions;
                this.executionMode = list.executionMode;
                this.currentViewAction = null;
                foreach ( var a in actions ) {
                    a.SetOwnerSystem(this.ownerSystem);
                }
            }
        }

#endif
    }
}