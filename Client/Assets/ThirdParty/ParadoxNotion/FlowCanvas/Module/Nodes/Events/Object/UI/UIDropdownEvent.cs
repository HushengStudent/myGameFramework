using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace FlowCanvas.Nodes{

	[Name("UI Dropdown")]
	[Category("Events/Object/UI")]
	[Description("Called when the target UI Dropdown value changed.")]
	public class UIDropdownEvent : EventNode<UnityEngine.UI.Dropdown> {

		private FlowOutput o;
		private int value;

		public override void OnGraphStarted(){
			ResolveSelf();
			if (!target.isNull){
				target.value.onValueChanged.AddListener(OnValueChanged);
			}
		}
		public override void OnGraphStoped(){
			if (!target.isNull){
				target.value.onValueChanged.RemoveListener(OnValueChanged);
			}
		}

		protected override void RegisterPorts(){
			o = AddFlowOutput("Value Changed");
			AddValueOutput<UnityEngine.UI.Dropdown>("This", ()=>{ return base.target.value; });
			AddValueOutput<int>("Value", ()=>{ return value; });
		}

		void OnValueChanged(int value){
			this.value = value;
			o.Call(new Flow());
		}
	}
}