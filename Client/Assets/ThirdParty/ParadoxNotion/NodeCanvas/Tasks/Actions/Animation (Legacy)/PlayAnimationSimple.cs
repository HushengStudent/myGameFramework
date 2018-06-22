using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("Animation")]
	public class PlayAnimationSimple : ActionTask<Animation>{

		[RequiredField]
		public BBParameter<AnimationClip> animationClip;
		[SliderField(0,1)]
		public float crossFadeTime = 0.25f;
		public WrapMode animationWrap= WrapMode.Loop;
		public bool waitActionFinish = true;

		//holds the last played animationClip.value for each agent 
		private static Dictionary<Animation, AnimationClip> lastPlayedClips = new Dictionary<Animation, AnimationClip>();

		protected override string info{
			get {return "Anim " + animationClip.ToString();}
		}

		protected override string OnInit(){
			agent.AddClip(animationClip.value, animationClip.value.name);
			animationClip.value.legacy = true;
			return null;
		}

		protected override void OnExecute(){

			if (lastPlayedClips.ContainsKey(agent) && lastPlayedClips[agent] == animationClip.value){
				EndAction(true);
				return;
			}

			lastPlayedClips[agent] = animationClip.value;
			agent[animationClip.value.name].wrapMode = animationWrap;
			agent.CrossFade(animationClip.value.name, crossFadeTime);
			
			if (!waitActionFinish){
				EndAction(true);
			}
		}

		protected override void OnUpdate(){

			if (elapsedTime >= animationClip.value.length - crossFadeTime){
				EndAction(true);
			}
		}
	}
}