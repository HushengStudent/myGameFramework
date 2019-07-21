using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Blackboard")]
    public class EvaluateCurve : ActionTask
    {

        [RequiredField]
        public BBParameter<AnimationCurve> curve;
        public BBParameter<float> from;
        public BBParameter<float> to = 1;
        public BBParameter<float> time = 1;

        [BlackboardOnly]
        public BBParameter<float> saveAs;

        protected override void OnUpdate() {

            saveAs.value = curve.value.Evaluate(Mathf.Lerp(from.value, to.value, elapsedTime / time.value));
            if ( elapsedTime > time.value )
                EndAction();
        }
    }
}