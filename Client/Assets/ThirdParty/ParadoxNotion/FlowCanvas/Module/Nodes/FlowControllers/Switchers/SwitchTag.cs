using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Category("Flow Controllers/Switchers")]
	[Description("Branch the Flow based on the tag of a GameObject value")]
	[ContextDefinedInputs(typeof(GameObject))]
	public class SwitchTag : FlowControlNode {

		//serialized since tags are fetched in editor
		[SerializeField]
		private string[] _tagNames = null;

		protected override void RegisterPorts(){

			#if UNITY_EDITOR
			_tagNames = UnityEditorInternal.InternalEditorUtility.tags;
			#endif

			var go = AddValueInput<GameObject>("Value");
			var outs = new List<FlowOutput>();
			
			for (var i = 0; i < _tagNames.Length; i++){
				outs.Add( AddFlowOutput(_tagNames[i], i.ToString() ) );
			}

			AddFlowInput("In", (f)=>
			{
				for (var i = 0; i < _tagNames.Length; i++){
					if (_tagNames[i] == go.value.tag){
						outs[i].Call(f);
					}
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