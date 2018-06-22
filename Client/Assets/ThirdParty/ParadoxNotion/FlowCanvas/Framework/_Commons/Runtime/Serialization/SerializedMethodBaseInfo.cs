using System.Reflection;
using UnityEngine;

namespace ParadoxNotion.Serialization {
    
	///Based class
    abstract public class SerializedMethodBaseInfo : ISerializationCallbackReceiver {
		abstract public MethodBase GetBase();
		abstract public bool HasChanged();
		abstract public string GetMethodString();
		public override string ToString(){ return GetMethodString(); }

		abstract public void OnBeforeSerialize();
		abstract public void OnAfterDeserialize();
	}
}