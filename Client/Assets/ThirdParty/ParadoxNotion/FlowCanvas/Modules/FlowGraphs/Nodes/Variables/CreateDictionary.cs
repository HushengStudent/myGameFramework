using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Description("Create a Dictionary of <string, T> objects")]
    [ContextDefinedInputs(typeof(string), typeof(Wild))]
    public class CreateDictionary<T> : VariableNode
    {

        [SerializeField]
        [ExposeField]
        [GatherPortsCallback]
        [MinValue(2)]
        [DelayedField]
        private int _portCount = 4;

        public override void SetVariable(object o) {
            //...
        }

        protected override void RegisterPorts() {
            var keys = new List<ValueInput<string>>();
            var values = new List<ValueInput<T>>();
            for ( var i = 0; i < _portCount; i++ ) {
                keys.Add(AddValueInput<string>("Key" + i.ToString()));
                values.Add(AddValueInput<T>("Value" + i.ToString()));
            }
            AddValueOutput<IDictionary<string, T>>("Dictionary", () =>
          {
              var k = keys.Select(x => x.value).ToList();
              var v = values.Select(x => x.value).ToList();
              return k.ToDictionary(x => x, x => v[k.IndexOf(x)]);
          });
        }
    }
}

