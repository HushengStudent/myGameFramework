using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("Target Within Distance")]
    [Category("GameObject")]
    public class CheckDistanceToGameObject : ConditionTask<Transform>
    {

        [RequiredField]
        public BBParameter<GameObject> checkTarget;
        public CompareMethod checkType = CompareMethod.LessThan;
        public BBParameter<float> distance = 10;

        [SliderField(0, 0.1f)]
        public float floatingPoint = 0.05f;

        protected override string info {
            get { return "Distance" + OperationTools.GetCompareString(checkType) + distance + " to " + checkTarget; }
        }

        protected override bool OnCheck() {
            return OperationTools.Compare(Vector3.Distance(agent.position, checkTarget.value.transform.position), distance.value, checkType, floatingPoint);
        }

        public override void OnDrawGizmosSelected() {
            if ( agent != null ) {
                Gizmos.DrawWireSphere(agent.position, distance.value);
            }
        }
    }
}