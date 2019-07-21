#if !UNITY_2018_3_OR_NEWER

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using ParadoxNotion;

namespace NodeCanvas.Framework
{

    [RequireComponent(typeof(Blackboard))]
    public class SyncBlackboard : NetworkBehaviour
    {

        class SyncVarMessage : MessageBase
        {

            public string name;
            public object value;

            public SyncVarMessage() { }
            public SyncVarMessage(string name, object value) {
                this.name = name;
                this.value = value;
            }

            public override void Serialize(NetworkWriter writer) {

                writer.Write(name);

                var isNull = value == null;
                writer.Write(isNull);
                if ( isNull ) {
                    return;
                }

                var typeName = value.GetType().AssemblyQualifiedName;
                var t = System.Type.GetType(typeName);
                writer.Write((string)typeName);

                if ( t == typeof(string) ) {
                    writer.Write((string)value);
                    return;
                }
                if ( t == typeof(bool) ) {
                    writer.Write((bool)value);
                    return;
                }
                if ( t == typeof(int) ) {
                    writer.Write((int)value);
                    return;
                }
                if ( t == typeof(float) ) {
                    writer.Write((float)value);
                    return;
                }
                if ( t == typeof(Color) ) {
                    writer.Write((Color)value);
                    return;
                }
                if ( t == typeof(Vector2) ) {
                    writer.Write((Vector2)value);
                    return;
                }
                if ( t == typeof(Vector3) ) {
                    writer.Write((Vector3)value);
                    return;
                }
                if ( t == typeof(Vector4) ) {
                    writer.Write((Vector4)value);
                    return;
                }
                if ( t == typeof(Quaternion) ) {
                    writer.Write((Quaternion)value);
                    return;
                }
                if ( t == typeof(GameObject) ) {
                    writer.Write((GameObject)value);
                    return;
                }
                if ( typeof(Component).RTIsAssignableFrom(t) ) {
                    writer.Write((GameObject)value);
                    return;
                }
            }


            public override void Deserialize(NetworkReader reader) {

                name = reader.ReadString();

                var isNull = reader.ReadBoolean();
                if ( isNull ) {
                    value = null;
                    return;
                }

                var typeName = reader.ReadString();
                var t = System.Type.GetType(typeName);

                if ( t == typeof(string) ) {
                    value = reader.ReadString();
                    return;
                }
                if ( t == typeof(bool) ) {
                    value = reader.ReadBoolean();
                    return;
                }
                if ( t == typeof(int) ) {
                    value = reader.ReadInt32();
                    return;
                }
                if ( t == typeof(float) ) {
                    value = reader.ReadSingle();
                    return;
                }
                if ( t == typeof(Color) ) {
                    value = reader.ReadColor();
                    return;
                }
                if ( t == typeof(Vector2) ) {
                    value = reader.ReadVector2();
                    return;
                }
                if ( t == typeof(Vector3) ) {
                    value = reader.ReadVector3();
                    return;
                }
                if ( t == typeof(Vector4) ) {
                    value = reader.ReadVector4();
                    return;
                }
                if ( t == typeof(Quaternion) ) {
                    value = reader.ReadQuaternion();
                    return;
                }
                if ( t == typeof(GameObject) ) {
                    value = reader.ReadGameObject();
                    return;
                }
                if ( typeof(Component).RTIsAssignableFrom(t) ) {
                    var go = reader.ReadGameObject();
                    if ( go != null ) {
                        value = go.GetComponent(t);
                    }
                    return;
                }
            }
        }

        ///----------------------------------------------------------------------------------------------

        private const short VALUE_CHANGE = 1000;
        private const short REFRESH_VALUES = 1001;
        private List<string> waitResponseNames = new List<string>();

        public override void OnStartServer() {
            NetworkServer.RegisterHandler(VALUE_CHANGE, SyncValue);
            NetworkServer.RegisterHandler(REFRESH_VALUES, RefreshValues);
        }

        public override void OnStartClient() {

            if ( !isServer ) {
                NetworkManager.singleton.client.RegisterHandler(VALUE_CHANGE, SyncValue);
                var connectionID = NetworkManager.singleton.client.connection.connectionId;
                NetworkManager.singleton.client.Send(REFRESH_VALUES, new IntegerMessage(connectionID));
            }

            var bb = GetComponent<Blackboard>();
            foreach ( var variable in bb.variables.Values ) {
                if ( !variable.isProtected ) {
                    variable.onValueChanged += OnValueChange;
                }
            }
        }

        void OnValueChange(string name, object value) {

            if ( waitResponseNames.Contains(name) ) {
                return;
            }

            waitResponseNames.Add(name);

            if ( isServer ) {
                Debug.Log("Value Changed on Server: " + name);
                NetworkServer.SendToReady(gameObject, VALUE_CHANGE, new SyncVarMessage(name, value));
            } else {
                Debug.Log("Value Changed on Client: " + name);
                NetworkManager.singleton.client.Send(VALUE_CHANGE, new SyncVarMessage(name, value));
            }
        }

        void RefreshValues(NetworkMessage msg) {
            var id = msg.ReadMessage<IntegerMessage>().value;
            var bb = GetComponent<Blackboard>();
            foreach ( var variable in bb.variables.Values ) {
                NetworkServer.SendToClient(id, VALUE_CHANGE, new SyncVarMessage(variable.name, variable.value));
            }
        }

        void SyncValue(NetworkMessage msg) {
            var m = msg.ReadMessage<SyncVarMessage>();
            var name = m.name;
            var value = m.value;
            var bb = GetComponent<Blackboard>();
            bb.SetValue(name, value);
            waitResponseNames.Remove(name);
        }
    }
}

#endif