using System;
using UnityEngine;
using ParadoxNotion;

namespace FlowCanvas
{

    ///Defines a dynamic, unknown type port. Used when ports should be dynamic.
    [Serializable]
    public class DynamicPortDefinition : ISerializationCallbackReceiver
    {

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            _type = resolvedType != null ? resolvedType.FullName : null;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            resolvedType = ReflectionTools.GetType(_type, /*fallback?*/ true);
        }

        [SerializeField]
        private string _ID;
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _type;

        [NonSerialized]
        private Type resolvedType;

        //The ID of the definition port
        public string ID {
            get
            {
                if ( string.IsNullOrEmpty(_ID) ) { //for correct update prior versions
                    _ID = name;
                }
                return _ID;
            }
            private set { _ID = value; }
        }

        //The name of the definition port
        public string name {
            get { return _name; }
            set { _name = value; }
        }

        ///The Type of the definition port
        public Type type {
            get { return resolvedType; }
            set { resolvedType = value; }
        }

        ///requiered
        public DynamicPortDefinition() { }

        public DynamicPortDefinition(string name, Type type) {
            this.ID = Guid.NewGuid().ToString();
            this.name = name;
            this.type = type;
        }
    }
}