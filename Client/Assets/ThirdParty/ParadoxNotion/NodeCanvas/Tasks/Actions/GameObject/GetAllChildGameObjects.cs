using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("GameObject")]
    public class GetAllChildGameObjects : ActionTask<Transform>
    {

        [BlackboardOnly]
        public BBParameter<List<GameObject>> saveAs;
        public bool recursive = false;

        protected override string info {
            get { return string.Format("{0} = {1} Children Of {2}", saveAs, recursive ? "All" : "First", agentInfo); }
        }

        protected override void OnExecute() {

            var found = new List<Transform>();
            foreach ( Transform t in agent.transform ) {
                found.Add(t);
                if ( recursive ) {
                    found.AddRange(Get(t));
                }
            }
            saveAs.value = found.Select(t => t.gameObject).ToList();
            EndAction();
        }

        List<Transform> Get(Transform parent) {
            var found = new List<Transform>();
            foreach ( Transform t in parent ) {
                found.Add(t);
                found.AddRange(Get(t));
            }
            return found;
        }
    }
}