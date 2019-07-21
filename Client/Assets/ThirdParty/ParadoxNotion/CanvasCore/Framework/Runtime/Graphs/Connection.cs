using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;
using UnityEngine;


namespace NodeCanvas.Framework
{

#if UNITY_EDITOR //handles missing types
    [fsObject(Processor = typeof(fsRecoveryProcessor<Connection, MissingConnection>))]
#endif

    ///Base class for connections between nodes in a graph
    [ParadoxNotion.Design.SpoofAOT]
    abstract public partial class Connection : IGraphElement
    {

        [SerializeField]
        private Node _sourceNode;
        [SerializeField]
        private Node _targetNode;
        [SerializeField]
        private bool _isDisabled;

        [System.NonSerialized]
        private Status _status = Status.Resting;

        ///The source node of the connection
        public Node sourceNode {
            get { return _sourceNode; }
            protected set { _sourceNode = value; }
        }

        ///The target node of the connection
        public Node targetNode {
            get { return _targetNode; }
            protected set { _targetNode = value; }
        }

        ///Is the connection active?
        public bool isActive {
            get { return !_isDisabled; }
            set
            {
                if ( !_isDisabled && value == false ) {
                    Reset();
                }
                _isDisabled = !value;
            }
        }

        ///The connection status
        public Status status {
            get { return _status; }
            set { _status = value; }
        }

        ///The graph this connection belongs to taken from the source node.
        public Graph graph {
            get { return sourceNode != null ? sourceNode.graph : null; }
        }

        ///----------------------------------------------------------------------------------------------

        //required
        public Connection() { }


        ///Create a new Connection. Use this for constructor
        public static Connection Create(Node source, Node target, int sourceIndex = -1, int targetIndex = -1) {

            if ( source == null || target == null ) {
                Debug.LogError("Can't Create a Connection without providing Source and Target Nodes");
                return null;
            }

            if ( source is MissingNode ) {
                Debug.LogError("Creating new Connections from a 'MissingNode' is not allowed. Please resolve the MissingNode node first");
                return null;
            }

            var newConnection = (Connection)System.Activator.CreateInstance(source.outConnectionType);

            if ( source.graph != null ) {
                source.graph.RecordUndo("Create Connection");
            }

            var resultSourceIndex = newConnection.SetSourceNode(source, sourceIndex);
            var resultTargetIndex = newConnection.SetTargetNode(target, targetIndex);

            newConnection.OnValidate(resultSourceIndex, resultTargetIndex);
            newConnection.OnCreate(resultSourceIndex, resultTargetIndex);
            return newConnection;
        }

        ///Duplicate the connection providing a new source and target
        public Connection Duplicate(Node newSource, Node newTarget) {

            if ( newSource == null || newTarget == null ) {
                Debug.LogError("Can't Duplicate a Connection without providing NewSource and NewTarget Nodes");
                return null;
            }

            //deep clone
            var newConnection = JSONSerializer.Clone<Connection>(this);

            if ( newSource.graph != null ) {
                newSource.graph.RecordUndo("Duplicate Connection");
            }

            var resultSourceIndex = newConnection.SetSourceNode(newSource);
            var resultTargetIndex = newConnection.SetTargetNode(newTarget);

            var assignable = this as ITaskAssignable;
            if ( assignable != null && assignable.task != null ) {
                ( newConnection as ITaskAssignable ).task = assignable.task.Duplicate(newSource.graph);
            }

            newConnection.OnValidate(resultSourceIndex, resultTargetIndex);
            return newConnection;
        }

        ///Called once when the connection is created.
        virtual public void OnCreate(int sourceIndex, int targetIndex) { }
        ///Called when the Connection is created, duplicated or otherwise needs validation.
        virtual public void OnValidate(int sourceIndex, int targetIndex) { }
        ///Called when the connection is destroyed (always through graph.RemoveConnection or when a node is removed through graph.RemoveNode)
        virtual public void OnDestroy() { }

        ///Sets the source node of the connection
        public int SetSourceNode(Node newSource, int index = -1) {

            if ( sourceNode == newSource ) {
                return -1;
            }

            if ( graph != null ) {
                graph.RecordUndo("Set Source");
            }

            //relink
            if ( sourceNode != null && sourceNode.outConnections.Contains(this) ) {
                var i = sourceNode.outConnections.IndexOf(this);
                sourceNode.OnChildDisconnected(i);
                sourceNode.outConnections.Remove(this);
            }

            index = index == -1 ? newSource.outConnections.Count : index;
            newSource.outConnections.Insert(index, this);
            newSource.OnChildConnected(index);
            sourceNode = newSource;

#if UNITY_EDITOR
            if ( sourceNode != null && targetNode != null ) {
                targetNode.TrySortConnectionsByPositionX();
            }
#endif

            return index;
        }

        ///Sets the target node of the connection
        public int SetTargetNode(Node newTarget, int index = -1) {

            if ( targetNode == newTarget ) {
                return -1;
            }

            if ( graph != null ) {
                graph.RecordUndo("Set Target");
            }

            //relink
            if ( targetNode != null && targetNode.inConnections.Contains(this) ) {
                var i = targetNode.inConnections.IndexOf(this);
                targetNode.OnParentDisconnected(i);
                targetNode.inConnections.Remove(this);
            }

            index = index == -1 ? newTarget.inConnections.Count : index;
            newTarget.inConnections.Insert(index, this);
            newTarget.OnParentConnected(index);
            targetNode = newTarget;

#if UNITY_EDITOR
            if ( sourceNode != null && targetNode != null ) {
                targetNode.TrySortConnectionsByPositionX();
            }
#endif

            return index;
        }

        //...
        sealed public override string ToString() {
            return this.GetType().FriendlyName();
        }

        ///----------------------------------------------------------------------------------------------

        ///Execute the conneciton for the specified agent and blackboard.
        public Status Execute(Component agent, IBlackboard blackboard) {

            if ( !isActive ) {
                return Status.Optional;
            }

            status = targetNode.Execute(agent, blackboard);
            return status;
        }

        ///Resets the connection and its targetNode, optionaly recursively
        public void Reset(bool recursively = true) {

            if ( status == Status.Resting ) {
                return;
            }

            status = Status.Resting;

            if ( recursively ) {
                targetNode.Reset(recursively);
            }
        }
    }
}