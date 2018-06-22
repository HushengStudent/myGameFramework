using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

	[Category("System Events")]
	[Name("Check Mouse Click 2D")]
	[EventReceiver("OnMouseDown", "OnMouseUp")]
	public class CheckMouseClick2D : ConditionTask<Collider2D> {

		public MouseClickEvent checkType = MouseClickEvent.MouseDown;

		protected override string info{
			get{ return checkType.ToString();}
		}

		protected override bool OnCheck(){
			return false;
		}

		public void OnMouseDown(){
			if (checkType == MouseClickEvent.MouseDown)
				YieldReturn(true);
		}

		public void OnMouseUp(){
			if (checkType == MouseClickEvent.MouseUp)
				YieldReturn(true);
		}
	}
}