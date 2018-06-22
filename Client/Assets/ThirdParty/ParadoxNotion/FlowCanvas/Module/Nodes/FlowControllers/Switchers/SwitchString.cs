using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{
	
	[Category("Flow Controllers/Switchers")]
	[Description("Branch the Flow based on a string value. The Default output is called if there is no other matching output same as the input value")]
	[ContextDefinedInputs(typeof(string))]
	public class SwitchString : FlowControlNode {

		public List<string> comparisonOutputs = new List<string>();

		protected override void RegisterPorts(){
			var name = AddValueInput<string>("Value");
			var outs = new List<FlowOutput>();
			
			for (var i = 0; i < comparisonOutputs.Count; i++){
				outs.Add( AddFlowOutput( string.Format("\"{0}\"", comparisonOutputs[i]), i.ToString() ) );
			}

			var def = AddFlowOutput("Default");
			AddFlowInput("In", (f)=>
			{
				var value = name.value;
				if (value == null){
					def.Call(f);
					return;
				}

				var found = false;
				for (var i = 0; i < comparisonOutputs.Count; i++){

					if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(comparisonOutputs[i]) ){
						outs[i].Call(f);
						found = true;
						continue;
					}

					if (comparisonOutputs[i].Trim().ToLower() == value.Trim().ToLower() ){
						outs[i].Call(f);
						found = true;
					}
				}

				if (!found){
					def.Call(f);
				}
			});
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