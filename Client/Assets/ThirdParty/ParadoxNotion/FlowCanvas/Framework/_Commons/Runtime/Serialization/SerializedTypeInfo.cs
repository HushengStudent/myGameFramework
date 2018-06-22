using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ParadoxNotion.Serialization.FullSerializer.Internal;

namespace ParadoxNotion.Serialization{

	[Serializable]
	public class SerializedTypeInfo : ISerializationCallbackReceiver {
		
		[SerializeField]
		private string _baseInfo;

		[NonSerialized]
		private Type _type;

		void ISerializationCallbackReceiver.OnBeforeSerialize(){
			if (_type != null){
				_baseInfo = _type.FullName;
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize(){
			if (_baseInfo == null){
				return;
			}
			_type = fsTypeCache.GetType(_baseInfo, null);
		}

		public SerializedTypeInfo(){}
		public SerializedTypeInfo(Type info){
			_type = info;
		}

		public Type Get(){
			return _type;
		}
	}
}