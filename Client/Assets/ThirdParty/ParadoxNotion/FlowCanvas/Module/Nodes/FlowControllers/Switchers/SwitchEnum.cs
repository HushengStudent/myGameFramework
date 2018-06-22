using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using ParadoxNotion.Serialization;

namespace FlowCanvas.Nodes{

	[Category("Flow Controllers/Switchers")]
	[Description("Branch the Flow based on an enum value.\nPlease connect an Enum first for the options to show.")]
	[ContextDefinedInputs(typeof(System.Enum))]
	public class SwitchEnum : FlowControlNode {

		[SerializeField]
		private SerializedTypeInfo _type;

		private System.Type type{
			get {return _type != null? _type.Get() : null;}
			set
			{
				if (_type == null || _type.Get() != value){
					_type = new SerializedTypeInfo(value);
				}
			}
		}

		protected override void RegisterPorts(){
			if (type == null){
				type = typeof(System.Enum);
			}

			var e = AddValueInput(type.Name, type, "Enum");
			if (type != typeof(System.Enum)){
				var outs = new List<FlowOutput>();
				foreach (var s in System.Enum.GetNames(type)){
					outs.Add( AddFlowOutput(s) );
				}
				AddFlowInput("In", (f)=>
				{
					var index = (int)System.Enum.Parse(e.value.GetType(), e.value.ToString());
					outs[index].Call(f);
				});
			}
		}

		public override System.Type GetNodeWildDefinitionType(){
			return typeof(System.Enum);
		}

		public override void OnPortConnected(Port port, Port otherPort){
			if (type == typeof(System.Enum)){
				if (typeof(System.Enum).RTIsAssignableFrom(otherPort.type) ){
					type = otherPort.type;
					GatherPorts();
				}
			}
		}
	}
}