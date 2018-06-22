using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

	[Category("System Events")]
	[Name("Check Mouse 2D")]
	[EventReceiver("OnMouseEnter", "OnMouseExit", "OnMouseOver")]
	public class CheckMouse2D : ConditionTask<Collider2D> {

		public MouseInteractionTypes checkType = MouseInteractionTypes.MouseEnter;

		protected override string info{
			get {return checkType.ToString();}
		}

		protected override bool OnCheck(){
			return false;
		}

		public void OnMouseEnter(){
			if (checkType == MouseInteractionTypes.MouseEnter)
				YieldReturn(true);
		}

		public void OnMouseExit(){
			if (checkType == MouseInteractionTypes.MouseExit)
				YieldReturn(true);
		}

		public void OnMouseOver(){
			if (checkType == MouseInteractionTypes.MouseOver)
				YieldReturn(true);
		}
	}
}