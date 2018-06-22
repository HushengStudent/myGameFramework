using System;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [Name("C# Event Callback", 2)]
	[Category("Events/Custom")]
	[Description("Providing a C# Event, Register a callback to be called when that event is raised.")]
	[ContextDefinedInputs(typeof(SharpEvent))]
	public class CSharpEventCallback : EventNode {

		[SerializeField] [ExposeField] [Tooltip("If enabled, registration will be handled on graph Enable/Disable automatically")]
		[GatherPortsCallback]
		private bool _autoHandleRegistration;

		[SerializeField]
		private SerializedTypeInfo _type;

		private object[] argValues;
		private ValueInput eventInput;
		private FlowOutput flowCallback;
		private ReflectedDelegateEvent reflectedEvent;

		private Type type{
			get {return _type != null? _type.Get() : null;}
			set
			{
				if (_type == null || _type.Get() != value){
					_type = new SerializedTypeInfo(value);
				}
			}
		}

		private bool autoHandleRegistration{
			get {return _autoHandleRegistration;}
		}

		public override void OnGraphStarted(){
			if (autoHandleRegistration){
				Register();
			}
		}

		public override void OnGraphStoped(){
			if (autoHandleRegistration){
				Unregister();
			}
		}

		protected override void RegisterPorts(){
			type = type != null? type : typeof(SharpEvent);
			eventInput = AddValueInput("Event", type);
			if (type == typeof(SharpEvent)){
				return;
			}

			var delegateType = type.RTGetGenericArguments()[0];
			if (reflectedEvent == null){
				reflectedEvent = new ReflectedDelegateEvent(delegateType);
			}
			var parameters = delegateType.RTGetDelegateTypeParameters();
			for (var _i = 0; _i < parameters.Length; _i++){
				var i = _i;
				var parameter = parameters[i];
				AddValueOutput(parameter.Name, "arg" + i, parameter.ParameterType, ()=>{ return argValues[i]; });
			}

			flowCallback = AddFlowOutput("Callback");
			if (!autoHandleRegistration){
				AddFlowInput("Register", Register, "Add");
				AddFlowInput("Unregister", Unregister, "Remove");
			}
		}

		void Register(Flow f = default(Flow)){
			var sharpEvent = eventInput.value as SharpEvent;
			if (sharpEvent != null){
				sharpEvent.StopListening(reflectedEvent, Callback);
				sharpEvent.StartListening(reflectedEvent, Callback);
			}
		}

		void Unregister(Flow f = default(Flow)){
			var sharpEvent = eventInput.value as SharpEvent;
			if (sharpEvent != null){
				sharpEvent.StopListening(reflectedEvent, Callback);
			}
		}

		void Callback(params object[] args){
			this.argValues = args;
			flowCallback.Call(new Flow());
		}

		public override Type GetNodeWildDefinitionType(){
			return typeof(SharpEvent);
		}

		public override void OnPortConnected(Port port, Port otherPort){
			if (port == eventInput){
				type = otherPort.type;
				GatherPorts();
			}
		}
	}
}