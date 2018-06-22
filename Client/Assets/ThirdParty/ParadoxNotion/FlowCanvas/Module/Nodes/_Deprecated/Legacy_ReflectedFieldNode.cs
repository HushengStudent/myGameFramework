using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using ParadoxNotion;

namespace FlowCanvas.Nodes.Legacy{

	///Base class for FieldInfo based nodes
	abstract public class ReflectedFieldNode {

		public static ReflectedFieldNode Create(FieldInfo field){
			return new PureReflectedFieldNode();
		}

		abstract public void RegisterPorts(FlowNode node, FieldInfo field, ReflectedFieldNodeWrapper.AccessMode accessMode);
	}


	//Pure refelction based
	sealed public class PureReflectedFieldNode : ReflectedFieldNode{

		public override void RegisterPorts(FlowNode node, FieldInfo field, ReflectedFieldNodeWrapper.AccessMode accessMode){
			if ( field.IsConstant() ){
				var constantValue = field.GetValue(null);
				node.AddValueOutput("Value", field.FieldType, ()=>{ return constantValue; });
				return;
			}

			var targetType = field.DeclaringType;
			if (accessMode == ReflectedFieldNodeWrapper.AccessMode.GetField){
				var instanceInput = node.AddValueInput(targetType.FriendlyName(), targetType);
				node.AddValueOutput("Value", field.FieldType, ()=>{ return field.GetValue(instanceInput.value); });			

			} else {

				object instance = null;
				var instanceInput = node.AddValueInput(targetType.FriendlyName(), targetType);
				var valueInput = node.AddValueInput("Value", field.FieldType);
				var flowOut = node.AddFlowOutput(" ");
				node.AddValueOutput(targetType.FriendlyName(), targetType, ()=>{ return instance; });
				node.AddFlowInput(" ", (f)=> { instance = instanceInput.value; field.SetValue(instance, valueInput.value); flowOut.Call(f); });
			}			
		}

	}
}