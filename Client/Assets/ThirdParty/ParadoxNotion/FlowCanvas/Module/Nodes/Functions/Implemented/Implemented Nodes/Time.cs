using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{

	[Category("Time")]
	[Description("Wait for a certain amount of time before continueing")]
	public class Wait : LatentActionNode<float>{
		public float timeLeft{ get; private set; }
		public override IEnumerator Invoke(float time = 1f){
			timeLeft = time;
			while (timeLeft > 0){
				timeLeft -= Time.deltaTime;
				timeLeft = Mathf.Max(timeLeft, 0);
				yield return null;
			}
		}
	}

	
	[Category("Time")]
	[Description("Wait for one (the next) frame")]
	public class WaitForOneFrame : LatentActionNode{
		public override bool exposeRoutineControls{get{return false;}}
		public override IEnumerator Invoke(){
			yield return null;
		}
	}

	[Category("Time")]
	[Description("Wait until the physics update frame")]
	public class WaitForPhysicsFrame : LatentActionNode{
		public override bool exposeRoutineControls{get{return false;}}
		public override IEnumerator Invoke(){
			yield return new UnityEngine.WaitForFixedUpdate();
		}
	}

	[Category("Time")]
	[Description("Wait until the end of current frame")]
	public class WaitForEndOfFrame : LatentActionNode{
		public override bool exposeRoutineControls{get{return false;}}
		public override IEnumerator Invoke(){
			yield return new UnityEngine.WaitForEndOfFrame();
		}
	}

	[Name("Per Second (Float)")]
	[Category("Time")]
	[Description("Mutliply input value by Time.deltaTime and optional multiplier")]
	public class DeltaTimed : PureFunctionNode<float, float, float>{
		public override float Invoke(float value, float multiplier = 1){
			return value * multiplier * Time.deltaTime;
		}
	}

	[Name("Per Second (Vector3)")]
	[Category("Time")]
	[Description("Mutliply input value by Time.deltaTime and optional multiplier")]
	public class DeltaTimedVector3 : PureFunctionNode<Vector3, Vector3, float>{
		public override Vector3 Invoke(Vector3 value, float multiplier = 1){
			return value * multiplier * Time.deltaTime;
		}
	}
}