using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [Obsolete]
    [Name("Select", 8)]
    [Category("Utility")]
    [Description("Returns either one of the two inputs, based on the boolean condition")]
    [ExposeAsDefinition]
    public class SwitchValue<T> : PureFunctionNode<T, bool, T, T>
    {
        public override T Invoke(bool condition, T isTrue, T isFalse) {
            return condition ? isTrue : isFalse;
        }
    }

    [Obsolete]
    [Category("Utility")]
    [Description("Return a value from the list by index")]
    [ExposeAsDefinition]
    public class PickValue<T> : PureFunctionNode<T, int, IList<T>>
    {
        public override T Invoke(int index, IList<T> values) {
            try { return values[index]; }
            catch { return default(T); }
        }
    }
}