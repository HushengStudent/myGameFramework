using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Name("Curve Tween")]
	[Category("Movement/Direct")]
	public class CurveTransformTween : ActionTask<Transform> {

		public enum TransformMode{
			Position,
			Rotation,
			Scale
		}

		public enum TweenMode{
			Absolute,
			Additive
		}

		public enum PlayMode{
			Normal,
			PingPong
		}

		public TransformMode transformMode;
		public TweenMode mode;
		public PlayMode playMode;
		public BBParameter<Vector3> targetPosition;
		public BBParameter<AnimationCurve> curve = AnimationCurve.EaseInOut(0,0,1,1);
		public BBParameter<float> time = 0.5f;

		private Vector3 original;
		private Vector3 final;
		private bool ponging = false;

		protected override void OnExecute(){

			if (ponging)
				final = original;

			if (transformMode == TransformMode.Position)
				original = agent.localPosition;
			if (transformMode == TransformMode.Rotation)
				original = agent.localEulerAngles;
			if (transformMode == TransformMode.Scale)
				original = agent.localScale;

			if (!ponging)
				final = targetPosition.value + (mode == TweenMode.Additive? original : Vector3.zero);

			ponging = playMode == PlayMode.PingPong;

			if ( (original - final).magnitude < 0.1f )
				EndAction();
		}

		protected override void OnUpdate(){
			
			var value = Vector3.Lerp(original, final, curve.value.Evaluate( elapsedTime/time.value ) );

			if (transformMode == TransformMode.Position)
				agent.localPosition = value;
			if (transformMode == TransformMode.Rotation)
				agent.localEulerAngles = value;
			if (transformMode == TransformMode.Scale)
				agent.localScale = value;			

			if (elapsedTime >= time.value)
				EndAction(true);
		}
	}
}