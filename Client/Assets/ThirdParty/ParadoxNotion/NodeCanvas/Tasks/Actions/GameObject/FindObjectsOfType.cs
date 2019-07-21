using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("GameObject")]
    [Description("Note that this is very slow")]
    public class FindObjectsOfType<T> : ActionTask where T : Component
    {

        [BlackboardOnly]
        public BBParameter<List<GameObject>> saveGameObjects;
        [BlackboardOnly]
        public BBParameter<List<T>> saveComponents;

        protected override void OnExecute() {

            var objects = Object.FindObjectsOfType<T>();
            if ( objects != null && objects.Length != 0 ) {
                saveGameObjects.value = objects.Select(o => o.gameObject).ToList();
                saveComponents.value = objects.ToList();
                EndAction(true);
                return;
            }

            EndAction(false);
        }
    }
}