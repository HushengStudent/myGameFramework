using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Category("Decorators")]
    [Description("Interupts decorated child node and returns Failure if the child node is still Running after the timeout period")]
    [Icon("Timeout")]
    public class Timeout : BTDecorator
    {

        public BBParameter<float> timeout = 1;

        private float timer;

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            status = decoratedConnection.Execute(agent, blackboard);

            if ( status == Status.Running ) {
                timer += Time.deltaTime;
                if ( timer >= timeout.value ) {
                    timer = 0;
                    decoratedConnection.Reset();
                    return Status.Failure;
                }
            }

            return status;
        }

        protected override void OnReset() {
            timer = 0;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Space(25);
            var pRect = new Rect(5, GUILayoutUtility.GetLastRect().y, rect.width - 10, 20);
            var t = 1 - ( timer / timeout.value );
            UnityEditor.EditorGUI.ProgressBar(pRect, t, timer > 0 ? string.Format("Timeouting ({0})", timer.ToString("0.0")) : "Ready");
        }

#endif
        ///----------------------------------------------------------------------------------------------

    }
}