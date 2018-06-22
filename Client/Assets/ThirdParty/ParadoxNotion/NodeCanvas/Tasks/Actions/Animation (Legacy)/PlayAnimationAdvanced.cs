using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("Animation")]
	public class PlayAnimationAdvanced : ActionTask<Animation>{

		[RequiredField]
		public BBParameter<AnimationClip> animationClip;
		public WrapMode animationWrap;
		public AnimationBlendMode blendMode;
		[SliderField(0,2)]
		public float playbackSpeed = 1;
		[SliderField(0,1)]
		public float crossFadeTime= 0.25f;
		public PlayDirections playDirection = PlayDirections.Forward;
		public BBParameter<string> mixTransformName;
		public BBParameter<int> animationLayer;
		public bool queueAnimation;
		public bool waitActionFinish = true;

		private string animationToPlay = string.Empty;
		private int dir = -1;
		private Transform mixTransform;

		protected override string info{
			get {return "Anim " + animationClip.ToString() ;}
		}

		protected override string OnInit(){
			agent.AddClip(animationClip.value, animationClip.value.name);
			animationClip.value.legacy = true;
			return null;
		}

		protected override void OnExecute(){

			if (playDirection == PlayDirections.Toggle)
				dir = -dir;

			if (playDirection == PlayDirections.Backward)
				dir = -1;

			if (playDirection == PlayDirections.Forward)
				dir = 1;
			
			agent.AddClip(animationClip.value, animationClip.value.name);
			animationToPlay = animationClip.value.name;

			if (!string.IsNullOrEmpty(mixTransformName.value)){
				mixTransform = FindTransform(agent.transform, mixTransformName.value);
				if (!mixTransform){
					Debug.LogWarning("Cant find transform with name '" + mixTransformName.value + "' for PlayAnimation Action");
				}
			
			} else {
				mixTransform = null;
			}

			animationToPlay = animationClip.value.name;

			if (mixTransform){
				agent[animationToPlay].AddMixingTransform(mixTransform, true);
			}
			
			agent[animationToPlay].layer = animationLayer.value;
			agent[animationToPlay].speed = dir * playbackSpeed;
			agent[animationToPlay].normalizedTime = Mathf.Clamp01(-dir);
			agent[animationToPlay].wrapMode = animationWrap;
			agent[animationToPlay].blendMode = blendMode;
			
			if (queueAnimation){
				agent.CrossFadeQueued(animationToPlay, crossFadeTime);
			} else {
				agent.CrossFade(animationToPlay, crossFadeTime);
			}

			if (!waitActionFinish){
				EndAction(true);
			}
		}

		protected override void OnUpdate(){

			if (elapsedTime >= (agent[animationToPlay].length / playbackSpeed) - crossFadeTime){
				EndAction(true);
			}
		}

		Transform FindTransform(Transform parent, string name){

			if (parent.name == name)
				return parent;

			var transforms= parent.GetComponentsInChildren<Transform>();

			foreach (var t in transforms){
				if (t.name == name)
					return t;
			}

			return null;
		}
	}
}