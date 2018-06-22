using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("GameObject")]
	public class GetDistance : ActionTask<Transform> {

		[RequiredField]
		public BBParameter<GameObject> target;
		[BlackboardOnly]
		public BBParameter<float> saveAs;

		protected override string info{
			get {return string.Format("Get Distance to {0}", target.ToString());}
		}

		protected override void OnExecute(){
			
			saveAs.value = Vector3.Distance(agent.position, target.value.transform.position);
			EndAction();
		}
	}
}