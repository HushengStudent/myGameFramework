#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Serialization;

namespace ParadoxNotion.Design{

	///A very simple pool to handle Copy/Pasting
	public static class CopyBuffer {

		private static Dictionary<Type, object> cachedCopies = new Dictionary<Type, object>();

		///Is copy available?
		public static bool Has<T>(){
			object o;
			return (cachedCopies.TryGetValue( typeof(T), out o ) && o is T);
		}

		///Peek at copy without cloning it
		public static T Peek<T>(){
			object o = null;
			if (cachedCopies.TryGetValue( typeof(T), out o ) && o is T ){
				return (T)o;
			}
			return default(T);
		}

		///Returns true if copy exist and the copy
		public static bool TryGet<T>(out T copy){
			copy = Get<T>();
			return object.Equals(copy, default(T)) == false;
		}

		///Returns the copy
		public static T Get<T>(bool clone = false){
			object o = null;
			if (cachedCopies.TryGetValue( typeof(T), out o ) && o is T ){
				return clone? JSONSerializer.Clone<T>( (T)o ) : (T)o;
			}
			return default(T);
		}

		///Sets a copy
		public static void Set<T>(T obj, bool clone = false){
			cachedCopies[typeof(T)] = clone? JSONSerializer.Clone<T>(obj) : obj;
		}
	}
}

#endif