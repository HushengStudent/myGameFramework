#define DO_EDITOR_BINDING //comment this out to test the real performance without editor binding specifics

using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;
using UnityEngine;
using NodeCanvas;


namespace FlowCanvas{

	public class BinderConnection : Connection {

		[SerializeField] [ParadoxNotion.Serialization.FullSerializer.fsProperty("_sourcePortName")]
		private string _sourcePortID;
		[SerializeField] [ParadoxNotion.Serialization.FullSerializer.fsProperty("_targetPortName")]
		private string _targetPortID;

		[System.NonSerialized]
		private Port _sourcePort;
		[System.NonSerialized]
		private Port _targetPort;

		///The source port ID name this binder is connected to
		public string sourcePortID{
			get {return sourcePort != null? sourcePort.ID : _sourcePortID;}
			private set {_sourcePortID = value;}
		}

		///The target port ID name this binder is connected to
		public string targetPortID{
			get {return targetPort != null? targetPort.ID : _targetPortID;}
			private set {_targetPortID = value;}
		}

		///The source Port
		public Port sourcePort{
			get
			{
				if (_sourcePort == null){
					if (sourceNode is FlowNode){ //In case it's 'MissingNode'
						_sourcePort = (sourceNode as FlowNode).GetOutputPort(_sourcePortID);
					}
				}
				return _sourcePort;
			}
		}

		///The target Port
		public Port targetPort{
			get
			{
				if (_targetPort == null){
					if (targetNode is FlowNode){ //In case it's 'MissingNode'
						_targetPort = (targetNode as FlowNode).GetInputPort(_targetPortID);
					}
				}
				return _targetPort;
			}
		}

		///The binder type. In case of Value connection, BinderConnection<T> is used, else it's basicaly a Flow binding
		public System.Type bindingType{
			get { return GetType().RTIsGenericType()? GetType().RTGetGenericArguments()[0] : typeof(Flow); }
		}

		//Called after the node has GatherPorts to gather the references and validate the binding connection
		public void GatherAndValidateSourcePort(){
			_sourcePort = null;
			if (sourcePort != null && TypeConverter.HasConvertion(sourcePort.type, bindingType)){
				sourcePortID = sourcePort.ID;
				sourcePort.connections ++;
				return;
			}

			graph.RemoveConnection(this, false);
		}

		//Called after the node has GatherPorts to gather the references and validate the binding connection
		public void GatherAndValidateTargetPort(){
			_targetPort = null;
			if (targetPort != null){

				if (targetPort.type == bindingType){
					targetPortID = targetPort.ID;
					targetPort.connections ++;
					return;
				}

				//replace binder connection type if possible
				if (targetPort is ValueInput && sourcePort is ValueOutput){
					if ( TypeConverter.HasConvertion(sourcePort.type, targetPort.type) ){
						ReplaceWith(typeof(BinderConnection<>).MakeGenericType(bindingType));
						targetPortID = targetPort.ID;
						targetPort.connections ++;
						return;
					}
				}
			}
			graph.RemoveConnection(this, false);
		}

