using System;
using System.Collections;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	///Action Nodes do not return any value and can have up to 10 parameters. They need Flow execution.
	abstract public class CallableActionNodeBase : SimplexNode {}

	abstract public class CallableActionNode : CallableActionNodeBase{
		abstract public void Invoke();
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			node.AddFlowInput(" ", (Flow f)=> { Invoke(); o.Call(f); });
		}		
	}

	abstract public class CallableActionNode<T1> : CallableActionNodeBase{
		abstract public void Invoke(T1 a);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			node.AddFlowInput(" ", (Flow f)=> { Invoke(p1.value); o.Call(f); });
		}		
	}

	abstract public class CallableActionNode<T1, T2> : CallableActionNodeBase{
		abstract public void Invoke(T1 a, T2 b);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			node.AddFlowInput(" ", (Flow f)=> { Invoke(p1.value, p2.value); o.Call(f); });
		}		
	}

	abstract public class CallableActionNode<T1, T2, T3> : CallableActionNodeBase{
		abstract public void Invoke(T1 a, T2 b, T3 c);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			node.AddFlowInput(" ", (Flow f)=> { Invoke(p1.value, p2.value, p3.value); o.Call(f); });
		}		
	}

	abstract public class CallableActionNode<T1, T2, T3, T4> : CallableActionNodeBase{
		abstract public void Invoke(T1 a, T2 b, T3 c, T4 d);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			node.AddFlowInput(" ", (Flow f)=> { Invoke(p1.value, p2.value, p3.value, p4.value); o.Call(f); });
		}		
	}

	abstract public class CallableActionNode<T1, T2, T3, T4, T5> : CallableActionNodeBase{
		abstract public void Invoke(T1 a, T2 b, T3 c, T4 d, T5 e);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			node.AddFlowInput(" ", (Flow f)=> { Invoke(p1.value, p2.value, p3.value, p4.value, p5.value); o.Call(f); });
		}		
	}

	abstract public class CallableActionNode<T1, T2, T3, T4, T5, T6> : CallableActionNodeBase{
		abstract public void Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			node.AddFlowInput(" ", (Flow f)=> { Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value); o.Call(f); });
		}		
	}

	abstract public class CallableActionNode<T1, T2, T3, T4, T5, T6, T7> : CallableActionNodeBase{
		abstract public void Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			node.AddFlowInput(" ", (Flow f)=> { Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value); o.Call(f); });
		}		
	}

	abstract public class CallableActionNode<T1, T2, T3, T4, T5, T6, T7, T8> : CallableActionNodeBase{
		abstract public void Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			var p8 = node.AddValueInput<T8>(parameters[7].Name);
			node.AddFlowInput(" ", (Flow f)=> { Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value); o.Call(f); });
		}		
	}

	abstract public class CallableActionNode<T1, T2, T3, T4, T5, T6, T7, T8, T9> : CallableActionNodeBase{
		abstract public void Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			var p8 = node.AddValueInput<T8>(parameters[7].Name);
			var p9 = node.AddValueInput<T9>(parameters[8].Name);
			node.AddFlowInput(" ", (Flow f)=> { Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value, p9.value); o.Call(f); });
		}		
	}

	abstract public class CallableActionNode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : CallableActionNodeBase{
		abstract public void Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i, T10 j);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			var p8 = node.AddValueInput<T8>(parameters[7].Name);
			var p9 = node.AddValueInput<T9>(parameters[8].Name);
			var p10 = node.AddValueInput<T10>(parameters[9].Name);
			node.AddFlowInput(" ", (Flow f)=> { Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value, p9.value, p10.value); o.Call(f); });
		}		
	}
}