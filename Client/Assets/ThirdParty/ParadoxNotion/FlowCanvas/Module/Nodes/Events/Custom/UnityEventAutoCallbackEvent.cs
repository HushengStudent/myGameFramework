using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using System.Reflection;
using UnityEngine.Events;
using ParadoxNotion.Serialization;

namespace FlowCanvas.Nodes
{

    [DoNotList]
	[Name("Unity Event")]
	[Description("Automatically Subscribes to the target UnityEvent when the graph is enabled, and is called when the event is raised")]
	public class UnityEventAutoCallbackEvent : EventNode{

		[SerializeField]
		private SerializedFieldInfo _field;

		private ReflectedUnityEvent reflectedEvent;
		private UnityEventBase unityEvent;
		private ValueInput instancePort;
		private FlowOutput callback;
		private object[] args;

		private FieldInfo field{
			get {return _field != null? _field.Get() : null;}
		}

		public override string name{
			get
			{
				if (field != null && field.IsStatic){
					return string.Format("{0} ({1})", base.name, field.RTReflectedType().FriendlyName());
				}
				return base.name;
			}
		}

		public void SetEvent(FieldInfo field, object instance = null){
			_field = new SerializedFieldInfo(field);
			GatherPorts();
		}

		protected override void RegisterPorts(){
			if (field == null){
				return;
			}

			if (reflectedEvent == null){
				reflectedEvent = new ReflectedUnityEvent(field.FieldType);
			}

			if (!field.IsStatic){
				instancePort = AddValueInput(field.RTReflectedType().FriendlyName(), field.RTReflectedType(), "Instance");
			}

			args = new object[reflectedEvent.parameters.Length];
			for (var _i = 0; _i < reflectedEvent.parameters.Length; _i++){
				var i = _i;
				var parameter = reflectedEvent.parameters[i];
				AddValueOutput(parameter.Name, "arg" + i, parameter.ParameterType, ()=>{ return args[i]; });
			}

			callback = AddFlowOutput(field.Name, "Event");
		}

		public override void OnGraphStarted(){
			
			if (field == null){
				return;
			}

			object instance = null;
			if (!field.IsStatic){
				instance = instancePort.value;
				if (instance == null){
					Fail("Target is null");
					return;
				}
			}

			unityEvent = (UnityEventBase)field.GetValue(instance);
			if (unityEvent != null){
				reflectedEvent.StartListening(unityEvent, OnEventRaised);
			}
		}

		public override void OnGraphStoped(){
			if (unityEvent != null){
				reflectedEvent.StopListening(unityEvent, OnEventRaised);
			}
		}

		void OnEventRaised(params object[] args){
			this.args = args;
			callback.Call(new Flow());
		}

	}
}