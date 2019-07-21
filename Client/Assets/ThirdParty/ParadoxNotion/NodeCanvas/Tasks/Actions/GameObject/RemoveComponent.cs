using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("GameObject")]
    public class RemoveComponent<T> : ActionTask<Transform> where T : Component
    {

        [Tooltip("DestroyImmediately is recomended if you are destroying objects in use of the framework.")]
        public bool immediately;

        protected override string info {
            get { return string.Format("Remove '{0}'", typeof(T).Name); }
        }

        protected override void OnExecute() {
            var o = agent.GetComponent<T>();
            if ( o != null ) {
                if ( immediately ) {
                    Object.DestroyImmediate(o);
                } else {
                    Object.Destroy(o);
                }
                EndAction(true);
                return;
            }

            EndAction(false);
        }
    }
}