using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
 
namespace NodeCanvas.Tasks.Actions
{
    [Category("GameObject")]
    public class InstantiateGameObject : ActionTask<Transform> {
        public BBParameter<Transform> parent;
        public BBParameter<Vector3> clonePosition;
        public BBParameter<Vector3> cloneRotation;
        [BlackboardOnly]
        public BBParameter<GameObject> saveCloneAs;
 
        protected override string info{
            get { return "Instantiate " + agentInfo + " under " + (parent.value ? parent.ToString() : "World") + " at " + clonePosition + " as " + saveCloneAs; }
        }
 
        protected override void OnExecute(){
            #if UNITY_5_4_OR_NEWER

            var clone = (GameObject)Object.Instantiate(agent.gameObject, parent.value, false);

            #else

            var clone = (GameObject)Object.Instantiate(agent.gameObject);
            clone.transform.SetParent(parent.value);

            #endif
            
            clone.transform.position = clonePosition.value;
            clone.transform.eulerAngles = cloneRotation.value;
            saveCloneAs.value = clone;
            EndAction();
        }
    }
}