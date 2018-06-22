using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;


namespace FlowCanvas.Nodes{

	[Name("Input Axis")]
	[Category("Events/Input")]
	[Description("You are free to define any Input Axis in this node.\nAxis can be set in 'Project Settings/Input'.\nCalls Out when either of the Axis defined is not zero")]
	public class InputCustomAxisEvent : EventNode, IUpdatable {

		public BBParameter<List<string>> axis = new List<string>(){"Horizontal", "Vertical"};
		private float[] axisValues;
		private bool calledLastFrame;

		private FlowOutput o;

		protected override void RegisterPorts(){
			o = AddFlowOutput("Out");
			axisValues = new float[axis.value.Count+1];
			for (int _i = 0; _i < axis.value.Count; _i++){
				var i = _i;
				if (!string.IsNullOrEmpty(axis.value[i])){
					AddValueOutput<float>(axis.value[i], ()=>{ return axisValues[i]; }, i.ToString() );
				}
			}
		}

		public void Update(){
			var list = axis.value;
			var isAnyNotZero = false;
			for (int i = 0; i < list.Count; i++){
				if (!string.IsNullOrEmpty(list[i])){
					var value = Input.GetAxis(list[i]);
					axisValues[i] = value;
					if (value != 0){
						isAnyNotZero = true;
					}
				}
			}

			if (isAnyNotZero){
				o.Call(new Flow());
				calledLastFrame = true;
			}

			if (!isAnyNotZero && calledLastFrame){
				o.Call(new Flow());
				calledLastFrame = false;
			}
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		protected override void OnNodeInspectorGUI(){
			if (GUILayout.Button("Refresh")){
				GatherPorts();
			}
			base.OnNodeInspectorGUI();
		}

		#endif
		
	}
}