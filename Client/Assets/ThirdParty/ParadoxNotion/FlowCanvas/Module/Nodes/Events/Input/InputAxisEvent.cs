using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes{

	[Name("Input Axis (Preset)")]
	[Category("Events/Input")]
	[Description("Calls out when Horizontal or Vertical Input Axis is not zero")]
	[System.Obsolete("Use Input Axis")]
	public class InputAxisEvent : EventNode, IUpdatable {

		private FlowOutput o;
		private float horizontal;
		private float vertical;
		private bool calledLastFrame;

		protected override void RegisterPorts(){
			o = AddFlowOutput("Out");
			AddValueOutput<float>("Horizontal", ()=>{ return horizontal; });
			AddValueOutput<float>("Vertical", ()=>{ return vertical; });
		}

		public void Update(){

			horizontal = Input.GetAxis("Horizontal");
			vertical = Input.GetAxis("Vertical");
		
			if (horizontal != 0 || vertical != 0){
				o.Call(new Flow());
				calledLastFrame = true;
			}

			//this is done to also call actual 0
			if (horizontal == 0 && vertical == 0 && calledLastFrame){
				o.Call(new Flow());
				calledLastFrame = false;
			}
		}
	}
}