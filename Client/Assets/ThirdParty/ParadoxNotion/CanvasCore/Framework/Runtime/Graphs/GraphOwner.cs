using System.Collections.Generic;
using System.Linq;
using ParadoxNotion;
using UnityEngine;

namespace NodeCanvas.Framework
{

    /// A component that is used to control a Graph in respects to the gameobject attached to
	abstract public class GraphOwner : MonoBehaviour
    {

        public enum EnableAction
        {
            EnableBehaviour,
            DoNothing
        }

        public enum DisableAction
        {
            DisableBehaviour,
            PauseBehaviour,
            DoNothing
        }

        [SerializeField]
        private string boundGraphSerialization;
        [SerializeField]
        private List<UnityEngine.Object> boundGraphObjectReferences;

        [HideInInspector] ///What will happen OnEnable
		public EnableAction enableAction = EnableAction.EnableBehaviour;
        [HideInInspector] ///What will happen OnDisable
		public DisableAction disableAction = DisableAction.DisableBehaviour;

        ///Raised when the assigned behaviour state is changed (start/pause/stop)
        public static System.Action<GraphOwner> onOwnerBehaviourStateChange;

        private Dictionary<Graph, Graph> instances = new Dictionary<Graph, Graph>();
        private bool initialized = false;
        private bool startCalled = false;

        private static bool isQuiting;

        ///----------------------------------------------------------------------------------------------

        ///The graph assigned
        abstract public Graph graph { get; set; }
        ///The blackboard assigned
        abstract public IBlackboard blackboard { get; set; }
        ///The type of graph that can be assigned
        abstract public System.Type graphType { get; }

        ///Is the assigned graph currently running?
        public bool isRunning {
            get { return graph != null ? graph.isRunning : false; }
        }

        ///Is the assigned graph currently paused?
        public bool isPaused {
            get { return graph != null ? graph.isPaused : false; }
        }

        ///The time is seconds the graph is running
        public float elapsedTime {
            get { return graph != null ? graph.elapsedTime : 0; }
        }

        ///----------------------------------------------------------------------------------------------

        //Gets the instance graph for this owner of the provided graph
        protected Graph GetInstance(Graph originalGraph) {

            if ( originalGraph == null ) {
                return null;
            }

            //in editor the instance is always the original!
#if UNITY_EDITOR
            {
                if ( !Application.isPlaying ) {
                    return originalGraph;
                }
            }
#endif

            //if its already an instance, return the instance
            if ( instances.ContainsValue(originalGraph) ) {
                return originalGraph;
            }

            Graph instance = null;

            //if it's not an instance but rather an asset reference which has been instantiated before, return the instance stored,
            //otherwise create and store a new instance.
            if ( !instances.TryGetValue(originalGraph, out instance) ) {
                instance = Graph.Clone<Graph>(originalGraph);
                instances[originalGraph] = instance;
            }

            return instance;
        }

        ///Start the graph assigned
        public void StartBehaviour() { StartBehaviour(true, null); }
        ///Start the graph assigned providing a callback for when it's finished if at all
        public void StartBehaviour(System.Action<bool> callback) { StartBehaviour(true, callback); }
        ///Start the graph assigned, optionally AutoUpdated or not, and providing a callback for when it's finished if at all
        public void StartBehaviour(bool autoUpdate, System.Action<bool> callback) {
            graph = GetInstance(graph);
            if ( graph != null ) {
                graph.StartGraph(this, blackboard, autoUpdate, callback);
                if ( onOwnerBehaviourStateChange != null ) {
                    onOwnerBehaviourStateChange(this);
                }
            }
        }

        ///Pause the current running graph
        public void PauseBehaviour() {
            if ( graph != null ) {
                graph.Pause();
                if ( onOwnerBehaviourStateChange != null ) {
                    onOwnerBehaviourStateChange(this);
                }
            }
        }

        ///Stop the current running graph
        public void StopBehaviour(bool success = true) {
            if ( graph != null ) {
                graph.Stop(success);
                if ( onOwnerBehaviourStateChange != null ) {
                    onOwnerBehaviourStateChange(this);
                }
            }
        }

        ///Manually update the assigned graph
        public void UpdateBehaviour() {
            if ( graph != null ) {
                graph.UpdateGraph();
            }
        }

        ///The same as calling Stop, Start Behaviour
        public void RestartBehaviour() {
            StopBehaviour();
            StartBehaviour();
        }

        ///----------------------------------------------------------------------------------------------

        ///Shortcut to SendEvent(EventData). Note that this overload does not take a sender argument and thus sender will be null.
        public void SendEvent(string eventName) { SendEvent(new EventData(eventName), null); }
        ///Shortcut to SendEvent(EventData). Note that this overload does not take a sender argument and thus sender will be null.
        public void SendEvent<T>(string eventName, T eventValue) { SendEvent(new EventData<T>(eventName, eventValue), null); }

