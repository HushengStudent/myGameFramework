using System;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Category("Flow Controllers/Switchers")]
	[Description("Branch the Flow based on a comparison between two comparable objects")]
	[ContextDefinedInputs(typeof(IComparable))]
	public class SwitchComparison : FlowControlNode {

		protected override void RegisterPorts(){
			var equal    = AddFlowOutput("A = B", "==");
			var notEqual = AddFlowOutput("A ≠ B", "!=");
			var greater  = AddFlowOutput("A > B", ">");
			var less     = AddFlowOutput("A < B", "<");

			var a = AddValueInput<IComparable>("A");
			var b = AddValueInput<IComparable>("B");
			AddFlowInput("In", (f)=>{

				var valueA = a.value;
				var valueB = b.value;

				if (valueA == null || valueB == null){

					if (valueA == valueB){
						equal.Call(f);
					}

					if (valueA != valueB){
						notEqual.Call(f);
					}
					return;
				}

				var aInt = TypeConverter.QuickConvert<int>(valueA);
				var bInt = TypeConverter.QuickConvert<int>(valueB);

				if (aInt == bInt){
					equal.Call(f);
				} else {
					notEqual.Call(f);
				}

				if (aInt > bInt){
					greater.Call(f);
				}

				if (aInt < bInt){
					less.Call(f);
				}
			});
		}
	}
}