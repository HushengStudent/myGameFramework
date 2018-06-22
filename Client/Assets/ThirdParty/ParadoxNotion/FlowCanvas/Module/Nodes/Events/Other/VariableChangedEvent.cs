using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Name("On Variable Change")]
	[Category("Events/Other")]
	[Description("Called when the target variable change. (Not whenever it is set).")]
	public class VariableChangedEvent : EventNode {

		[BlackboardOnly]
		public BBObjectParameter targetVariable;

		private FlowOutput fOut;
		private object newValue;

		public override string name{
			get {return string.Format("{0} [{1}]", base.name, targetVariable);}
		}

		public override void OnGraphStarted(){
			if (targetVariable.varRef != null){
				targetVariable.varRef.onValueChanged += OnValueChanged;
			}
		}

		public override void OnGraphStoped(){
			if (targetVariable.varRef != null){
				targetVariable.varRef.onValueChanged -= OnValueChanged;
			}
		}

		protected override void RegisterPorts(){
			#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode){
				targetVariable.onVariableReferenceChanged -= OnVariableRefChange;
				targetVariable.onVariableReferenceChanged += OnVariableRefChange;
			}
			#endif
			fOut = AddFlowOutput("Out");
			AddValueOutput("Value", targetVariable.varType, ()=>{ return newValue; });
		}

		void OnValueChanged(string name, object value){
			newValue = value;
			fOut.Call(new Flow());
		}

		void OnVariableRefChange(Variable newVarRef){
			targetVariable.SetType(newVarRef.varType);
			GatherPorts();
		}
	}
}