		///Create a NEW binding connection object between two ports
		public static BinderConnection Create(Port source, Port target){

			if (source == null || target == null){
				return null;
			}

			if (source == target){
				return null;
			}

			if (!source.CanAcceptConnections()){
                ParadoxNotion.Services.Logger.LogWarning("Source port can accept no more connections.", "Editor", source.parent);
				return null;
			}

			if (!target.CanAcceptConnections()){
                ParadoxNotion.Services.Logger.LogWarning("Target port can accept no more connections.", "Editor", source.parent);
				return null;				
			}

			if (source.parent == target.parent){
                ParadoxNotion.Services.Logger.LogWarning("Can't connect ports on the same parent node.", "Editor", source.parent);
				return null;
			}

			if (source is FlowOutput && !(target is FlowInput) ){
                ParadoxNotion.Services.Logger.LogWarning("Flow ports can only be connected to other Flow ports.", "Editor", source.parent);
				return null;
			}

			if ( (source is FlowInput && target is FlowInput) || (source is ValueInput && target is ValueInput) ){
                ParadoxNotion.Services.Logger.LogWarning("Can't connect input to input.", "Editor", source.parent);
				return null;
			}

			if ( (source is FlowOutput && target is FlowOutput) || (source is ValueOutput && target is ValueOutput) ){
                ParadoxNotion.Services.Logger.LogWarning("Can't connect output to output.", "Editor", source.parent);
				return null;
			}

			if (!TypeConverter.HasConvertion(source.type, target.type)){
                ParadoxNotion.Services.Logger.LogWarning(string.Format("Can't connect ports. Type '{0}' is not assignable from Type '{1}' and there exists no automatic conversion for those types.", target.type.FriendlyName(), source.type.FriendlyName()), "Editor", source.parent);
				return null;
			}
			
			source.parent.graph.RecordUndo("Connect Ports");

			BinderConnection binder = null;
			if (source is FlowOutput && target is FlowInput){
				binder = new BinderConnection();
			}

			if (source is ValueOutput && target is ValueInput){
				binder = (BinderConnection)System.Activator.CreateInstance( typeof(BinderConnection<>).RTMakeGenericType(new System.Type[]{target.type}));
			}

			if (binder != null){
				binder.OnCreate(source, target);
			}
			return binder;
		}

		///Called in runtime intialize to actualy bind the delegates
		virtual public void Bind(){
			
			if (!isActive){
				return;
			}

			if (sourcePort is FlowOutput && targetPort is FlowInput){
				(sourcePort as FlowOutput).BindTo( (FlowInput)targetPort );

				#if UNITY_EDITOR && DO_EDITOR_BINDING
				(sourcePort as FlowOutput).Append( BlinkStatus );
				#endif
			}
		}


		///UnBinds the delegates
		virtual public void UnBind(){
			if (sourcePort is FlowOutput){
				(sourcePort as FlowOutput).UnBind();
			}
		}

		///Initialize some references when a connection has been made
		void OnCreate(Port source, Port target){
			sourceNode = source.parent;
			targetNode = target.parent;
			sourcePortID = source.ID;
			targetPortID = target.ID;
			sourceNode.outConnections.Add(this);
			targetNode.inConnections.Add(this);
			
			sourceNode.OnChildConnected(sourceNode.outConnections.Count-1);
			targetNode.OnParentConnected(targetNode.inConnections.Count-1);

			source.connections ++;
			target.connections ++;

			//for live editing
			if (Application.isPlaying){
				Bind();
			}
		}

		//callback from base class. The connection reference is already removed from target and source Nodes
		public override void OnDestroy(){
			if (sourcePort != null){ //check null for cases like the SwitchInt, where basicaly the source port is null since port is removed first
				sourcePort.connections --;
			}
			if (targetPort != null){
				targetPort.connections --;
			}

			//for live editing
			if (Application.isPlaying){
				UnBind();
			}
		}

		///Replace typeof of connection
		public BinderConnection ReplaceWith(System.Type t){
			var source = sourcePort;
			var target = targetPort;
			graph.RemoveConnection(this);
			return Create(source, target);
		}


		///----------------------------------------------------------------------------------------------
		///---------------------------------------UNITY EDITOR-------------------------------------------
		#if UNITY_EDITOR
		
		private int blinks;
		private int lastBlinkFrame;
		
		protected override TipConnectionStyle tipConnectionStyle{ get {return TipConnectionStyle.None;} }
		protected override bool canRelink{ get {return false;} }

		//TODO: Better implement blinking in base class
		protected void BlinkStatus(Flow f = default(Flow)){
			if (Application.isPlaying){
				if (lastBlinkFrame != Time.frameCount){
					lastBlinkFrame = Time.frameCount;
					blinks ++;
					if (blinks <= 1){
						MonoManager.current.StartCoroutine(Internal_BlinkStatus(f));
					}
				}
			}
		}

		IEnumerator Internal_BlinkStatus(Flow f){
			status = Status.Running;
			while (blinks > 0){
				yield return null;
				blinks --;
			}
			status = Status.Resting;
		}

		#endif
		///----------------------------------------------------------------------------------------------

	}
}