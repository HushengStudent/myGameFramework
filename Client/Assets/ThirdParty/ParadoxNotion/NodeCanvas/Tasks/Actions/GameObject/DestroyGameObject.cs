using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("GameObject")]
    public class DestroyGameObject : ActionTask<Transform>
    {

        [Tooltip("DestroyImmediately is recomended if you are destroying objects in use of the framework.")]
        public bool immediately;

        protected override string info {
            get { return string.Format("Destroy {0}", agentInfo); }
        }

        //in case it destroys self
        protected override void OnUpdate() {
            if ( immediately ) {
                Object.DestroyImmediate(agent.gameObject);
            } else {
                Object.Destroy(agent.gameObject);
            }
            EndAction();
        }
    }
}