        ///Send an event through the graph (Same as .graph.SendEvent)
        public void SendEvent(EventData eventData, object sender) {
            if ( graph != null ) {
                graph.SendEvent(eventData, sender);
            }
        }

        ///Initialize bound or reference graph. This is called in Awake automatically,
        ///but it's public so that you can call this manually to pre-initialize when gameobject is deactive, if required.
        public void Initialize() {

            if ( initialized ) {
                return;
            }

            initialized = true;

            //Bound
            if ( !string.IsNullOrEmpty(boundGraphSerialization) ) {
                if ( graph == null ) {
                    graph = (Graph)ScriptableObject.CreateInstance(graphType);
                    graph.name = this.name + " " + graphType.Name;
                    graph.Deserialize(boundGraphSerialization, true, boundGraphObjectReferences);
                    instances[graph] = graph;
                    return;
                }

                //this is done for when instantiating a prefab with a bound graph
                graph.SetSerializationObjectReferences(boundGraphObjectReferences);
            }

            //Asset reference
            graph = GetInstance(graph);
        }

        ///----------------------------------------------------------------------------------------------

        //instantiate and deserialize the bound graph, or instantiate the asset graph reference.
        protected void Awake() {
            Initialize();
        }

        //handle enable behaviour setting
        protected void OnEnable() {
            if ( startCalled && enableAction == EnableAction.EnableBehaviour ) {
                StartBehaviour();
            }
        }

        //we want the first time behaviour starts to be OnStart rather than OnEnable
        protected void Start() {
            startCalled = true;
            if ( enableAction == EnableAction.EnableBehaviour ) {
                StartBehaviour();
            }
        }

        //handle disable behaviour setting
        protected void OnDisable() {

            if ( isQuiting ) {
                return;
            }

            if ( disableAction == DisableAction.DisableBehaviour ) {
                StopBehaviour();
            }

            if ( disableAction == DisableAction.PauseBehaviour ) {
                PauseBehaviour();
            }
        }

        //Destroy instanced graphs as well
        protected void OnDestroy() {

            if ( isQuiting ) {
                return;
            }

            StopBehaviour();

            foreach ( var instanceGraph in instances.Values ) {
                foreach ( var subGraph in instanceGraph.GetAllInstancedNestedGraphs() ) {
                    Destroy(subGraph);
                }
                Destroy(instanceGraph);
            }
        }

        //just set the quit flag
        protected void OnApplicationQuit() {
            isQuiting = true;
        }

        ///----------------------------------------------------------------------------------------------




        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
        ///----------------------------------------------------------------------------------------------
#if UNITY_EDITOR

        private Graph boundGraphInstance;

        ///Editor. Is the graph a bound one?
        public bool graphIsBound {
            get { return boundGraphInstance != null || !string.IsNullOrEmpty(boundGraphSerialization); }
        }

