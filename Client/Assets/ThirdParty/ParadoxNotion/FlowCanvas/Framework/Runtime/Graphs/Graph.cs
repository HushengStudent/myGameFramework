using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Services;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;

namespace NodeCanvas.Framework{

	///This is the base and main class of NodeCanvas and graphs. All graph System are deriving from this.
	[System.Serializable]
	abstract public partial class Graph : ScriptableObject, ITaskSystem, ISerializationCallbackReceiver {

		[SerializeField]
		private string _serializedGraph;
		[SerializeField]
		private List<Object> _objectReferences;
		[SerializeField]
		private bool _deserializationFailed = false;

		[System.NonSerialized]
		private bool hasDeserialized = false;

		//These are the data that are serialized and deserialized into/from a 'GraphSerializationData' object
		///----------------------------------------------------------------------------------------------
		private string _category                  = string.Empty;
		private string _comments                  = string.Empty;
		private Vector2 _translation              = new Vector2(-5000, -5000);
		private float _zoomFactor                 = 1f;
		private List<Node> _nodes                 = new List<Node>();
		private Node _primeNode                   = null;
		private List<CanvasGroup> _canvasGroups   = null;
		private BlackboardSource _localBlackboard = null;
		///----------------------------------------------------------------------------------------------

		///----------------------------------------------------------------------------------------------
		void ISerializationCallbackReceiver.OnBeforeSerialize(){ Serialize(); }
		void ISerializationCallbackReceiver.OnAfterDeserialize(){ Deserialize(); }
		///----------------------------------------------------------------------------------------------

		///----------------------------------------------------------------------------------------------
		protected void OnEnable(){ Validate(); }
		protected void OnDisable(){}
		protected void OnDestroy(){}
		protected void OnValidate(){}
		///----------------------------------------------------------------------------------------------

		///Serialize the Graph
		public void Serialize(){
			#if UNITY_EDITOR //we only serialize in the editor

			if (JSONSerializer.applicationPlaying){
				return;
			}

			if (_objectReferences != null && _objectReferences.Count > 0 && _objectReferences.Any(o => o != null)){
				hasDeserialized = false;
			}
			
			_serializedGraph = this.Serialize(false, _objectReferences);

			//notify owner. This is used for bound graphs
			var owner = agent != null && agent is GraphOwner? (GraphOwner)agent : null;
			if (owner != null && owner.graph == this){
				owner.OnAfterGraphSerialized(this);
			}
			#endif
		}

		///Deserialize the Graph
		public void Deserialize(){
			if (hasDeserialized && JSONSerializer.applicationPlaying){
				return;
			}
			hasDeserialized = true;
			this.Deserialize(_serializedGraph, false, _objectReferences);
		}

		///----------------------------------------------------------------------------------------------

		///Serialize the graph and returns the serialized json string
		public string Serialize(bool pretyJson, List<UnityEngine.Object> objectReferences){
			//if something went wrong on deserialization, dont serialize back, but rather keep what we had
			if (_deserializationFailed){
				_deserializationFailed = false;
				return _serializedGraph;
			}
			
			//the list to save the references in. If not provided explicitely we save into the local list
			if (objectReferences == null){
				objectReferences = this._objectReferences = new List<Object>();
			}

			UpdateNodeIDs(true);
			return JSONSerializer.Serialize(typeof(GraphSerializationData), new GraphSerializationData(this), pretyJson, objectReferences);
		}

		///Deserialize the json serialized graph provided. Returns the data or null if failed.
		public GraphSerializationData Deserialize(string serializedGraph, bool validate, List<UnityEngine.Object> objectReferences){
			if (string.IsNullOrEmpty(serializedGraph)){
				return null;
			}

			//the list to load the references from. If not provided explicitely we load from the local list (which is the case most times)
			if (objectReferences == null){
				objectReferences = this._objectReferences;
			}

			try
			{
				//deserialize provided serialized graph into a new GraphSerializationData object and load it
				var data = JSONSerializer.Deserialize<GraphSerializationData>(serializedGraph, objectReferences);
				if (LoadGraphData(data, validate) == true){
					this._deserializationFailed = false;
					this._serializedGraph = serializedGraph;
					this._objectReferences = objectReferences;
					return data;
				}

				_deserializationFailed = true;
				return null;
			}
			catch (System.Exception e)
			{
                Logger.LogException(e, "NodeCanvas", this);
				_deserializationFailed = true;
				return null;
			}
		}

