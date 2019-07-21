using System;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer
{

    /// fsContext stores global metadata that can be used to customize how fsConverters operate
    /// during serialization.
    public sealed class fsContext
    {

        /// All of the context objects.
        private readonly Dictionary<Type, object> _contextObjects = new Dictionary<Type, object>();

        /// Removes all context objects from the context.
        public void Reset() {
            _contextObjects.Clear();
        }

        /// Sets the context object for the given type with the given value.
        public void Set<T>(T obj) {
            _contextObjects[typeof(T)] = obj;
        }

        /// Returns true if there is a context object for the given type.
        public bool Has<T>() {
            return _contextObjects.ContainsKey(typeof(T));
        }

        /// Fetches the context object for the given type.
        public T Get<T>() {
            object val;
            if ( _contextObjects.TryGetValue(typeof(T), out val) ) {
                return (T)val;
            }
            throw new InvalidOperationException("There is no context object of type " + typeof(T));
        }
    }
}