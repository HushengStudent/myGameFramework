using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Services;
using UnityEngine;


namespace NodeCanvas.Framework
{


    ///The base class for all nodes that can live in a NodeCanvas Graph

#if UNITY_EDITOR //handles missing Nodes
    [fsObject(Processor = typeof(fsRecoveryProcessor<Node, MissingNode>))]
#endif

    [System.Serializable]
    [ParadoxNotion.Design.SpoofAOT]
    abstract public partial class Node : IGraphElement
    {

        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private string _UID;
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _tag;
        [SerializeField]
        private string _comment;
        [SerializeField]
        private bool _isBreakpoint;

        //reconstructed OnDeserialization
        private Graph _graph;
        //reconstructed OnDeserialization
        private List<Connection> _inConnections = new List<Connection>();
        //reconstructed OnDeserialization
        private List<Connection> _outConnections = new List<Connection>();
        //reconstructed OnDeserialization
        private int _ID;

        [System.NonSerialized]
        private Status _status = Status.Resting;
        [System.NonSerialized]
        private string _nameCache;
        [System.NonSerialized]
        private string _descriptionCache;
        [System.NonSerialized]
        private int _priorityCache = int.MinValue;

        /////

        ///The graph this node belongs to.
        public Graph graph {
            get { return _graph; }
            set { _graph = value; }
        }

        ///The node's int ID in the graph.
        public int ID {
            get { return _ID; }
            set { _ID = value; }
        }

        ///All incomming connections to this node.
        public List<Connection> inConnections {
            get { return _inConnections; }
            protected set { _inConnections = value; }
        }

        ///All outgoing connections from this node.
        public List<Connection> outConnections {
            get { return _outConnections; }
            protected set { _outConnections = value; }
        }

        ///The position of the node in the graph.
        public Vector2 position {
            get { return _position; }
            set { _position = value; }
        }

        ///The Unique ID of the node. One is created only if requested.
        public string UID {
            get { return string.IsNullOrEmpty(_UID) ? _UID = System.Guid.NewGuid().ToString() : _UID; }
        }

        ///The custom title name of the node if any.
        private string customName {
            get { return _name; }
            set { _name = value; }
        }

        ///The node tag. Useful for finding nodes through code.
        public string tag {
            get { return _tag; }
            set { _tag = value; }
        }

        ///The comments of the node if any.
        public string comments {
            get { return _comment; }
            set { _comment = value; }
        }

        ///Is the node set as a breakpoint?
        public bool isBreakpoint {
            get { return _isBreakpoint; }
            set { _isBreakpoint = value; }
        }


        ///The title name of the node shown in the window if editor is not in Icon Mode. This is a property so title name may change instance wise
        virtual public string name {
            get
            {
                if ( !string.IsNullOrEmpty(customName) ) {
                    return customName;
                }

                if ( string.IsNullOrEmpty(_nameCache) ) {
                    var nameAtt = this.GetType().RTGetAttribute<NameAttribute>(true);
                    _nameCache = nameAtt != null ? nameAtt.name : GetType().FriendlyName().SplitCamelCase();
                }
                return _nameCache;
            }
            set { customName = value; }
        }

        ///The description info of the node
        virtual public string description {
            get
            {
                if ( string.IsNullOrEmpty(_descriptionCache) ) {
                    var descAtt = this.GetType().RTGetAttribute<DescriptionAttribute>(true);
                    _descriptionCache = descAtt != null ? descAtt.description : "No Description";
                }
                return _descriptionCache;
            }
        }

        ///The execution priority order of the node when it matters to the graph system
        virtual public int priority {
            get
            {
                if ( _priorityCache == int.MinValue ) {
                    var prioAtt = this.GetType().RTGetAttribute<ExecutionPriorityAttribute>(true);
                    _priorityCache = prioAtt != null ? prioAtt.priority : 0;
                }
                return _priorityCache;
            }
        }

        ///The numer of possible inputs. -1 for infinite.
        abstract public int maxInConnections { get; }
        ///The numer of possible outputs. -1 for infinite.
        abstract public int maxOutConnections { get; }
        ///The output connection Type this node has.
        abstract public System.Type outConnectionType { get; }
        ///Can this node be set as prime (Start)?
        abstract public bool allowAsPrime { get; }
        ///Alignment of the comments when shown.
        abstract public Alignment2x2 commentsAlignment { get; }
        ///Alignment of the icons.
        abstract public Alignment2x2 iconAlignment { get; }


