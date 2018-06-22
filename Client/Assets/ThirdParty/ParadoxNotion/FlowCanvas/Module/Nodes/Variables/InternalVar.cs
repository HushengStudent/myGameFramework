using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Name("Set Internal Var")]
	[Description("Can be used to set an internal variable, to later be retrieved with a 'Get Internal Var' node.")]
	[Category("Variables/Internal")]
	[ContextDefinedInputs(typeof(Wild))]
	[ExposeAsDefinition]
	abstract public class RelayValueInputBase : FlowNode{
		abstract public System.Type relayType{get;}
	}

	///Relay Input
	public class RelayValueInput<T> : RelayValueInputBase, IEditorMenuCallbackReceiver {

		[Tooltip("The identifier name of the internal var")] [DelayedField]
		public string identifier = "MyInternalVarName";
		[HideInInspector]
		public ValueInput<T> port{get; private set;}

		public override System.Type relayType{
			get {return typeof(T);}
		}

		public override string name{
			get {return string.Format("@ {0}", identifier);}
		}

		protected override void RegisterPorts(){
			port = AddValueInput<T>("Value");
		}		

		///----------------------------------------------------------------------------------------------
		///---------------------------------------UNITY EDITOR-------------------------------------------
		#if UNITY_EDITOR
		void IEditorMenuCallbackReceiver.OnMenu(UnityEditor.GenericMenu menu, Vector2 pos, Port sourcePort, object dropInstance){
			if ( sourcePort == null || sourcePort.type.IsAssignableFrom(this.relayType) ){
				menu.AddItem(new GUIContent(string.Format("Variables/Internal/Get: '{0}'", identifier)), false, ()=>{ flowGraph.AddFlowNode<RelayValueOutput<T>>(pos, sourcePort, dropInstance).SetSource(this); });
			}
		}
		#endif
		///----------------------------------------------------------------------------------------------

	}


	///----------------------------------------------------------------------------------------------

	[DoNotList]
	[Name("Get Internal Var")]
	[Description("Returns the selected and previously set Internal Variable's input value.")]
	[Category("Variables/Internal")]
	[ContextDefinedOutputs(typeof(Wild))]
	[ExposeAsDefinition]
	abstract public class RelayValueOutputBase : FlowNode{
		abstract public void SetSource(RelayValueInputBase source);
	}

	///Relay Output
	public class RelayValueOutput<T> : RelayValueOutputBase {
		
		[SerializeField]
		private string _sourceInputUID;
		private string sourceInputUID{
			get {return _sourceInputUID;}
			set {_sourceInputUID = value;}
		}

		private object _sourceInput;
		private RelayValueInput<T> sourceInput{
			get
			{
				if (_sourceInput == null){
					_sourceInput = graph.GetAllNodesOfType<RelayValueInput<T>>().FirstOrDefault(i => i.UID == sourceInputUID);
					if (_sourceInput == null){
						_sourceInput = new object();
					}
				}
				return _sourceInput as RelayValueInput<T>;
			}
			set { _sourceInput = value; }
		}

		public override string name{
			get {return string.Format("{0}", sourceInput != null? sourceInput.ToString() : "@ NONE");}
		}

		public override void SetSource(RelayValueInputBase source){
			_sourceInputUID = source != null? source.UID : null;
			_sourceInput = source != null? source : null;
			GatherPorts();
		}

		protected override void RegisterPorts(){
			AddValueOutput<T>("Value", ()=>{ return sourceInput != null? sourceInput.port.value : default(T); });
		}


		///----------------------------------------------------------------------------------------------
		///---------------------------------------UNITY EDITOR-------------------------------------------
		#if UNITY_EDITOR

		protected override void OnNodeInspectorGUI(){
			if (sourceInput == null){
				var relayInputs = graph.GetAllNodesOfType<RelayValueInputBase>();
				var newInput = EditorUtils.Popup<RelayValueInputBase>("Internal Var Source", sourceInput, relayInputs);
				if (newInput != sourceInput){
					if (newInput == null){
						SetSource(null);
						return;
					}
					if (newInput.relayType == typeof(T)){
						SetSource(newInput);
						return;
					}

					var newNode = (RelayValueOutputBase)ReplaceWith( typeof(RelayValueOutput<>).MakeGenericType(newInput.relayType) );
					newNode.SetSource( (RelayValueInputBase)newInput );
				}
			}
		}

		#endif
		///----------------------------------------------------------------------------------------------

	}
}