		///Loads the GraphSerializationData
		bool LoadGraphData(GraphSerializationData data, bool validate){

			if (data == null){
                Logger.LogError("Can't Load graph, cause of null GraphSerializationData provided", "Serialization", this);
				return false;
			}

			if (data.type != this.GetType()){
                Logger.LogError("Can't Load graph, cause of different Graph type serialized and required", "Serialization", this);
				return false;
			}

			data.Reconstruct(this);

			//grab the final data and set fields directly
			this._category        = data.category;
			this._comments        = data.comments;
			this._translation     = data.translation;
			this._zoomFactor      = data.zoomFactor;
			this._nodes           = data.nodes;
			this._primeNode       = data.primeNode;
			this._canvasGroups    = data.canvasGroups;
			this._localBlackboard = data.localBlackboard;

			//IMPORTANT: Validate should be called in all deserialize cases outside of Unity's 'OnAfterDeserialize',
			//like for example when loading from json, or manualy calling this outside of OnAfterDeserialize.
			if (validate){
				Validate();
			}

			return true;
		}

		///Graph can override this for derived data serialization if needed
		virtual public object OnDerivedDataSerialization(){return null;}
		///Graph can override this for derived data deserialization if needed
		virtual public void OnDerivedDataDeserialization(object data){}

		///Gets the json string along with the list of UnityObject references used for serialization by this graph.
		public void GetSerializationData(out string json, out List<UnityEngine.Object> references){
			json = _serializedGraph;
			references = _objectReferences != null? new List<UnityEngine.Object>(_objectReferences) : null;
		}

		///Sets the target list of UnityObject references. This is for internal use and highly not recomended to do.
		public void SetSerializationObjectReferences(List<UnityEngine.Object> references){
			this._objectReferences = references;
		}

		///Clones the graph and returns the new one. Currently exactly the same as Instantiate, but could change in the future
		public static T Clone<T>(T graph) where T:Graph{
			var newGraph = (T)Instantiate(graph);
			newGraph.name = newGraph.name.Replace("(Clone)", "");
			return (T)newGraph;
		}

		///Serialize the local blackboard of the graph alone
		public string SerializeLocalBlackboard(){
			return JSONSerializer.Serialize(typeof(BlackboardSource), _localBlackboard, false, _objectReferences);
		}

		///Deserialize the local blackboard of the graph alone
		public bool DeserializeLocalBlackboard(string json){
			try
			{
				_localBlackboard = JSONSerializer.Deserialize<BlackboardSource>(json, _objectReferences);
				if (_localBlackboard == null) _localBlackboard = new BlackboardSource();
				return true;
			}
			catch (System.Exception e)
			{
                Logger.LogException(e, "Serialization", this);
				return false;
			}
		}
		
		///Non-editor CopySerialized
		public void CopySerialized(Graph target){
			var json = this.Serialize(false, target._objectReferences);
			target.Deserialize( json, true, this._objectReferences );
		}

		///Validate the graph and it's nodes. Also called from OnEnable callback.
		public void Validate(){

			#if UNITY_EDITOR
			if (!Application.isPlaying && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode){
				UpdateReferences();
			}
			#endif

			for (var i = 0; i < allNodes.Count; i++){
				try { allNodes[i].OnValidate(this); } //validation could be critical. we always continue
				catch (System.Exception e) { Logger.LogException(e, "Validation", allNodes[i]); continue; }
			}

			var allTasks = GetAllTasksOfType<Task>();
			for (var i = 0; i < allTasks.Count; i++){
				try { allTasks[i].OnValidate(this); } //validation could be critical. we always continue
				catch (System.Exception e) { Logger.LogException(e, "Validation", allTasks[i]); continue; }
			}
			
			OnGraphValidate();

			//in runtime and if graph uses local blackboard, initialize property/field binding.
			//'null' target gameObject (since localBlackboard can only have static bindings).
			//'false' so that setter is not called.
			if (Application.isPlaying && useLocalBlackboard){
				localBlackboard.InitializePropertiesBinding(null, false);
			}
		}

		///Use this for derived graph Validation
		virtual protected void OnGraphValidate(){}

		///----------------------------------------------------------------------------------------------

		///Raised when the graph is Stoped/Finished if it was Started at all.
		///Important: After the graph is stopped, the OnFinished event is cleared from all subscribers!
		public event System.Action<bool> onFinish;

		[System.NonSerialized]
		private Component _agent;
		[System.NonSerialized]
		private IBlackboard _blackboard;

		[System.NonSerialized]
		private static List<Graph> runningGraphs = new List<Graph>();
		[System.NonSerialized]
		private float timeStarted;
		[System.NonSerialized]
		private bool _isRunning;
		[System.NonSerialized]
		private bool _isPaused;

		///----------------------------------------------------------------------------------------------

