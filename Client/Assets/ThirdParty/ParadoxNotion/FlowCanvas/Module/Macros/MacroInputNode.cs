using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Macros{

	[DoNotList]
	[Icon("MacroIn")]
	[Description("Defines the Input ports of the Macro.\nTo quickly create ports, you can also Drag&Drop a connection on top of this node!")]
	[ProtectedSingleton]
	public class MacroInputNode : FlowNode{

		public override ParadoxNotion.Alignment2x2 iconAlignment{
			get {return ParadoxNotion.Alignment2x2.Default;}
		}

		private Macro macro{
			get {return (Macro)graph;}
		}

		protected override void RegisterPorts(){
			for (var i = 0; i < macro.inputDefinitions.Count; i++){
				var def = macro.inputDefinitions[i];
				if (def.type == typeof(Flow)){
					macro.entryActionMap[def.ID] = AddFlowOutput(def.name, def.ID).Call;
				} else {
					AddValueOutput(def.name, def.type, ()=> { return macro.entryFunctionMap[def.ID](); }, def.ID );
				}				
			}
		}


		///----------------------------------------------------------------------------------------------
		///---------------------------------------UNITY EDITOR-------------------------------------------
		#if UNITY_EDITOR
		
		protected override UnityEditor.GenericMenu OnContextMenu(UnityEditor.GenericMenu menu){
			return null;
		}

		protected override UnityEditor.GenericMenu OnDragAndDropPortContextMenu(UnityEditor.GenericMenu menu, Port port){
			if (port is ValueInput || port is FlowInput){
				menu.AddItem(new GUIContent(string.Format("Promote to new Input '{0}'", port.name)), false, ()=>{
					var def = new DynamicPortDefinition(port.name, port.type);
					if (macro.AddInputDefinition(def)){
						GatherPorts();
						BinderConnection.Create(GetOutputPort(def.ID), port);
					}
				});
			}
			return menu;
		}		

		protected override void OnNodeInspectorGUI(){

			if (GUILayout.Button("Add Flow Input")){
				macro.AddInputDefinition(new DynamicPortDefinition("Flow Input", typeof(Flow)));
				GatherPorts();
			}

			if (GUILayout.Button("Add Value Input")){
				EditorUtils.ShowPreferedTypesSelectionMenu(typeof(object), (t)=>
				{
					macro.AddInputDefinition(new DynamicPortDefinition(string.Format("{0} Input", t.FriendlyName() ), t));
					GatherPorts();
				});
			}

			var options = new EditorUtils.ReorderableListOptions();
			options.allowRemove = true;
			EditorUtils.ReorderableList(macro.inputDefinitions, options, (i, picked)=>
			{
				var def = macro.inputDefinitions[i];
				GUILayout.BeginHorizontal();
				def.name = UnityEditor.EditorGUILayout.DelayedTextField(def.name, GUILayout.Width(150), GUILayout.ExpandWidth(true));
				GUI.enabled = def.type != typeof(Flow);
				EditorUtils.ButtonTypePopup("", def.type, (t)=>{ def.type = t; GatherPorts(); });
				GUI.enabled = true;
				GUILayout.EndHorizontal();
			});

			if (GUI.changed){
				GatherPorts();
			}
		}

		#endif
		
	}
}