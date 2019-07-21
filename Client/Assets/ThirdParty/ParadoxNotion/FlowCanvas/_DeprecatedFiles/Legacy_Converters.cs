using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [Category("Utilities/Converters")]
    [Obsolete]
    public class ConvertTo<T> : PureFunctionNode<T, IConvertible> where T : IConvertible
    {
        public override T Invoke(IConvertible obj) {
            return (T)Convert.ChangeType(obj, typeof(T));
        }
    }

    [Category("Utilities/Converters")]
    [Obsolete]
    public class CastTo<T> : PureFunctionNode<T, object>
    {
        public override T Invoke(object obj) {
            try { return (T)obj; }
            catch { return default(T); }
        }
    }

    [Category("Utilities/Converters")]
    [Obsolete]
    public class ToArray<T> : PureFunctionNode<T[], IList<T>>
    {
        public override T[] Invoke(IList<T> list) {
            return list.ToArray();
        }
    }

    [Category("Utilities/Converters")]
    [Obsolete]
    public class ToList<T> : PureFunctionNode<List<T>, IList<T>>
    {
        public override List<T> Invoke(IList<T> list) {
            return list.ToList();
        }
    }
}