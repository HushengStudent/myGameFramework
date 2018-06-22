using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions{

	[Category("✫ Utility")]
	[Description("Will return true after a specific amount of time has passed and false while still counting down")]
	public class Timeout : ConditionTask {

		public BBParameter<float> timeout = 1f;
		private float currentTime;
		private Coroutine coroutine;

		protected override string info{
			get {return string.Format("Timeout {0}/{1}", currentTime.ToString("0.00"), timeout.ToString());}
		}

		protected override void OnDisable(){
			if (coroutine != null){
				StopCoroutine(coroutine);
				coroutine = null;
			}
		}

		protected override bool OnCheck(){

			if (coroutine == null){
				currentTime = 0;
				coroutine = StartCoroutine(Do());
			}

			if (currentTime >= timeout.value){
				coroutine = null;
				return true;
			}

			return false;
		}

		IEnumerator Do(){
			while (currentTime < timeout.value){
				currentTime += Time.deltaTime;
				yield return null;
			}
		}
	}
}