		///The base type of all nodes that can live in this system
		abstract public System.Type baseNodeType{get;}
		///Is this system allowed to start with a null agent?
		abstract public bool requiresAgent{get;}
		///Does the system needs a prime Node to be set for it to start?
		abstract public bool requiresPrimeNode{get;}
		///Should the the nodes be auto sorted on position x?
		abstract public bool autoSort{get;}
		///Force use the local blackboard vs propagated one?
		abstract public bool useLocalBlackboard{get;}
		//Whether the graph can accept variables Drag&Drop
		abstract public bool canAcceptVariableDrops{get;}

		///----------------------------------------------------------------------------------------------

		///The friendly title name of the graph
		new public string name{
			get {return base.name;}
			set {base.name = value;}
		}

		///Graph category
		public string category{
			get {return _category;}
			set {_category = value;}
		}

		///Graph Comments
		public string comments{
			get {return _comments;}
			set {_comments = value;}
		}
		
		///The time in seconds this graph is running
		public float elapsedTime{
			get {return isRunning || isPaused? Time.time - timeStarted : 0;}
		}

		///Is the graph running?
		public bool isRunning {
			get {return _isRunning;}
			private set {_isRunning = value;}
		}

		///Is the graph paused?
		public bool isPaused {
			get {return _isPaused;}
			private set {_isPaused = value;}
		}

		///All nodes assigned to this system
		public List<Node> allNodes{
			get {return _nodes;}
			private set {_nodes = value;}
		}

		///The node to execute first. aka 'START'
		public Node primeNode{
			get {return _primeNode;}
			set
			{
				if (_primeNode != value){

					if (value != null && value.allowAsPrime == false){
						return;
					}
					
					if (isRunning){
						if (_primeNode != null)	_primeNode.Reset();
						if (value != null) value.Reset();
					}

					RecordUndo("Set Start");
					_primeNode = value;
					UpdateNodeIDs(true);
				}
			}
		}

		///The canvas groups of the graph (Editor purposes)
		public List<CanvasGroup> canvasGroups{
			get {return _canvasGroups;}
			set {_canvasGroups = value;}
		}

		///The translation of the graph in the total canvas (Editor purposes)
		public Vector2 translation{
			get {return _translation;}
			set {_translation = value;}
		}

		///The zoom of the graph (Editor purposes)
		public float zoomFactor{
			get {return _zoomFactor;}
			set {_zoomFactor = value;}
		}

		///The agent currently assigned to the graph
		public Component agent{
			get {return _agent;}
			set {_agent = value;}
		}

		///The blackboard currently assigned to the graph
		public IBlackboard blackboard{
			get
			{
				if (useLocalBlackboard){ return localBlackboard; }
				#if UNITY_EDITOR
				//done for when user removes bb component in editor.
				if (_blackboard == null || _blackboard.Equals(null)){
					return null;
				}
				#endif

				return _blackboard;
			}
			set
			{
				if (_blackboard != value){
					if (isRunning){ return; }
					if (useLocalBlackboard){ return; }
					_blackboard = value;
				}
			}
		}

		///The local blackboard of the graph
		public BlackboardSource localBlackboard{
			get
			{
				if (ReferenceEquals(_localBlackboard, null)){
					_localBlackboard = new BlackboardSource();
					_localBlackboard.name = "Local Blackboard";
				}
				return _localBlackboard;
			}
		}

		///The UnityObject of the ITaskSystem. In this case the graph itself
		UnityEngine.Object ITaskSystem.contextObject{
			get {return this;}
		}

		///----------------------------------------------------------------------------------------------

