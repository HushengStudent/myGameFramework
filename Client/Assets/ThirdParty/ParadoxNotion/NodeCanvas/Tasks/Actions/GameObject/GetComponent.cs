using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("GameObject")]
	public class GetComponent<T> : ActionTask<Transform> where T:Component{

		[BlackboardOnly]
		public BBParameter<T> saveAs;

		protected override string info{
			get{return string.Format("Get {0} as {1}", typeof(T).Name, saveAs.ToString());}
		}

		protected override void OnExecute(){
			var o = agent.GetComponent<T>();
			saveAs.value = o;
			EndAction( o != null );
		}
	}
}