        ///The current status of the node
        public Status status {
            get { return _status; }
            protected set { _status = value; }
        }

        ///The current agent. Taken from the graph this node belongs to
        public Component graphAgent {
            get { return graph != null ? graph.agent : null; }
        }

        ///The current blackboard. Taken from the graph this node belongs to
        public IBlackboard graphBlackboard {
            get { return graph != null ? graph.blackboard : null; }
        }

        //Used to check recursion
        private bool isChecked { get; set; }
        //used to flag breakpoint reached
        private bool breakPointReached { get; set; }


        ///----------------------------------------------------------------------------------------------
        ///----------------------------------------------------------------------------------------------

        //required
        public Node() { }

        ///Create a new Node of type and assigned to the provided graph. Use this for constructor
        public static Node Create(Graph targetGraph, System.Type nodeType, Vector2 pos) {

            if ( targetGraph == null ) {
                ParadoxNotion.Services.Logger.LogError("Can't Create a Node without providing a Target Graph", "NodeCanvas");
                return null;
            }

            var newNode = (Node)System.Activator.CreateInstance(nodeType);

            if ( targetGraph != null ) {
                targetGraph.RecordUndo("Create Node");
            }

            newNode.graph = targetGraph;
            newNode.position = pos;
            BBParameter.SetBBFields(newNode, targetGraph.blackboard);

            newNode.OnValidate(targetGraph);
            newNode.OnCreate(targetGraph);
            return newNode;
        }

        ///Duplicate node alone assigned to the provided graph
        public Node Duplicate(Graph targetGraph) {

            if ( targetGraph == null ) {
                ParadoxNotion.Services.Logger.LogError("Can't duplicate a Node without providing a Target Graph", "NodeCanvas");
                return null;
            }

            //deep clone
            var newNode = JSONSerializer.Clone<Node>(this);

            if ( targetGraph != null ) {
                targetGraph.RecordUndo("Duplicate Node");
            }

            targetGraph.allNodes.Add(newNode);
            newNode.inConnections.Clear();
            newNode.outConnections.Clear();

            if ( targetGraph == this.graph ) {
                newNode.position += new Vector2(50, 50);
            }

            newNode._UID = null;
            newNode.graph = targetGraph;
            BBParameter.SetBBFields(newNode, targetGraph.blackboard);

            var assignable = this as ITaskAssignable;
            if ( assignable != null && assignable.task != null ) {
                ( newNode as ITaskAssignable ).task = assignable.task.Duplicate(targetGraph);
            }

            newNode.OnValidate(targetGraph);
            return newNode;
        }

        ///Called once the first time node is created.
        virtual public void OnCreate(Graph assignedGraph) { }
        ///Called when the Node is created, duplicated or otherwise needs validation.
        virtual public void OnValidate(Graph assignedGraph) { }
        ///Called when the Node is removed from the graph (always through graph.RemoveNode)
        virtual public void OnDestroy() { }

        ///----------------------------------------------------------------------------------------------

        ///The main execution function of the node. Execute the node for the agent and blackboard provided.
        public Status Execute(Component agent, IBlackboard blackboard) {

#if UNITY_EDITOR
            if ( isBreakpoint ) {
                if ( status == Status.Resting ) {
                    var breakEditor = NodeCanvas.Editor.Prefs.breakpointPauseEditor;
                    var owner = agent as GraphOwner;
                    var contextName = owner != null ? owner.gameObject.name : graph.name;
                    ParadoxNotion.Services.Logger.LogWarning(string.Format("Node: '{0}' | ID: '{1}' | Graph Type: '{2}' | Context Object: '{3}'", name, ID, graph.GetType().Name, contextName), "Breakpoint", this);
                    if ( owner != null ) { owner.PauseBehaviour(); }
                    if ( breakEditor ) { StartCoroutine(YieldBreak(() => { if ( owner != null ) { owner.StartBehaviour(); } })); }
                    breakPointReached = true;
                    status = Status.Running;
                    return Status.Running;
                }
                if ( breakPointReached ) {
                    breakPointReached = false;
                    status = Status.Resting;
                }
            }
#endif

            status = OnExecute(agent, blackboard);
            return status;
        }

