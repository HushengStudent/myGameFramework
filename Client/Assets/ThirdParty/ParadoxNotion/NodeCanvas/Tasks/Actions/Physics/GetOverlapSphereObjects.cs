using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("Physics")]
    [Description("Gets a lists of game objects that are in the physics overlap sphere at the position of the agent, excluding the agent")]
    public class GetOverlapSphereObjects : ActionTask<Transform>
    {

        public LayerMask layerMask = -1;
        public BBParameter<float> radius = 2;
        [BlackboardOnly]
        public BBParameter<List<GameObject>> saveObjectsAs;

        protected override void OnExecute() {

            var hitColliders = Physics.OverlapSphere(agent.position, radius.value, layerMask);
            saveObjectsAs.value = hitColliders.Select(c => c.gameObject).ToList();
            saveObjectsAs.value.Remove(agent.gameObject);

            if ( saveObjectsAs.value.Count == 0 ) {
                EndAction(false);
                return;
            }

            EndAction(true);
        }

        public override void OnDrawGizmosSelected() {
            if ( agent != null ) {
                Gizmos.color = new Color(1, 1, 1, 0.2f);
                Gizmos.DrawSphere(agent.position, radius.value);
            }
        }
    }
}