		///Makes a copy of provided nodes and if targetGraph is provided, pastes the new nodes in that graph.
		public static List<Node> CloneNodes(List<Node> originalNodes, Graph targetGraph = null, Vector2 originPosition = default(Vector2)){

			if (targetGraph != null){
				if (originalNodes.Any(n => n.GetType().IsSubclassOf(targetGraph.baseNodeType) == false )){
					return null;
				}
			}

			var newNodes = new List<Node>();
			var linkInfo = new Dictionary<Connection, KeyValuePair<int, int>>();

			//duplicate all nodes first
			foreach (var original in originalNodes){
				Node newNode = null;
				if (targetGraph != null){
					newNode = original.Duplicate(targetGraph);
					if (targetGraph != original.graph && original.graph != null && original == original.graph.primeNode){
						targetGraph.primeNode = newNode;
					}
				} else {
					newNode = JSONSerializer.Clone<Node>(original);
				}
				newNodes.Add(newNode);					

				//store the out connections that need dulpicate along with the indeces of source and target
				foreach (var c in original.outConnections){
					var sourceIndex = originalNodes.IndexOf(c.sourceNode);
					var targetIndex = originalNodes.IndexOf(c.targetNode);
					linkInfo[c] = new KeyValuePair<int, int>(sourceIndex, targetIndex);
				}
			}

			//duplicate all connections that we stored as 'need duplicating' providing new source and target
			foreach (var linkPair in linkInfo){
				if (linkPair.Value.Value != -1){ //we check this to see if the target node is part of the duplicated nodes since IndexOf returns -1 if element is not part of the list
					var newSource = newNodes[ linkPair.Value.Key ];
					var newTarget = newNodes[ linkPair.Value.Value ];
					if (targetGraph != null){
						linkPair.Key.Duplicate(newSource, newTarget);
					} else {
						var newConnection = JSONSerializer.Clone<Connection>(linkPair.Key);
						newConnection.SetSource(newSource, false);
						newConnection.SetTarget(newTarget, false);
					}
				}
			}

			if (originPosition != default(Vector2) && newNodes.Count > 0){
				if (newNodes.Count == 1){
					newNodes[0].position = originPosition;
				} else {
					var diff = newNodes[0].position - originPosition;
					newNodes[0].position = originPosition;
					for (var i = 1; i < newNodes.Count; i++){
						newNodes[i].position -= diff;
					}					
				}
			}

			if (targetGraph != null){
				//revalidate all new nodes in their new graph
				for (var i = 0; i < newNodes.Count; i++){
					newNodes[i].OnValidate(targetGraph);
				}
			}

			return newNodes;
		}

		///Updates the required references: Setting the system to the tasks and blackboard to BBParameters.
		///This is done when the graph starts and in the editor for convenience.
		public void UpdateReferences(){
			UpdateNodeBBFields();
			SendTaskOwnerDefaults();
		}

		//Update all graph node's BBFields for current Blackboard.
		void UpdateNodeBBFields(){
			for (var i = 0; i < allNodes.Count; i++){
				BBParameter.SetBBFields(allNodes[i], blackboard);
			}
		}

		//Sets all graph Tasks' owner (which is this).
		public void SendTaskOwnerDefaults(){
			var tasks = GetAllTasksOfType<Task>();
			for (var i = 0; i < tasks.Count; i++){
				tasks[i].SetOwnerSystem(this);
			}
		}

		///Update the IDs of the nodes in the graph. Is automatically called whenever a change happens in the graph by the adding removing connecting etc.
		public void UpdateNodeIDs(bool alsoReorderList){

			var lastID = -1;

			//start from prime
			if (primeNode != null){
				lastID = primeNode.AssignIDToGraph(lastID);
			}

			//set the rest starting from nodes without parent(s)
			var tempList = allNodes.OrderBy(n => n.inConnections.Count != 0).ToList();
			for (var i = 0; i < tempList.Count; i++){
				lastID = tempList[i].AssignIDToGraph(lastID);
			}

			//reset the check
			for (var i = 0; i < allNodes.Count; i++){
				allNodes[i].ResetRecursion();
			}

			if (alsoReorderList){
				allNodes = allNodes.OrderBy(node => node.ID).ToList();
			}
		}


		///Start the graph for the agent and blackboard provided.
		///Optionally provide a callback for when the graph stops or ends
		public void StartGraph(Component agent, IBlackboard blackboard, bool autoUpdate, System.Action<bool> callback = null){

			#if UNITY_EDITOR //prevent the user to accidentaly start the graph while its an asset. At least in the editor
			if (UnityEditor.EditorUtility.IsPersistent(this)){
                Logger.LogError("You have tried to start a graph which is an asset, not an instance! You should Instantiate the graph first.", "NodeCanvas", this);
				return;
			}
			#endif

			if (isRunning){
				if (callback != null){
					onFinish += callback;
				}
                Logger.LogWarning("Graph is already Active.", "NodeCanvas", this);
				return;
			}

			if (agent == null && requiresAgent){
                Logger.LogWarning("You've tried to start a graph with null Agent.", "NodeCanvas", this);
				return;
			}

			if (primeNode == null && requiresPrimeNode){
                Logger.LogWarning("You've tried to start graph without a 'Start' node.", "NodeCanvas", this);
				return;
			}
			
			if (blackboard == null){
				if (agent != null){
                    Logger.Log("Graph started without blackboard. Looking for blackboard on agent '" + agent.gameObject + "'...", "NodeCanvas", this);
					blackboard = agent.GetComponent(typeof(IBlackboard)) as IBlackboard;
				}
				if (blackboard == null){
                    Logger.LogWarning("Started with null Blackboard. Using Local Blackboard instead.", "NodeCanvas", this);
					blackboard = localBlackboard;
				}
			}

			this.agent = agent;
			this.blackboard = blackboard;
			UpdateReferences();

			if (callback != null){
				this.onFinish = callback;
			}

			isRunning = true;

			///place after OnGraphStarted?
			runningGraphs.Add(this);

			if (!isPaused){
				timeStarted = Time.time;
				OnGraphStarted();
			} else {
				OnGraphUnpaused();
			}
			///

			for (var i = 0; i < allNodes.Count; i++){
				if (!isPaused){
					allNodes[i].OnGraphStarted();
				} else {
					allNodes[i].OnGraphUnpaused();
				}
			}

			isPaused = false;

			if (autoUpdate){
				MonoManager.current.onUpdate += UpdateGraph;
				UpdateGraph();
			}
		}

