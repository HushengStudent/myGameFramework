using System;
using System.Linq;
using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Serialization.FullSerializer.Internal;

namespace ParadoxNotion.Serialization
{

    ///Handles missing types serialization and recovery
    public class fsRecoveryProcessor<TCanProcess, TMissing> : fsObjectProcessor where TMissing : TCanProcess, IMissingRecoverable
    {

        private const string FIELD_NAME_TYPE = "_missingType";
        private const string FIELD_NAME_STATE = "_recoveryState";

        public override bool CanProcess(Type type) {
            return typeof(TCanProcess).RTIsAssignableFrom(type);
        }

        public override void OnBeforeDeserialize(Type storageType, ref fsData data) {

            if ( !data.IsDictionary ) {
                return;
            }

            if ( JSONSerializer.applicationPlaying ) {
                return;
            }

            var json = data.AsDictionary;

            fsData typeData;
            if ( json.TryGetValue(fsSerializer.KEY_INSTANCE_TYPE, out typeData) ) {

                //check if serialized can actually resolve the type
                var serializedType = ReflectionTools.GetType(typeData.AsString, storageType);

                //If not, handle missing serialized type
                if ( serializedType == null ) {
                    //Replace with a Missing Type
                    //inject the Missing Type and store recovery serialization state.
                    //recoveryState and missingType are serializable members of Missing Type.
                    json[FIELD_NAME_TYPE] = new fsData(typeData.AsString);
                    json[FIELD_NAME_STATE] = new fsData(data.ToString());
                    json[fsSerializer.KEY_INSTANCE_TYPE] = new fsData(typeof(TMissing).FullName);
                }

                //Recover possibly found serialized type
                if ( serializedType == typeof(TMissing) ) {

                    //Does the missing type now exists?
                    var missingType = ReflectionTools.GetType(json[FIELD_NAME_TYPE].AsString, storageType);

                    //Finaly recover if we have a type
                    if ( missingType != null ) {

                        var recoveryState = json[FIELD_NAME_STATE].AsString;
                        var recoverJson = fsJsonParser.Parse(recoveryState).AsDictionary;

                        //merge the recover state *ON TOP* of the current state, thus merging only Declared recovered members
                        json = json.Concat(recoverJson.Where(kvp => !json.ContainsKey(kvp.Key))).ToDictionary(c => c.Key, c => c.Value);
                        json[fsSerializer.KEY_INSTANCE_TYPE] = new fsData(missingType.FullName);
                        data = new fsData(json);
                    }
                }
            }
        }

        public override void OnAfterDeserialize(Type storageType, object instance) {

        }
    }
}