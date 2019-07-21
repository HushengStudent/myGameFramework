using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("Physics")]
    public class GetLinecastInfo : ActionTask<Transform>
    {

        [RequiredField]
        public BBParameter<GameObject> target;
        public BBParameter<LayerMask> layerMask = (LayerMask)( -1 );
        [BlackboardOnly]
        public BBParameter<GameObject> saveHitGameObjectAs;
        [BlackboardOnly]
        public BBParameter<float> saveDistanceAs;
        [BlackboardOnly]
        public BBParameter<Vector3> savePointAs;
        [BlackboardOnly]
        public BBParameter<Vector3> saveNormalAs;

        private RaycastHit hit = new RaycastHit();

        protected override void OnExecute() {

            if ( Physics.Linecast(agent.position, target.value.transform.position, out hit, layerMask.value) ) {
                saveHitGameObjectAs.value = hit.collider.gameObject;
                saveDistanceAs.value = hit.distance;
                savePointAs.value = hit.point;
                saveNormalAs.value = hit.normal;
                EndAction(true);
                return;
            }

            EndAction(false);
        }

        public override void OnDrawGizmosSelected() {
            if ( agent && target.value )
                Gizmos.DrawLine(agent.position, target.value.transform.position);
        }
    }
}