		///Stops the graph completely and resets all nodes.
		public void Stop(bool success = true){

			if (!isRunning && !isPaused){
				return;
			}

			runningGraphs.Remove(this);
			MonoManager.current.onUpdate -= UpdateGraph;

			isRunning = false;
			isPaused = false;

			for (var i = 0; i < allNodes.Count; i++){
				allNodes[i].Reset(false);
				allNodes[i].OnGraphStoped();
			}

			OnGraphStoped();

			if (onFinish != null){
				onFinish(success);
				onFinish = null;
			}
		}

		///Pauses the graph from updating as well as notifying all nodes and tasks.
		public void Pause(){

			if (!isRunning){
				return;
			}

			runningGraphs.Remove(this);
			MonoManager.current.onUpdate -= UpdateGraph;
			
			isRunning = false;
			isPaused = true;

			for (var i = 0; i < allNodes.Count; i++){
				allNodes[i].OnGraphPaused();
			}
			
			OnGraphPaused();
		}

		///Updates the graph. Normaly this is updated by MonoManager since at StartGraph, this method is registered for updating.
		public void UpdateGraph(){
			// UnityEngine.Profiling.Profiler.BeginSample(string.Format("NC Graph ({0})", agent != null? agent.name : this.name) );
			if (isRunning){
				OnGraphUpdate();
			}
			// UnityEngine.Profiling.Profiler.EndSample();
		}

		///Override for graph specific stuff to run when the graph is started
		virtual protected void OnGraphStarted(){}
		///Override for graph specific per frame logic. Called every frame if the graph is running
		virtual protected void OnGraphUpdate(){}
		///Override for graph specific stuff to run when the graph is stoped
		virtual protected void OnGraphStoped(){}
		///Override this for when the graph is paused
		virtual protected void OnGraphPaused(){}
		///Override for graph stuff to run when the graph is unpause
		virtual protected void OnGraphUnpaused(){}


		///Sends an OnCustomEvent message to the tasks that needs them. Tasks subscribe to events using [EventReceiver] attribute.
		public void SendEvent(string name){SendEvent(new EventData(name));}
		public void SendEvent<T>(string name, T value){SendEvent(new EventData<T>(name, value));}
		public void SendEvent(EventData eventData){
			if (!isRunning || eventData == null || agent == null){
				return;
			}

			#if UNITY_EDITOR
			if (NodeCanvas.Editor.NCPrefs.logEvents){
				Logger.Log(string.Format("Event '{0}' Send to '{1}'", eventData.name, agent.gameObject.name), "Event", agent);
			}
			#endif

			var router = agent.GetComponent<MessageRouter>();
			if (router != null){
				router.Dispatch("OnCustomEvent", eventData);
				router.Dispatch(eventData.name, eventData.value);
			}
			//if router is null, it means that nothing has subscribed to any event, thus we dont care.
		}

		///Sends an event to all Running graphs
		public static void SendGlobalEvent(string name){ SendGlobalEvent(new EventData(name)); }
		public static void SendGlobalEvent<T>(string name, T value){ SendGlobalEvent(new EventData<T>(name, value)); }
		public static void SendGlobalEvent(EventData eventData){
			var send = new List<GameObject>();
			foreach(var graph in runningGraphs.ToArray()){ //ToArray because an event may result in a graph stopping and thus messing with the enumeration.
				if (graph.agent != null){
					if (!send.Contains(graph.agent.gameObject)){
						send.Add(graph.agent.gameObject);
						graph.SendEvent(eventData);
					}
				}
			}
		}

		///----------------------------------------------------------------------------------------------

		///Get a node by it's ID, null if not found
		public Node GetNodeWithID(int searchID){
			if (searchID < allNodes.Count && searchID >= 0){
				return allNodes.Find(n => n.ID == searchID);
			}
			return null;
		}

		///Get all nodes of a specific type
		public List<T> GetAllNodesOfType<T>() where T:Node{
			return allNodes.OfType<T>().ToList();
		}

