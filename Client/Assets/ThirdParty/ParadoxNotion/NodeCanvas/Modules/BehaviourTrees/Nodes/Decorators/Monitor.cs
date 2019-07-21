using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.BehaviourTrees
{

    [Category("Decorators")]
    [Icon("Eye")]
    [Description("Monitors the decorated child node for a returned Status and executes an Action when that is the case.\nThe final Status returned to the parent can either be the original Decorated Child Node Status, or the new Decorator Action Status.")]
    public class Monitor : BTDecorator, ITaskAssignable<ActionTask>
    {

        public enum MonitorMode
        {
            Failure = 0,
            Success = 1,
            AnyStatus = 10,
        }

        public enum ReturnStatusMode
        {
            OriginalDecoratedChildStatus,
            NewDecoratorActionStatus,
        }

        [Name("Monitor")]
        public MonitorMode monitorMode;
        [Name("Return")]
        public ReturnStatusMode returnMode;

        private Status decoratorActionStatus;

        [SerializeField]
        private ActionTask _action;

        public ActionTask action {
            get { return _action; }
            set { _action = value; }
        }

        public Task task {
            get { return action; }
            set { action = (ActionTask)value; }
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            var newChildStatus = decoratedConnection.Execute(agent, blackboard);

            if ( status != newChildStatus ) {
                var execute = false;
                if ( newChildStatus == Status.Success && monitorMode == MonitorMode.Success ) {
                    execute = true;
                }
                if ( newChildStatus == Status.Failure && monitorMode == MonitorMode.Failure ) {
                    execute = true;
                }
                if ( monitorMode == MonitorMode.AnyStatus && newChildStatus != Status.Running ) {
                    execute = true;
                }

                if ( execute ) {
                    decoratorActionStatus = action.ExecuteAction(agent, blackboard);
                    if ( decoratorActionStatus == Status.Running ) {
                        return Status.Running;
                    }
                }
            }

            return returnMode == ReturnStatusMode.NewDecoratorActionStatus && decoratorActionStatus != Status.Resting ? decoratorActionStatus : newChildStatus;
        }

        protected override void OnReset() {
            action.EndAction(null);
            decoratorActionStatus = Status.Resting;
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Label(string.Format("<b>[On {0}]</b>", monitorMode.ToString()));
        }

#endif
        ///---------------------------------------UNITY EDITOR-------------------------------------------
    }
}