        ///Recursively reset the node and child nodes if it's not Resting already
        public void Reset(bool recursively = true) {

            if ( status == Status.Resting || isChecked ) {
                return;
            }

            OnReset();
            status = Status.Resting;

            isChecked = true;
            for ( var i = 0; i < outConnections.Count; i++ ) {
                outConnections[i].Reset(recursively);
            }
            isChecked = false;
        }

        ///----------------------------------------------------------------------------------------------

        ///Helper for breakpoints
        IEnumerator YieldBreak(System.Action resume) {
            Debug.Break();
            yield return null;
            resume();
        }

        ///Helper for easier logging
        public Status Error(object msg) {
            if ( msg is System.Exception ) {
                ParadoxNotion.Services.Logger.LogException((System.Exception)msg, "Execution", this);
            } else {
                ParadoxNotion.Services.Logger.LogError(msg, "Execution", this);
            }
            status = Status.Error;
            return Status.Error;
        }

        ///Helper for easier logging
        public Status Fail(string msg) {
            ParadoxNotion.Services.Logger.LogError(msg, "Execution", this);
            status = Status.Failure;
            return Status.Failure;
        }

        ///Helper for easier logging
        public void Warn(string msg) {
            ParadoxNotion.Services.Logger.LogWarning(msg, "Execution", this);
        }

        ///Set the Status of the node directly. Not recomended if you don't know why!
        public void SetStatus(Status status) {
            this.status = status;
        }

        ///----------------------------------------------------------------------------------------------

        ///Sends an event to the graph (same as calling graph.SendEvent)
        protected void SendEvent(EventData eventData) {
            graph.SendEvent(eventData, this);
        }

        ///Subscribe the node to a unity message send to the agent
        public void RegisterEvents(params string[] eventNames) { RegisterEvents(graphAgent, eventNames); }
        public void RegisterEvents(Component targetAgent, params string[] eventNames) {
            if ( targetAgent == null ) {
                ParadoxNotion.Services.Logger.LogError("Null Agent provided for event registration", "Events", this);
                return;
            }
            var router = targetAgent.GetComponent<MessageRouter>();
            if ( router == null ) {
                router = targetAgent.gameObject.AddComponent<MessageRouter>();
            }

            router.Register(this, eventNames);
        }

        ///Unsubscribe from a specific message to the target agent
        public void UnRegisterEvents(params string[] eventNames) { UnRegisterEvents(graphAgent, eventNames); }
        public void UnRegisterEvents(Component targetAgent, params string[] eventNames) {
            if ( targetAgent == null ) {
                return;
            }
            var router = targetAgent.GetComponent<MessageRouter>();
            if ( router != null ) {
                router.UnRegister(this, eventNames);
            }
        }

        ///Unsubscribe the node from all eventNames send to the target agent
        public void UnregisterAllEvents() { UnregisterAllEvents(graphAgent); }
        public void UnregisterAllEvents(Component targetAgent) {
            if ( targetAgent == null ) {
                return;
            }
            var router = targetAgent.GetComponent<MessageRouter>();
            if ( router != null ) {
                router.UnRegister(this);
            }
        }

        ///----------------------------------------------------------------------------------------------

        ///Returns whether source and target nodes can generaly be connected together.
        ///This only validates max in/out connections that source and target nodes has.
        ///Providing an existing refConnection, will bypass source/target validation respectively if that connection is already connected to that source/target node.
        public static bool IsNewConnectionAllowed(Node sourceNode, Node targetNode, Connection refConnection = null) {

            if ( sourceNode == null || targetNode == null ) {
                ParadoxNotion.Services.Logger.LogWarning("A Node Provided is null.", "Editor", targetNode);
                return false;
            }

            if ( sourceNode == targetNode ) {
                ParadoxNotion.Services.Logger.LogWarning("Node can't connect to itself.", "Editor", targetNode);
                return false;
            }

            if ( refConnection == null || refConnection.sourceNode != sourceNode ) {
                if ( sourceNode.outConnections.Count >= sourceNode.maxOutConnections && sourceNode.maxOutConnections != -1 ) {
                    ParadoxNotion.Services.Logger.LogWarning("Source node can have no more out connections.", "Editor", sourceNode);
                    return false;
                }
            }

            if ( refConnection == null || refConnection.targetNode != targetNode ) {
                if ( targetNode.maxInConnections <= targetNode.inConnections.Count && targetNode.maxInConnections != -1 ) {
                    ParadoxNotion.Services.Logger.LogWarning("Target node can have no more in connections.", "Editor", targetNode);
                    return false;
                }
            }

            var final = true;
            final &= sourceNode.CanConnectToTarget(targetNode);
            final &= targetNode.CanConnectFromSource(sourceNode);
            return final;
        }

