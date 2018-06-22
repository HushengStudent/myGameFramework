using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace FlowCanvas.Macros
{

    [UnityEngine.CreateAssetMenu(menuName="ParadoxNotion/FlowCanvas/Macro Asset")]
	public class Macro : FlowScriptBase {

		///----------------------------------------------------------------------------------------------
		[System.Serializable]
		struct DerivedSerializationData{
			public List<DynamicPortDefinition> inputDefinitions;
			public List<DynamicPortDefinition> outputDefinitions;
		}

		public override object OnDerivedDataSerialization(){
			var data = new DerivedSerializationData();
			data.inputDefinitions = this.inputDefinitions;
			data.outputDefinitions = this.outputDefinitions;
			return data;
		}

		public override void OnDerivedDataDeserialization(object data){
			if (data is DerivedSerializationData){
				this.inputDefinitions = ((DerivedSerializationData)data).inputDefinitions;
				this.outputDefinitions = ((DerivedSerializationData)data).outputDefinitions;
			}
		}
		///----------------------------------------------------------------------------------------------

		///The list of input port definition of the macro
		[SerializeField]
		public List<DynamicPortDefinition> inputDefinitions = new List<DynamicPortDefinition>();
		///The list of output port definition of the macro
		[SerializeField]
		public List<DynamicPortDefinition> outputDefinitions = new List<DynamicPortDefinition>();

		[NonSerialized]
		public Dictionary<string, FlowHandler> entryActionMap = new Dictionary<string, FlowHandler>(StringComparer.Ordinal);
		[NonSerialized]
		public Dictionary<string, FlowHandler> exitActionMap  = new Dictionary<string, FlowHandler>(StringComparer.Ordinal);
		[NonSerialized]
		public Dictionary<string, ValueHandlerObject> entryFunctionMap = new Dictionary<string, ValueHandlerObject>(StringComparer.Ordinal);
		[NonSerialized]
		public Dictionary<string, ValueHandlerObject> exitFunctionMap  = new Dictionary<string, ValueHandlerObject>(StringComparer.Ordinal);

		private MacroInputNode _entry;
		private MacroOutputNode _exit;


		///Macros use local blackboard instead of propagated one
		public override bool useLocalBlackboard{
			get {return true;}
		}

		///The entry node of the Macro (input ports)
		public MacroInputNode entry{
			get
			{
				if (_entry == null){
					_entry = allNodes.OfType<MacroInputNode>().FirstOrDefault();
					if (_entry == null){
						_entry = AddNode<MacroInputNode>(new Vector2(-translation.x + 200, -translation.y + 200));
					}
				}
				return _entry;
			}
		}

		///The exit node of the Macro (output ports)
		public MacroOutputNode exit
		{
			get
			{
				if (_exit == null){
					_exit = allNodes.OfType<MacroOutputNode>().FirstOrDefault();
					if (_exit == null){
						_exit = AddNode<MacroOutputNode>(new Vector2(-translation.x + 600, -translation.y + 200));
					}
				}
				return _exit;
			}
		}

		///validates the entry and exit references
		protected override void OnGraphValidate(){
			base.OnGraphValidate();
			_entry = null;
			_exit = null;
			_entry = entry;
			_exit = exit;
			//create initial ports in case there are none in both entry and exit
			if (inputDefinitions.Count == 0 && outputDefinitions.Count == 0){
				var defIn = new DynamicPortDefinition("In", typeof(Flow));
				var defOut = new DynamicPortDefinition("Out", typeof(Flow));
				inputDefinitions.Add( defIn );
				outputDefinitions.Add( defOut );
				entry.GatherPorts();
				exit.GatherPorts();
				var source = entry.GetOutputPort( defIn.ID );
				var target = exit.GetInputPort( defOut.ID );
				BinderConnection.Create(source, target);
			}
		}

		///Adds a new input port definition to the Macro
		public bool AddInputDefinition(DynamicPortDefinition def){
			if (inputDefinitions.Find(d => d.ID == def.ID) == null){
				inputDefinitions.Add(def);
				return true;
			}
			return false;
		}

		///Adds a new output port definition to the Macro
		public bool AddOutputDefinition(DynamicPortDefinition def){
			if (outputDefinitions.Find(d => d.ID == def.ID) == null){
				outputDefinitions.Add(def);
				return true;
			}
			return false;
		}

		///----------------------------------------------------------------------------------------------
		///---------------------------------------UNITY EDITOR-------------------------------------------
		#if UNITY_EDITOR

		[UnityEditor.MenuItem("Tools/ParadoxNotion/FlowCanvas/Create/Macro Asset", false, 1)]
		public static void CreateMacro(){
			var macro = ParadoxNotion.Design.EditorUtils.CreateAsset<Macro>(true);
			UnityEditor.Selection.activeObject = macro;
		}

		#endif
		///----------------------------------------------------------------------------------------------


	}
}