		///Get a node by it's tag name
		public T GetNodeWithTag<T>(string tagName) where T:Node{
			foreach (var node in allNodes.OfType<T>()){
				if (node.tag == tagName){
					return node;
				}
			}
			return null;
		}

		///Get all nodes taged with such tag name
		public List<T> GetNodesWithTag<T>(string tagName) where T:Node{
			var nodes = new List<T>();
			foreach (var node in allNodes.OfType<T>()){
				if (node.tag == tagName){
					nodes.Add(node);
				}
			}
			return nodes;
		}

		///Get all taged nodes regardless tag name
		public List<T> GetAllTagedNodes<T>() where T:Node{
			var nodes = new List<T>();
			foreach (var node in allNodes.OfType<T>()){
				if (!string.IsNullOrEmpty(node.tag)){
					nodes.Add(node);
				}
			}
			return nodes;
		}

		///Get all nodes of the graph that have no incomming connections
		public List<Node> GetRootNodes(){
			return allNodes.Where(n => n.inConnections.Count == 0).ToList();
		}

		///Get all nodes of the graph that have no outgoing connections
		public List<Node> GetLeafNodes(){
			return allNodes.Where(n => n.outConnections.Count == 0).ToList();
		}

		///Get all Nested graphs of this graph
		public List<T> GetAllNestedGraphs<T>(bool recursive) where T:Graph{
			var graphs = new List<T>();
			foreach (var node in allNodes.OfType<IGraphAssignable>()){
				if (node.nestedGraph is T){
					if (!graphs.Contains((T)node.nestedGraph)){
						graphs.Add( (T)node.nestedGraph );
					}
					if (recursive){
						graphs.AddRange( node.nestedGraph.GetAllNestedGraphs<T>(recursive) );
					}
				}
			}
			return graphs;
		}

		///Get all runtime instanced Nested graphs of this graph and it's sub-graphs
		public List<Graph> GetAllInstancedNestedGraphs(){
			var instances = new List<Graph>();
			foreach (var node in allNodes.OfType<IGraphAssignable>()){
				var subInstances = node.GetInstances();
				instances.AddRange( subInstances );
				foreach(var subInstance in subInstances){
					instances.AddRange( subInstance.GetAllInstancedNestedGraphs() );
				}
			}
			return instances;
		}

		///Get ALL assigned node Tasks of type T, in the graph (including tasks assigned on Nodes and on Connections and within ActionList & ConditionList)
		public List<T> GetAllTasksOfType<T>() where T:Task{

			var tasks = new List<Task>();
			var resultTasks = new List<T>();
			for (var i = 0; i < allNodes.Count; i++){
				var node = allNodes[i];
				if (node is ITaskAssignable && (node as ITaskAssignable).task != null){
					tasks.Add( (node as ITaskAssignable).task );
				}
				
				if (node is ISubTasksContainer){
					tasks.AddRange( (node as ISubTasksContainer).GetSubTasks() );
				}

				for (var j = 0; j < node.outConnections.Count; j++){
					var c = node.outConnections[j];
					if (c is ITaskAssignable && (c as ITaskAssignable).task != null){
						tasks.Add( (c as ITaskAssignable).task );
					}
					if (c is ISubTasksContainer){
						tasks.AddRange( (c as ISubTasksContainer).GetSubTasks() );
					}
				}
			}

			for (var i = 0; i < tasks.Count; i++){
				var task = tasks[i];
				if (task is ActionList){
					resultTasks.AddRange( (task as ActionList).actions.OfType<T>());
				}
				if (task is ConditionList){
					resultTasks.AddRange( (task as ConditionList).conditions.OfType<T>());
				}
				if (task is T){
					resultTasks.Add((T)task);				
				}
			}

			return resultTasks;
		}

		///Get the parent node on which target task is assigned.
		///This is done this way, since Tasks have no dependency on where they are assigned.
		public Node GetTaskParent(Task task){
			if (task == null){
				return null;
			}

			for (var i = 0; i < allNodes.Count; i++){
				var node = allNodes[i];
				if (node is ITaskAssignable){
					var assignable = (ITaskAssignable)node;
					var nodeTask = assignable.task;
					if (nodeTask == task){
						return node;
					}

					if (nodeTask is ActionList){
						var listTasks = (nodeTask as ActionList).actions;
						for (var j = 0; j < listTasks.Count; j++){
							if (listTasks[j] == task){
								return node;
							}
						}
					}

					if (nodeTask is ConditionList){
						var listTasks = (nodeTask as ConditionList).conditions;
						for (var j = 0; j < listTasks.Count; j++){
							if (listTasks[j] == task){
								return node;
							}
						}
					}
				}
				
				if (node is ISubTasksContainer){
					var container = (ISubTasksContainer)node;
					var subTasks = container.GetSubTasks();
					for (var j = 0; j < subTasks.Length; j++){
						var subTask = subTasks[j];
						if (subTask == task){
							return node;
						}

						if (subTask is ActionList){
							var listTasks = (subTask as ActionList).actions;
							for (var k = 0; k < listTasks.Count; k++){
								if (listTasks[k] == task){
									return node;
								}
							}
						}

						if (subTask is ConditionList){
							var listTasks = (subTask as ConditionList).conditions;
							for (var k = 0; k < listTasks.Count; k++){
								if (listTasks[k] == task){
									return node;
								}
							}
						}
					}
				}
			}
			return null;
		}

