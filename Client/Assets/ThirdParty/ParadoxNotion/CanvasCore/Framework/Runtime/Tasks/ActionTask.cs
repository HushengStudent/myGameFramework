using System;
using System.Collections;
using ParadoxNotion.Services;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;
using NodeCanvas.Framework.Internal;
using UnityEngine;


namespace NodeCanvas.Framework
{

    ///Base class for all actions. Extend this to create your own. T is the agentType required by the action.
    ///Generic version where T is the AgentType (Behaviour or Interface) required by the Action.
    abstract public class ActionTask<T> : ActionTask where T : class
    {
        sealed public override Type agentType { get { return typeof(T); } }
        new public T agent { get { return base.agent as T; } }
    }

    ///----------------------------------------------------------------------------------------------

#if UNITY_EDITOR //handles missing types
    [fsObject(Processor = typeof(fsRecoveryProcessor<ActionTask, MissingAction>))]
#endif

    ///Base class for all actions. Extend this to create your own.
    abstract public class ActionTask : Task
    {

        [NonSerialized]
        private Status status = Status.Resting;
        [NonSerialized]
        private float startedTime;
        [NonSerialized]
        private float pausedTime;
        [NonSerialized]
        private bool latch;
        [NonSerialized]
        private bool _isPaused;

        ///The time in seconds this action is running if at all
        public float elapsedTime {
            get
            {
                if ( isPaused ) {
                    return pausedTime - startedTime;
                }
                if ( isRunning ) {
                    return Time.time - startedTime;
                }
                return 0;
            }
        }

        ///Is the action currently running?
        public bool isRunning {
            get { return status == Status.Running; }
        }

        ///Is the action currently paused?
        public bool isPaused {
            get { return _isPaused; }
            private set { _isPaused = value; }
        }

        ///----------------------------------------------------------------------------------------------

        ///Used to call an action for standalone execution providing a callback.
        ///Be careful! *This will make the action execute as a coroutine*
        public void ExecuteAction(Component agent, IBlackboard blackboard, Action<bool> callback) {
            if ( !isRunning ) { MonoManager.current.StartCoroutine(ActionUpdater(agent, blackboard, callback)); }
        }

        //The internal updater for when an action has been called with a callback parameter and only then.
        //This is only used and usefull if user needs to execute an action task completely outside a graph as standalone.
        IEnumerator ActionUpdater(Component agent, IBlackboard blackboard, Action<bool> callback) {
            while ( ExecuteAction(agent, blackboard) == Status.Running ) { yield return null; }
            if ( callback != null ) { callback(status == Status.Success ? true : false); }
        }

        ///----------------------------------------------------------------------------------------------

        ///Ticks the action for the provided agent and blackboard
        public Status ExecuteAction(Component agent, IBlackboard blackboard) {

            if ( !isActive ) {
                return Status.Failure;
            }

            if ( isPaused ) { //then resume
                startedTime += Time.time - pausedTime;
                isPaused = false;
            }

            if ( status == Status.Running ) {
                OnUpdate();
                latch = false;
                return status;
            }

            if ( latch ) { //to be possible to call EndAction anywhere
                latch = false;
                return status;
            }

            if ( !Set(agent, blackboard) ) {
                latch = false;
                return Status.Failure;
            }

            startedTime = Time.time;
            status = Status.Running;
            OnExecute();
            if ( status == Status.Running ) {
                OnUpdate();
            }
            latch = false;
            return status;
        }

        ///Ends the action either in success or failure. Ending with null means that it's a cancel/interrupt.
        ///Null is used by the external system. You should use true or false when calling EndAction within it.
        public void EndAction() { EndAction(true); }
        public void EndAction(bool success) { EndAction((bool?)success); }
        public void EndAction(bool? success) {

            if ( status != Status.Running ) {
                return;
            }

            latch = success != null ? true : false;

            isPaused = false;
            status = success == null ? Status.Resting : ( success == true ? Status.Success : Status.Failure );
            OnStop(success == null);
        }

        ///Pause the action from updating and calls OnPause
        public void PauseAction() {

            if ( status != Status.Running ) {
                return;
            }

            pausedTime = Time.time;
            isPaused = true;
            OnPause();
        }

        ///Called once when the actions is executed.
        virtual protected void OnExecute() { }

        ///Called every frame, if and while the action is running and until it ends.
        virtual protected void OnUpdate() { }

        ///Called whenever the action ends due to any reason with the argument denoting whether the action was interrupted or finished properly.
        virtual protected void OnStop(bool interrupted) { OnStop(); }

        ///Called whenever the action ends due to any reason.
        virtual protected void OnStop() { }

        ///Called when the action gets paused
        virtual protected void OnPause() { }
    }
}