        ///Override for explicit handling
        virtual protected bool CanConnectToTarget(Node targetNode) { return true; }
        ///Override for explicit handling
        virtual protected bool CanConnectFromSource(Node sourceNode) { return true; }

        ///Are provided nodes connected at all regardless of parent/child relation?
        public static bool AreNodesConnected(Node a, Node b) {
            var conditionA = a != null && a.outConnections.FirstOrDefault(c => c.targetNode == b) != null;
            var conditionB = b != null && b.outConnections.FirstOrDefault(c => c.targetNode == a) != null;
            return conditionA || conditionB;
        }

        ///----------------------------------------------------------------------------------------------

        ///Nodes can use coroutine as normal through MonoManager.
        protected Coroutine StartCoroutine(IEnumerator routine) {
            return MonoManager.current.StartCoroutine(routine);
        }

        ///Nodes can use coroutine as normal through MonoManager.
        protected void StopCoroutine(Coroutine routine) {
            MonoManager.current.StopCoroutine(routine);
        }


        ///Returns all *direct* parent nodes (first depth level)
        public Node[] GetParentNodes() {
            if ( inConnections.Count != 0 ) {
                return inConnections.Select(c => c.sourceNode).ToArray();
            }
            return new Node[0];
        }

        ///Returns all *direct* children nodes (first depth level)
        public Node[] GetChildNodes() {
            if ( outConnections.Count != 0 ) {
                return outConnections.Select(c => c.targetNode).ToArray();
            }
            return new Node[0];
        }

        ///Is node child of parent node?
        public bool IsChildOf(Node parentNode) {
            return inConnections.Any(c => c.sourceNode == parentNode);
        }

        ///Is node parent of child node?
        public bool IsParentOf(Node childNode) {
            return outConnections.Any(c => c.targetNode == childNode);
        }

        ///Override to define node functionality. The Agent and Blackboard used to start the Graph are propagated
        virtual protected Status OnExecute(Component agent, IBlackboard blackboard) { return status; }

        ///Called when the node gets reseted. e.g. OnGraphStart, after a tree traversal, when interrupted, OnGraphEnd etc...
        virtual protected void OnReset() { }

        ///Called when an input connection is connected
        virtual public void OnParentConnected(int connectionIndex) { }

        ///Called when an input connection is disconnected but before it actually does
        virtual public void OnParentDisconnected(int connectionIndex) { }

        ///Called when an output connection is connected
        virtual public void OnChildConnected(int connectionIndex) { }

        ///Called when an output connection is disconnected but before it actually does
        virtual public void OnChildDisconnected(int connectionIndex) { }

        ///Called when the parent graph is started. Use to init values or otherwise.
        virtual public void OnGraphStarted() { }

        ///Called when the parent graph is stopped.
        virtual public void OnGraphStoped() { }

        ///Called when the parent graph is paused.
        virtual public void OnGraphPaused() { }

        ///Called when the parent graph is unpaused.
        virtual public void OnGraphUnpaused() { }

        //...
        public override string ToString() {
            var result = name;
            if ( this is IReflectedWrapper ) {
                var info = ( this as IReflectedWrapper ).GetMemberInfo();
                if ( info != null ) { result = info.FriendlyName(); }
            }
            return string.Format("{0}{1}", result, ( !string.IsNullOrEmpty(tag) ? " (" + tag + ")" : "" ));
        }

        //...
        virtual public void OnDrawGizmos() {
            if ( this is ITaskAssignable && ( this as ITaskAssignable ).task != null ) {
                ( this as ITaskAssignable ).task.OnDrawGizmos();
            }
        }

        //...
        virtual public void OnDrawGizmosSelected() {
            if ( this is ITaskAssignable && ( this as ITaskAssignable ).task != null ) {
                ( this as ITaskAssignable ).task.OnDrawGizmosSelected();
            }
        }
    }
}