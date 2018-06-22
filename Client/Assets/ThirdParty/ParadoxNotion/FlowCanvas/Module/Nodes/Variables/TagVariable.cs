using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes{

	[DoNotList]
	[Name("Tag")]
	[Description("An easy way to get a Tag name")]
	public class TagVariable : VariableNode {
		
		[TagField]
		public BBParameter<string> tagName = "Untagged";
		
		public override string name{
			get	{ return tagName.value ;}
		}

		protected override void RegisterPorts(){
			AddValueOutput<string>("Tag", ()=> {return tagName.value; });
		}

		public override void SetVariable(object o){
			if (o is string){
				tagName.value = (string)o;
			}
		}
	}
}