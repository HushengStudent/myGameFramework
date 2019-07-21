using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
    public class fsCyclicReferenceManager
    {
        // We use the default ReferenceEquals when comparing two objects because
        // custom objects may override equals methods. These overriden equals may
        // treat equals differently; we want to serialize/deserialize the object
        // graph *identically* to how it currently exists.
        class ObjectReferenceEqualityComparator : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y) {
                return ReferenceEquals(x, y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj) {
                return RuntimeHelpers.GetHashCode(obj);
            }

            public static readonly IEqualityComparer<object> Instance = new ObjectReferenceEqualityComparator();
        }

        private Dictionary<object, int> _objectIds;
        private int _nextId;

        private Dictionary<int, object> _marked;
        private int _depth;

        public fsCyclicReferenceManager() {
            _objectIds = new Dictionary<object, int>(ObjectReferenceEqualityComparator.Instance);
            _marked = new Dictionary<int, object>();
        }

        public void Enter() {
            _depth++;
        }

        public bool Exit() {
            _depth--;

            if ( _depth == 0 ) {
                _nextId = 0;
                _objectIds.Clear();
                _marked.Clear();
            }

            if ( _depth < 0 ) {
                _depth = 0;
                throw new InvalidOperationException("Internal Error - Mismatched Enter/Exit");
            }

            return _depth == 0;
        }

        public object GetReferenceObject(int id) {
            object result = null;
            if ( !_marked.TryGetValue(id, out result) ) {
                throw new InvalidOperationException("Internal Deserialization Error - Object " +
                    "definition has not been encountered for object with id=" + id +
                    "; have you reordered or modified the serialized data? If this is an issue " +
                    "with an unmodified Full Json implementation and unmodified serialization " +
                    "data, please report an issue with an included test case.");
            }

            return result;
        }

        public void AddReferenceWithId(int id, object reference) {
            _marked[id] = reference;
        }

        public int GetReferenceId(object item) {
            int id;
            if ( !_objectIds.TryGetValue(item, out id) ) {
                id = _nextId++;
                _objectIds[item] = id;
            }
            return id;
        }

        public bool IsReference(object item) {
            return _marked.ContainsKey(GetReferenceId(item));
        }

        public void MarkSerialized(object item) {
            int referenceId = GetReferenceId(item);

            if ( _marked.ContainsKey(referenceId) ) {
                throw new InvalidOperationException("Internal Error - " + item + " has already been marked as serialized");
            }

            _marked[referenceId] = item;
        }
    }
}