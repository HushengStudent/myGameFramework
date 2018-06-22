using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ParadoxNotion.Serialization.FullSerializer.Internal;

namespace ParadoxNotion.Serialization{

	[Serializable]
	public class SerializedEventInfo : ISerializationCallbackReceiver {
		
		[SerializeField]
		private string _baseInfo;

		[NonSerialized]
		private EventInfo _event;

		void ISerializationCallbackReceiver.OnBeforeSerialize(){
			if (_event != null){
				_baseInfo = string.Format("{0}|{1}", _event.RTReflectedType().FullName, _event.Name);
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize(){
			if (_baseInfo == null){
				return;
			}
			var split = _baseInfo.Split('|');
			var type = fsTypeCache.GetType(split[0], null);
			if (type == null){
				_event = null;
				return;
			}
			var name = split[1];
			_event = type.RTGetEvent(name);
		}

		public SerializedEventInfo(){}
		public SerializedEventInfo(EventInfo info){
			_event = info;
		}

		public EventInfo Get(){
			return _event;
		}
	}
}