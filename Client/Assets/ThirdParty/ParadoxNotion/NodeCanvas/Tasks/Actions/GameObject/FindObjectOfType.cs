using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("GameObject")]
	[Description("Note that this is very slow")]
	public class FindObjectOfType<T> : ActionTask where T:Component{

		[BlackboardOnly]
		public BBParameter<T> saveComponentAs;
		[BlackboardOnly]
		public BBParameter<GameObject> saveGameObjectAs;

		protected override void OnExecute(){
			var o = Object.FindObjectOfType<T>();
			if (o != null){
				saveComponentAs.value = o;
				saveGameObjectAs.value = o.gameObject;
				EndAction(true);
				return;
			}

			EndAction(false);
		}
	}
}