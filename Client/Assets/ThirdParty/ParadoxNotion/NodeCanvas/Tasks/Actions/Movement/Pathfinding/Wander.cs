using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

#if UNITY_5_5_OR_NEWER
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
using NavMesh = UnityEngine.AI.NavMesh;
using NavMeshHit = UnityEngine.AI.NavMeshHit;
#endif

namespace NodeCanvas.Tasks.Actions
{

    [Category("Movement/Pathfinding")]
    [Description("Makes the agent wander randomly within the navigation map")]
    public class Wander : ActionTask<NavMeshAgent>
    {

        public BBParameter<float> speed = 4;
        public BBParameter<float> keepDistance = 0.1f;
        public BBParameter<float> minWanderDistance = 5;
        public BBParameter<float> maxWanderDistance = 20;
        public bool repeat = true;

        protected override void OnExecute() {
            agent.speed = speed.value;
            DoWander();
        }

        protected override void OnUpdate() {
            if ( !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + keepDistance.value ) {
                if ( repeat ) {
                    DoWander();
                } else {
                    EndAction();
                }
            }
        }

        void DoWander() {
            var min = minWanderDistance.value;
            var max = maxWanderDistance.value;
            min = Mathf.Clamp(min, 0.01f, max);
            max = Mathf.Clamp(max, min, max);
            var wanderPos = agent.transform.position;
            while ( ( wanderPos - agent.transform.position ).sqrMagnitude < min ) {
                wanderPos = ( Random.insideUnitSphere * max ) + agent.transform.position;
            }

            NavMeshHit hit;
            if ( NavMesh.SamplePosition(wanderPos, out hit, float.PositiveInfinity, NavMesh.AllAreas) ) {
                agent.SetDestination(hit.position);
            }
        }

        protected override void OnPause() { OnStop(); }
        protected override void OnStop() {
            if ( agent != null && agent.gameObject.activeSelf ) {
                agent.ResetPath();
            }
        }
    }
}