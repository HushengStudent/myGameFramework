using System;
using System.Linq;
using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Serialization.FullSerializer.Internal;

namespace ParadoxNotion.Serialization{

	///Handles missing types serialization and recovery
	public class fsRecoveryProcessor<TCanProcess, TMissing> : fsObjectProcessor where TMissing : TCanProcess, IMissingRecoverable {

		public override bool CanProcess(Type type){
			return typeof(TCanProcess).RTIsAssignableFrom(type);
		}

		public override void OnBeforeDeserialize(Type storageType, ref fsData data){

			if (!data.IsDictionary){
				return;
			}

			var json = data.AsDictionary;

			fsData typeData;
			if (json.TryGetValue("$type", out typeData)){

				//check if serialized can actually resolve the type
				var serializedType = fsTypeCache.GetType( typeData.AsString, storageType );

				//If not, handle missing serialized type
				if (serializedType == null){
					//Replace with a Missing Type
					//inject the Missing Type and store recovery serialization state.
					//recoveryState and missingType are serializable members of Missing Type.
					json["missingType"] = new fsData( typeData.AsString );
					json["recoveryState"] = new fsData( data.ToString() );
					json["$type"] = new fsData( typeof(TMissing).FullName );
				}

				//Recover possibly found serialized type
				if (serializedType == typeof(TMissing)){

					//Does the missing type now exists?
					var missingType = fsTypeCache.GetType( json["missingType"].AsString, storageType );

					//Finaly recover if we have a type
					if (missingType != null){

						var recoveryState = json["recoveryState"].AsString;
						var recoverJson = fsJsonParser.Parse(recoveryState).AsDictionary;

						//merge the recover state *ON TOP* of the current state, thus merging only Declared recovered members
						json = json.Concat( recoverJson.Where( kvp => !json.ContainsKey(kvp.Key) ) ).ToDictionary( c => c.Key, c => c.Value );
						json["$type"] = new fsData( missingType.FullName );
						data = new fsData( json );
					}
				}
			}
		}
	}
}