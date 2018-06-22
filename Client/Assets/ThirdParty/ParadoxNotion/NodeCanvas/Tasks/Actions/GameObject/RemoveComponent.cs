using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("GameObject")]
	public class RemoveComponent<T> : ActionTask<Transform> where T:Component{

		protected override string info{
			get {return string.Format("Remove '{0}'", typeof(T).Name);}
		}

		protected override void OnExecute(){
			var o = agent.GetComponent<T>();
			if (o != null){
				Object.Destroy(o);
				EndAction(true);
				return;
			}

			EndAction(false);
		}
	}
}