		///Get a list of BBParameter names used by the target element object.
		public static List<string> GetUsedParameterNamesOfTarget(object target){

			var result = new List<string>();
			if (target == null){
				return result;
			}

			result.AddRange( BBParameter.GetObjectBBParameters(target).Select(p => p.name) );

			var task = target as Task;
			if (task != null){
				result.AddRange( BBParameter.GetObjectBBParameters(task).Select(p => p.name) );
				if (!string.IsNullOrEmpty(task.overrideAgentParameterName) ){
					result.Add(task.overrideAgentParameterName);
				}
			}

			var taskActionList = target as ActionList;
			if (taskActionList != null){
				for (var i = 0; i < taskActionList.actions.Count; i++){
					var t = taskActionList.actions[i];
					result.AddRange( BBParameter.GetObjectBBParameters(t).Select(p => p.name) );
					if (!string.IsNullOrEmpty(t.overrideAgentParameterName) ){
						result.Add(t.overrideAgentParameterName);
					}
				}
			}

			var taskConditionList = target as ConditionList;
			if (taskConditionList != null){
				for (var i = 0; i < taskConditionList.conditions.Count; i++){
					var t = taskConditionList.conditions[i];
					result.AddRange( BBParameter.GetObjectBBParameters(t).Select(p => p.name) );
					if (!string.IsNullOrEmpty(t.overrideAgentParameterName) ){
						result.Add(t.overrideAgentParameterName);
					}
				}
			}

			var subContainer = target as ISubTasksContainer;
			if (subContainer != null){
				var subTasks = subContainer.GetSubTasks();
				for (var i = 0; i < subTasks.Length; i++){
					result.AddRange(GetUsedParameterNamesOfTarget(subTasks[i]));
				}
			}

			var assignable = target as ITaskAssignable;
			if (assignable != null && assignable.task != null){
				result.AddRange(GetUsedParameterNamesOfTarget(assignable.task));
			}

			return result;
		}

		///Given an element returns the graph containing it.
		public static Graph GetElementGraph(object obj){
			if (obj == null){ return null; }
			if (obj is GraphOwner){ return (obj as GraphOwner).graph; }
			if (obj is Graph){ return (Graph)obj; }
			if (obj is Node){ return (obj as Node).graph; }
			if (obj is Connection){ return (obj as Connection).sourceNode.graph; }
			if (obj is Task){ return (obj as Task).ownerSystem as Graph; }
			return null;			
		}

		///Get all defined BBParameters in the graph. Defined means that the BBParameter is set to read or write to/from a Blackboard and is not "|NONE|".
		public BBParameter[] GetDefinedParameters(){
			var bbParams = new List<BBParameter>();
			var objects = new List<object>();
			objects.AddRange(GetAllTasksOfType<Task>().Cast<object>());
			objects.AddRange(allNodes.Cast<object>());

			for (var i = 0; i < objects.Count; i++){
				var o = objects[i];
				if (o is Task){
					var task = (Task)o;
					if (task.agentIsOverride && !string.IsNullOrEmpty(task.overrideAgentParameterName) ){
						bbParams.Add( typeof(Task).RTGetField("overrideAgent").GetValue(task) as BBParameter );
					}
				}

				foreach(BBParameter bbParam in BBParameter.GetObjectBBParameters(o)){
					if (bbParam != null && bbParam.useBlackboard && !bbParam.isNone){
						bbParams.Add(bbParam);
					}
				}
			}

			return bbParams.ToArray();
		}

		///Utility function to create all defined parameters of this graph as variables into the provided blackboard.
		public void PromoteDefinedParametersToVariables(IBlackboard bb){
			foreach (var bbParam in GetDefinedParameters()){
				bbParam.PromoteToVariable(bb);
			}
		}

		///----------------------------------------------------------------------------------------------

		///Add a new node to this graph
		public T AddNode<T>() where T : Node{
			return (T)AddNode(typeof(T));
		}

