#if UNITY_EDITOR

using System.Reflection;
using System.Collections.Generic;

namespace ParadoxNotion.Design {

	///Factory for EditorObjectWrappers
	public static class EditorWrapperFactory{
		private static Dictionary<object, EditorObjectWrapper> cachedEditors = new Dictionary<object, EditorObjectWrapper>();
		
		///Returns a cached EditorObjectWrapepr of type T for target object
		public static T GetEditor<T>(object target) where T:EditorObjectWrapper{
			EditorObjectWrapper wrapper = null;
			if (cachedEditors.TryGetValue(target, out wrapper)){
				return (T)wrapper;
			}
			wrapper = (T)(typeof(T).CreateObject());
			wrapper.Init(target);
			return (T)(cachedEditors[target] = wrapper);
		}
	}

	///----------------------------------------------------------------------------------------------

    ///Wrapper Editor for objects
    abstract public class EditorObjectWrapper {
		///The target
		public object target{get; private set;}
		
		///Init for target
		public void Init(object target){
			this.target = target;
			OnInit();
		}

		virtual protected void OnInit(){}

		///Get a wrapped editor serialized field on target
		public EditorPropertyWrapper<T> CreatePropertyWrapper<T>(string name){
			var type = target.GetType();
			FieldInfo field = null;
			//we need to edit private fields
			while (field == null && type != null){
				field = type.RTGetField(name);
				type = type.BaseType;
			}
			if (field != null){
				var wrapper = (EditorPropertyWrapper<T>)typeof(EditorPropertyWrapper<>).MakeGenericType(typeof(T)).CreateObject();
				wrapper.Init(target, field);
				return wrapper;
			}
			return null;
		}

		///Get a wrapped editor method on target
		public EditorMethodWrapper CreateMethodWrapper(string name){
			var type = target.GetType();
			var method = type.RTGetMethod(name);
			if (method != null){
				var wrapper = new EditorMethodWrapper();
				wrapper.Init(target, method);
				return wrapper;
			}
			return null;
		}
	}

	///Wrapper Editor for objects
	public class EditorObjectWrapper<T> : EditorObjectWrapper{
		new public T target{ get { return (T)base.target; } }
	}

	///----------------------------------------------------------------------------------------------

	///An editor wrapped serialize field
	sealed public class EditorPropertyWrapper<T> {
		private object instance;
		private FieldInfo field;
		public T value{
			get
			{
				var o = field.GetValue(instance);
				return o != null? (T)o : default(T);
			}
			set
			{
				field.SetValue(instance, value);
			}
		}

		public void Init(object instance, FieldInfo field){
			this.instance = instance;
			this.field = field;
		}
	}

	///----------------------------------------------------------------------------------------------

	///An editor wrapped serialize method
	sealed public class EditorMethodWrapper {
		private object instance;
		private MethodInfo method;
		public void Invoke(params object[] args){
			method.Invoke(instance, args);
		}
		public void Init(object instance, MethodInfo method){
			this.instance = instance;
			this.method = method;
		}
	}
}

#endif