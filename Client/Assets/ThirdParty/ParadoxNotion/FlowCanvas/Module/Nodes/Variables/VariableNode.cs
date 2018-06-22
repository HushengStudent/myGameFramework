using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Category("Variables")]
	abstract public class VariableNode : FlowNode {

		///For setting the default variable
		abstract public void SetVariable(object o);
	}
}