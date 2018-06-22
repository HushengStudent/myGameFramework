using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("GameObject")]
	public class DestroyGameObject : ActionTask<Transform> {

		protected override string info{
			get {return string.Format("Destroy {0}", agentInfo);}
		}

		//in case it destroys self
		protected override void OnUpdate(){
			Object.Destroy(agent.gameObject);
			EndAction();
		}
	}
}