        ///Called in editor only after assigned graph is serialized.
        ///If the graph is bound, we store the serialization data here.
        public void OnAfterGraphSerialized(Graph serializedGraph) {
            if ( graphIsBound && this.graph == serializedGraph ) {
                string newSerialization = null;
                List<Object> newReferences = null;
                graph.GetSerializationData(out newSerialization, out newReferences);
                if ( newSerialization != boundGraphSerialization || !newReferences.SequenceEqual(boundGraphObjectReferences) ) {
                    UnityEditor.Undo.RecordObject(this, "Bound Graph Change");
                    boundGraphSerialization = newSerialization;
                    boundGraphObjectReferences = newReferences;
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
        }

        ///Editor. Validate.
        protected void OnValidate() { Validate(); }
        ///Editor. Validate.
        public void Validate() {

            //everything here is relevant to bound graphs only
            if ( graphIsBound && !Application.isPlaying && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode ) {

#if !UNITY_2018_3_OR_NEWER //works in 2018.3 without this code
                var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
                if ( prefabType == UnityEditor.PrefabType.Prefab ) {
                    //Update from previous version. No longer store graph as sub-asset.
                    if ( graph != null && UnityEditor.AssetDatabase.IsSubAsset(graph) ) {
                        DestroyImmediate(graph, true);
                    }
                }
#endif

                if ( boundGraphInstance == null ) {
                    boundGraphInstance = (Graph)ScriptableObject.CreateInstance(graphType);
                }

#if !UNITY_2018_3_OR_NEWER //works in 2018.3 without this code
                boundGraphInstance.hideFlags = prefabType == UnityEditor.PrefabType.Prefab ? HideFlags.HideAndDontSave : HideFlags.None;
#endif

                boundGraphInstance.Deserialize(boundGraphSerialization, false, boundGraphObjectReferences);
                ( boundGraphInstance as UnityEngine.Object ).name = this.name + " " + graphType.Name;
                boundGraphInstance.UpdateReferencesFromOwner(this);
                boundGraphInstance.Validate();
                boundGraphSerialization = boundGraphInstance.Serialize(false, boundGraphObjectReferences);

                graph = boundGraphInstance;
            }
        }


        ///Editor. Binds the target graph (null to unbind current).
        public void SetBoundGraphReference(Graph target) {

            if ( Application.isPlaying || UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode ) {
                Debug.LogError("SetBoundGraphReference method is an Editor only method!");
                return;
            }

            graph = null;
            boundGraphInstance = null;
            if ( target == null ) {
                boundGraphSerialization = null;
                boundGraphObjectReferences = null;
                return;
            }

            target.Serialize();
            target.GetSerializationData(out boundGraphSerialization, out boundGraphObjectReferences);
            Validate(); //validate to handle bound graph instance
        }

        ///Reset unity callback
        protected void Reset() {
            blackboard = gameObject.GetComponent<Blackboard>();
            if ( blackboard == null ) {
                blackboard = gameObject.AddComponent<Blackboard>();
            }
        }

        ///Forward Gizmos callback
        protected void OnDrawGizmos() {
            Gizmos.DrawIcon(transform.position, "GraphOwnerGizmo.png", true);
            DoGraphGizmos(graph);
        }

        ///Show gizmos for nodes and tasks
        static void DoGraphGizmos(Graph targetGraph) {
            if ( targetGraph != null ) {
                for ( var i = 0; i < targetGraph.allNodes.Count; i++ ) {
                    var node = targetGraph.allNodes[i];
                    node.OnDrawGizmos();
                    if ( NodeCanvas.Editor.GraphEditorUtility.activeElement == node ) {
                        node.OnDrawGizmosSelected();
                    }
                    var graphAssignable = node as IGraphAssignable;
                    if ( graphAssignable != null && graphAssignable.nestedGraph != null ) {
                        DoGraphGizmos(graphAssignable.nestedGraph);
                    }
                }
            }
        }

#endif
    }


    ///----------------------------------------------------------------------------------------------

    ///The class where GraphOwners derive from
    abstract public class GraphOwner<T> : GraphOwner where T : Graph
    {

        [SerializeField]
        private T _graph;
        [SerializeField]
        private Object _blackboard;

        ///The current behaviour Graph assigned
        sealed public override Graph graph {
            get { return _graph; }
            set { _graph = (T)value; }
        }

        ///The current behaviour Graph assigned (same as .graph but of type T)
        public T behaviour {
            get { return _graph; }
            set { _graph = value; }
        }

        ///The blackboard that the assigned behaviour will be Started with or currently using
        sealed public override IBlackboard blackboard {
            get
            {
                if ( graph != null && graph.useLocalBlackboard ) {
                    return graph.localBlackboard;
                }

                if ( _blackboard == null ) {
                    _blackboard = GetComponent<Blackboard>();
                }

                return _blackboard as IBlackboard;
            }
            set
            {
                if ( !ReferenceEquals(_blackboard, value) ) {
                    _blackboard = (Blackboard)(object)value;
                    if ( graph != null && !graph.useLocalBlackboard ) {
                        graph.UpdateReferences(this, value);
                    }
                }
            }
        }

        ///The Graph type this Owner can be assigned
        sealed public override System.Type graphType { get { return typeof(T); } }

        ///Start a new behaviour on this owner
        public void StartBehaviour(T newGraph) { StartBehaviour(newGraph, true, null); }
        ///Start a new behaviour on this owner and get a call back for when it's finished if at all
        public void StartBehaviour(T newGraph, System.Action<bool> callback) { StartBehaviour(newGraph, true, callback); }
        ///Start a new behaviour on this owner and get a call back for when it's finished if at all
        public void StartBehaviour(T newGraph, bool autoUpdate, System.Action<bool> callback) {
            SwitchBehaviour(newGraph, autoUpdate, callback);
        }

        ///Use to switch the behaviour dynamicaly at runtime
        public void SwitchBehaviour(T newGraph) { SwitchBehaviour(newGraph, true, null); }
        ///Use to switch or set graphs at runtime and optionaly get a callback when it's finished if at all
        public void SwitchBehaviour(T newGraph, System.Action<bool> callback) { SwitchBehaviour(newGraph, true, callback); }
        ///Use to switch or set graphs at runtime and optionaly get a callback when it's finished if at all
        public void SwitchBehaviour(T newGraph, bool autoUpdate, System.Action<bool> callback) {
            StopBehaviour();
            graph = newGraph;
            StartBehaviour(autoUpdate, callback);
        }
    }
}