using System.Linq;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.StateMachines
{

    /// Use FSMs to create state like behaviours
    [GraphInfo(
        packageName = "NodeCanvas",
        docsURL = "http://nodecanvas.paradoxnotion.com/documentation/",
        resourcesURL = "http://nodecanvas.paradoxnotion.com/downloads/",
        forumsURL = "http://nodecanvas.paradoxnotion.com/forums-page/"
        )]
    [CreateAssetMenu(menuName = "ParadoxNotion/NodeCanvas/FSM Asset")]
    public class FSM : Graph
    {

        private bool hasInitialized;

        private List<IUpdatable> updatableNodes;
        private List<AnyState> anyStates;
        private List<ConcurrentState> concurentStates;

        public event System.Action<IState> onStateEnter;
        public event System.Action<IState> onStateUpdate;
        public event System.Action<IState> onStateExit;
        public event System.Action<IState> onStateTransition;

        public FSMState currentState { get; private set; }
        public FSMState previousState { get; private set; }

        ///The current state name. Null if none
        public string currentStateName {
            get { return currentState != null ? currentState.name : null; }
        }

        ///The previous state name. Null if none
        public string previousStateName {
            get { return previousState != null ? previousState.name : null; }
        }


        public override System.Type baseNodeType { get { return typeof(FSMState); } }
        public override bool requiresAgent { get { return true; } }
        public override bool requiresPrimeNode { get { return true; } }
        public override bool isTree { get { return false; } }
        public override bool useLocalBlackboard { get { return false; } }
        sealed public override bool canAcceptVariableDrops { get { return false; } }


        protected override void OnGraphStarted() {

            if ( !hasInitialized ) {

                hasInitialized = true;

                GatherDelegates();

                updatableNodes = new List<IUpdatable>();
                anyStates = new List<AnyState>();
                concurentStates = new List<ConcurrentState>();

                for ( var i = 0; i < allNodes.Count; i++ ) {
                    var node = allNodes[i] as FSMState;
                    if ( node == null ) {
                        continue;
                    }
                    if ( node is IUpdatable ) {
                        updatableNodes.Add((IUpdatable)node);
                    }
                    if ( node is AnyState ) {
                        anyStates.Add((AnyState)node);
                    }
                    if ( node is ConcurrentState ) {
                        concurentStates.Add((ConcurrentState)node);
                    }
                }
            }

            //Trigger AnyStates
            for ( var i = 0; i < anyStates.Count; i++ ) {
                anyStates[i].Execute(agent, blackboard);
            }

            //Trigger ConcurrentStates
            for ( var i = 0; i < concurentStates.Count; i++ ) {
                concurentStates[i].Execute(agent, blackboard);
            }

            //Enter the last or "start" state
            EnterState(previousState == null ? (FSMState)primeNode : previousState);
        }

        protected override void OnGraphUnpaused() {
            //Enter the last or "start" state
            EnterState(previousState == null ? (FSMState)primeNode : previousState);
        }

        protected override void OnGraphUpdate() {

            //if null state, stop.
            if ( currentState == null ) {
                Stop(false);
                return;
            }

            //if nowhere else to go, stop.
            if ( currentState.status != Status.Running && currentState.outConnections.Count == 0 ) {
                if ( anyStates.Count == 0 ) {
                    Stop(true);
                    return;
                }
            }

            //Update AnyStates and ConcurentStates
            for ( var i = 0; i < updatableNodes.Count; i++ ) {
                updatableNodes[i].Update();
            }

            if ( currentState != null ) {
                //Update current state
                currentState.Update();
                if ( onStateUpdate != null && currentState.status == Status.Running ) {
                    onStateUpdate(currentState);
                }
            }
        }

        protected override void OnGraphStoped() {
            if ( currentState != null ) {
                if ( onStateExit != null ) {
                    onStateExit(currentState);
                }
            }

            previousState = null;
            currentState = null;
        }

        protected override void OnGraphPaused() {
            previousState = currentState;
            currentState = null;
        }

        ///Enter a state providing the state itself
        public bool EnterState(FSMState newState) {

            if ( !isRunning ) {
                Debug.LogWarning("Tried to EnterState on an FSM that was not running", this);
                return false;
            }

            if ( newState == null ) {
                Debug.LogWarning("Tried to Enter Null State");
                return false;
            }

            if ( currentState != null ) {

                if ( onStateExit != null ) {
                    onStateExit(currentState);
                }

                currentState.Reset();

#if UNITY_EDITOR
                //Done for visualizing in editor
                for ( var i = 0; i < currentState.inConnections.Count; i++ ) {
                    currentState.inConnections[i].status = Status.Resting;
                }
#endif
            }

            previousState = currentState;
            currentState = newState;

            if ( onStateTransition != null ) {
                onStateTransition(currentState);
            }

            if ( onStateEnter != null ) {
                onStateEnter(currentState);
            }

            currentState.Execute(agent, blackboard);
            return true;
        }

        ///Trigger a state to enter by it's name. Returns the state found and entered if any
        public FSMState TriggerState(string stateName) {

            var state = GetStateWithName(stateName);
            if ( state != null ) {
                EnterState(state);
                return state;
            }

            Debug.LogWarning("No State with name '" + stateName + "' found on FSM '" + name + "'");
            return null;
        }

        ///Get all State Names
        public string[] GetStateNames() {
            return allNodes.Where(n => n.allowAsPrime).Select(n => n.name).ToArray();
        }

        ///Get a state by it's name
        public FSMState GetStateWithName(string name) {
            return (FSMState)allNodes.Find(n => n.allowAsPrime && n.name == name);
        }

        //Gather and creates delegates from MonoBehaviours on agents that implement required methods
        void GatherDelegates() {

            if ( agent == null ) { return; }

            var monos = agent.gameObject.GetComponents<MonoBehaviour>();
            var args = new System.Type[] { typeof(IState) };
            for ( var i = 0; i < monos.Length; i++ ) {
                var mono = monos[i];
                var monoType = mono.GetType();
                var enterMethod = monoType.RTGetMethod("OnStateEnter", args);
                var stayMethod = monoType.RTGetMethod("OnStateUpdate", args);
                var exitMethod = monoType.RTGetMethod("OnStateExit", args);

                if ( enterMethod != null ) {
                    try { onStateEnter += enterMethod.RTCreateDelegate<System.Action<IState>>(mono); } //JIT
                    catch { onStateEnter += (m) => { enterMethod.Invoke(mono, ReflectionTools.SingleTempArgsArray(m)); }; } //AOT
                }

                if ( stayMethod != null ) {
                    try { onStateUpdate += stayMethod.RTCreateDelegate<System.Action<IState>>(mono); } //JIT
                    catch { onStateUpdate += (m) => { stayMethod.Invoke(mono, ReflectionTools.SingleTempArgsArray(m)); }; } //AOT
                }

                if ( exitMethod != null ) {
                    try { onStateExit += exitMethod.RTCreateDelegate<System.Action<IState>>(mono); } //JIT
                    catch { onStateExit += (m) => { exitMethod.Invoke(mono, ReflectionTools.SingleTempArgsArray(m)); }; } //AOT
                }
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        [UnityEditor.MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/State Machine Asset", false, 0)]
        static void Editor_CreateGraph() {
            var newGraph = EditorUtils.CreateAsset<FSM>();
            UnityEditor.Selection.activeObject = newGraph;
        }

#endif
    }
}