		public T AddNode<T>(Vector2 pos) where T : Node{
			return (T)AddNode(typeof(T), pos);
		}

		public Node AddNode(System.Type nodeType){
			return AddNode(nodeType, new Vector2(50,50));
		}

		///Add a new node to this graph
		public Node AddNode(System.Type nodeType, Vector2 pos){
			
			if (nodeType.IsGenericTypeDefinition){
				nodeType = nodeType.RTMakeGenericType(nodeType.GetFirstGenericParameterConstraintType());
			}

			if (!nodeType.RTIsSubclassOf(baseNodeType)){
                Logger.LogWarning(nodeType + " can't be added to " + this.GetType().FriendlyName() + " graph.", "NodeCanvas", this);
				return null;
			}

			var newNode = Node.Create(this, nodeType, pos);

			RecordUndo("New Node");

			allNodes.Add(newNode);

			if (primeNode == null){
				primeNode = newNode;
			}
		
			UpdateNodeIDs(false);
			return newNode;
		}

		///Disconnects and then removes a node from this graph
		public void RemoveNode(Node node, bool recordUndo = true, bool force = false){
 
			if (!force && node.GetType().RTIsDefined<ParadoxNotion.Design.ProtectedSingletonAttribute>(true)){
				if (allNodes.Where(n => n.GetType() == node.GetType()).ToArray().Length == 1){
					return;
				}
			}

			if (!allNodes.Contains(node)){
                Logger.LogWarning("Node is not part of this graph.", "NodeCanvas", this);
				return;
			}

			#if UNITY_EDITOR
			if (!Application.isPlaying){
				//auto reconnect parent & child of deleted node. Just a workflow convenience
				if (autoSort && node.inConnections.Count == 1 && node.outConnections.Count == 1){
					var relinkNode = node.outConnections[0].targetNode;
					if (relinkNode != node.inConnections[0].sourceNode){
						RemoveConnection(node.outConnections[0]);
						node.inConnections[0].SetTarget(relinkNode);
					}
				}
			}

			NodeCanvas.Editor.GraphEditorUtility.activeElement = null;
			#endif

			//callback
			node.OnDestroy();

			//disconnect parents
			foreach (var inConnection in node.inConnections.ToArray()){
				RemoveConnection(inConnection);
			}

			//disconnect children
			foreach (var outConnection in node.outConnections.ToArray()){
				RemoveConnection(outConnection);
			}

			if (recordUndo){
				RecordUndo("Delete Node");
			}

			allNodes.Remove(node);

			if (node == primeNode){
				primeNode = GetNodeWithID( primeNode.ID );
			}

			UpdateNodeIDs(false);
		}
		
		///Connect two nodes together to the next available port of the source node
		public Connection ConnectNodes(Node sourceNode, Node targetNode){
			return ConnectNodes(sourceNode, targetNode, sourceNode.outConnections.Count);
		}

		///Connect two nodes together to a specific port index of the source node
		public Connection ConnectNodes(Node sourceNode, Node targetNode, int indexToInsert){

			if (targetNode.IsNewConnectionAllowed(sourceNode) == false){
				return null;
			}

			RecordUndo("New Connection");

			var newConnection = Connection.Create(sourceNode, targetNode, indexToInsert);
			sourceNode.OnChildConnected(indexToInsert);
			targetNode.OnParentConnected(targetNode.inConnections.IndexOf(newConnection));

			UpdateNodeIDs(false);
			return newConnection;
		}

		///Removes a connection
		public void RemoveConnection(Connection connection, bool recordUndo = true){

			//for live editing
			if (Application.isPlaying){
				connection.Reset();
			}

			if (recordUndo){
				RecordUndo("Delete Connection");
			}

			//callbacks
			connection.OnDestroy();
			connection.sourceNode.OnChildDisconnected(connection.sourceNode.outConnections.IndexOf(connection));
			connection.targetNode.OnParentDisconnected(connection.targetNode.inConnections.IndexOf(connection));
			
			connection.sourceNode.outConnections.Remove(connection);
			connection.targetNode.inConnections.Remove(connection);

			#if UNITY_EDITOR
			NodeCanvas.Editor.GraphEditorUtility.activeElement = null;
			#endif

			UpdateNodeIDs(false);
		}

		//Helper function
		public void RecordUndo(string name){
			#if UNITY_EDITOR
			if (!Application.isPlaying){
				UnityEditor.Undo.RecordObject(this, name);
			}
			#endif
		}

		///Clears the whole graph
		public void ClearGraph(){
			canvasGroups = null;
			foreach (var node in allNodes.ToArray()){
				RemoveNode(node);
			}
		}
		
	}
}