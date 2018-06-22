using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("Input")]
	public class GetInputAxis : ActionTask {

		public BBParameter<string> xAxisName = "Horizontal" ;
		public BBParameter<string> yAxisName;
		public BBParameter<string> zAxisName = "Vertical";
		public BBParameter<float> multiplier = 1;
		[BlackboardOnly]
		public BBParameter<Vector3> saveAs;
		[BlackboardOnly]
		public BBParameter<float> saveXAs;
		[BlackboardOnly]
		public BBParameter<float> saveYAs;
		[BlackboardOnly]
		public BBParameter<float> saveZAs;

		public bool repeat;

		protected override void OnExecute(){ Do(); }
		protected override void OnUpdate(){ Do(); }

		void Do(){

			var x = string.IsNullOrEmpty(xAxisName.value)? 0 : Input.GetAxis(xAxisName.value);
			var y = string.IsNullOrEmpty(yAxisName.value)? 0 : Input.GetAxis(yAxisName.value);
			var z = string.IsNullOrEmpty(zAxisName.value)? 0 : Input.GetAxis(zAxisName.value);
			
			saveXAs.value = x * multiplier.value;
			saveYAs.value = y * multiplier.value;
			saveZAs.value = z * multiplier.value;
			saveAs.value = new Vector3(x, y, z) * multiplier.value;
			if (!repeat)
				EndAction();
		}
	}
}