using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("Target In View Angle")]
    [Category("GameObject")]
    [Description("Checks whether the target is in the view angle of the agent")]
    public class IsInFront : ConditionTask<Transform>
    {

        [RequiredField]
        public BBParameter<GameObject> checkTarget;
        [SliderField(1, 180)]
        public BBParameter<float> viewAngle = 70f;

        protected override string info {
            get { return checkTarget + " in view angle"; }
        }

        protected override bool OnCheck() {
            return Vector3.Angle(checkTarget.value.transform.position - agent.position, agent.forward) < viewAngle.value;
        }

        public override void OnDrawGizmosSelected() {
            if ( agent != null ) {
                Gizmos.matrix = Matrix4x4.TRS(agent.position, agent.rotation, Vector3.one);
                Gizmos.DrawFrustum(Vector3.zero, viewAngle.value, 5, 0, 1f);
            }
        }
    }
}