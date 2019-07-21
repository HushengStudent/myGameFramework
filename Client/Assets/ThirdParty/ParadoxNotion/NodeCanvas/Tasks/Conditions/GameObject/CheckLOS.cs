using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("Target In Line Of Sight")]
    [Category("GameObject")]
    [Description("Check of agent is in line of sight with target by doing a linecast and optionaly save the distance")]
    public class CheckLOS : ConditionTask<Transform>
    {

        [RequiredField]
        public BBParameter<GameObject> LOSTarget;
        public BBParameter<LayerMask> layerMask = (LayerMask)( -1 );
        public Vector3 offset;
        [BlackboardOnly]
        public BBParameter<float> saveDistanceAs;

        private RaycastHit hit = new RaycastHit();

        protected override string info {
            get { return "LOS with " + LOSTarget.ToString(); }
        }

        protected override bool OnCheck() {

            var t = LOSTarget.value.transform;

            if ( Physics.Linecast(agent.position + offset, t.position + offset, out hit, layerMask.value) ) {
                var targetCollider = t.GetComponent<Collider>();
                if ( targetCollider == null || hit.collider != targetCollider ) {
                    saveDistanceAs.value = hit.distance;
                    return false;
                }
            }

            return true;
        }

        public override void OnDrawGizmosSelected() {
            if ( agent && LOSTarget.value ) {
                Gizmos.DrawLine(agent.position + offset, LOSTarget.value.transform.position + offset);
            }
        }
    }
}