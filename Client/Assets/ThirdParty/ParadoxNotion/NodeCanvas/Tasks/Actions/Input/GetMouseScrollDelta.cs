using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("Input")]
	public class GetMouseScrollDelta : ActionTask {

		[BlackboardOnly]
		public BBParameter<float> saveAs;
		public bool repeat = false;

		protected override string info{
			get {return "Get Scroll Delta as " + saveAs;}
		}

		protected override void OnExecute(){
			Do();
		}

		protected override void OnUpdate(){
			Do();
		}

		void Do(){

			saveAs.value = Input.GetAxis("Mouse ScrollWheel");
			if (!repeat)
				EndAction();
		}
	}
}