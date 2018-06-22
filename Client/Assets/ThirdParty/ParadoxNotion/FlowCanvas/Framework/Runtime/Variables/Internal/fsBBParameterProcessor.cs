using System;
using ParadoxNotion;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;

namespace NodeCanvas.Framework.Internal {

    ///Extended functionality for BBParameters, so that changing a serialized type T, to BBParameter<T>, retains the original serialization.
    ///As such "upgrading" a normal T to BBParameter<T> impose no problems.
    public class fsBBParameterProcessor : fsRecoveryProcessor<BBParameter, MissingBBParameterType> {

		//...
		public override void OnBeforeDeserializeAfterInstanceCreation(Type storageType, object instance, ref fsData data){

			//Check if the previous state is already an object and whether it's empty (default values), or it contains certain BBParameter fields.
			//This way avoid extra work if previous state is already a BBParameter.
			//There is no other way to find out if the previous state is BBParameter or not. It's no bullet proof, but not harmfull anyway.
			if (data.IsDictionary){
				var dict = data.AsDictionary;
				if ( dict.Count == 0 || dict.ContainsKey("_value") || dict.ContainsKey("_name") ){
					return;
				}
			}

			var bbParam = instance as BBParameter;
			if (bbParam != null && bbParam.GetType().RTIsGenericType()){
				var varType = bbParam.varType;
				var serializer = new fsSerializer();
				object prevInstance = null;
				//try deserialize previous json state to current BBParameter T type.
				if (serializer.TryDeserialize(data, varType, ref prevInstance).Succeeded){
					//if success and assignalbes, set the BBParameter instance value and serialize back again.
					if (prevInstance != null && varType.RTIsAssignableFrom(prevInstance.GetType())){
						bbParam.value = prevInstance;
						serializer.TrySerialize(storageType, instance, out data);
					}
				}
			}
		}
	}
}