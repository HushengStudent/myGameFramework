using ParadoxNotion.Design;
using UnityEngine;
using System;
using ParadoxNotion;
using ParadoxNotion.Serialization;

namespace FlowCanvas.Nodes
{

    [Name("Delegate Callback", 1)]
	[Category("Events/Custom")]
	[Description("The exposed Delegate points directly to the 'Callback' output. You can connect this delegate as listener to a Unity or C# Event using the AddListener function of that Unity Event, or the += function of that C# Event. When that event is raised, this node will be called.")]
	[ContextDefinedOutputs(typeof(Flow), typeof(Delegate))]
	public class DelegateCallbackEvent : EventNode {
		
		[SerializeField]
		private SerializedTypeInfo _type;

		private Type delegateType{
			get {return _type != null? _type.Get() : null;}
			set
			{
				if (_type == null || _type.Get() != value){
					_type = new SerializedTypeInfo(value);
				}
			}
		}


		private ReflectedDelegateEvent reflectedEvent;
		private ValueOutput delegatePort;
		private FlowOutput callbackPort;
		private object[] args;

		protected override void RegisterPorts(){
			delegateType = delegateType != null? delegateType : typeof(Delegate);
			delegatePort = AddValueOutput(delegateType.FriendlyName(), "Delegate", delegateType, ()=>{ return reflectedEvent.AsDelegate(); });
			callbackPort = AddFlowOutput("Callback");
			if (delegateType == typeof(Delegate)){
				return;
			}

			if (reflectedEvent == null){
				reflectedEvent = new ReflectedDelegateEvent(delegateType);
				reflectedEvent.Add(Callback);
			}

			var parameters = delegateType.RTGetDelegateTypeParameters();
			for (var _i = 0; _i < parameters.Length; _i++){
				var i = _i;
				var parameter = parameters[i];
				AddValueOutput(parameter.Name, "arg" + i, parameter.ParameterType, ()=>{ return args[i]; });
			}
		}

		void Callback(params object[] args){
			this.args = args;
			callbackPort.Call(new Flow());
		}

		public override void OnPortConnected(Port port, Port otherPort){
			if (port == delegatePort && otherPort.type.RTIsSubclassOf(typeof(Delegate)) ){
				delegateType = otherPort.type;
				GatherPorts();
			}
		}
	}
}