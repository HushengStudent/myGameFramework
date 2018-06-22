using System;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;
using NodeCanvas.Framework.Internal;
using UnityEngine;


namespace NodeCanvas.Framework {

	#if UNITY_EDITOR //handles missing variable types
    [fsObject(Processor = typeof(fsRecoveryProcessor<Variable, MissingVariableType>))]
	#endif

	[Serializable]
	[ParadoxNotion.Design.SpoofAOT]
	///Variables are stored in Blackboards and can optionaly be bound to Properties of a Unity Component
	abstract public class Variable {

	    [SerializeField]
		private string _name;
		[SerializeField]
		private string _id;
		[SerializeField]
		private bool _protected;

		public event Action<string> onNameChanged;
		public event Action<string, object> onValueChanged;

		///The name of the variable
		public string name{
			get {return _name;}
			set
			{
				if (_name != value){
					_name = value;
					if (onNameChanged != null){
						onNameChanged(value);
					}
				}
			}
		}

		public string ID{
			get
			{
				if (string.IsNullOrEmpty(_id)){
					_id = Guid.NewGuid().ToString();
				}
				return _id;
			}
		}

		///The value as object type when accessing from base class
		public object value{
			get {return objectValue;}
			set {objectValue = value;}
		}

		///Is the variable protected?
		public bool isProtected{
			get {return _protected;}
			set {_protected = value;}
		}


		//we need this since onValueChanged is an event and we can't check != null outside of this class
		protected bool HasValueChangeEvent(){
			return onValueChanged != null;
		}

		protected void OnValueChanged(string name, object value){
			onValueChanged(name, value);
		}

		//required
		public Variable(){}

		///The System.Object value of the contained variable
		abstract protected object objectValue{get;set;}
		///The Type this Variable holds
		abstract public Type varType{get;}
		///Returns whether or not the variable is property binded
		abstract public bool hasBinding{get;}
		///The path to the property this data is binded to. Null if none
		abstract public string propertyPath{get;set;}
		///Used to bind variable to a property
		abstract public void BindProperty(MemberInfo prop, GameObject target = null);
		///Used to un-bind variable from a property
		abstract public void UnBindProperty();
		///Called from Blackboard in Awake to Initialize the binding on specified game object
		abstract public void InitializePropertyBinding(GameObject go, bool callSetter = false);

		///Checks whether a convertion to type is possible
		public bool CanConvertTo(Type toType){ return GetGetConverter(toType) != null; }
		///Gets a Func<object> that converts the value ToType if possible. Null if not.
		public Func<object> GetGetConverter(Type toType){
			
			//normal assignment
			if (toType.RTIsAssignableFrom(varType)){
				return ()=>{ return value; };
			}

			//convertible to convertible
			if (typeof(IConvertible).RTIsAssignableFrom(toType) && typeof(IConvertible).RTIsAssignableFrom(varType)){
				return ()=> { try {return Convert.ChangeType(value, toType);} catch {return !toType.RTIsAbstract()? Activator.CreateInstance(toType) : null;} };
			}
/*
			//Unity object to bool
			if (toType == typeof(bool) && typeof(UnityEngine.Object).RTIsAssignableFrom(varType) ){
				return ()=>{ return value != null; };
			}
*/
			//gameobject to transform
			if (toType == typeof(Transform) && varType == typeof(GameObject)){
				return ()=> { return value != null? (value as GameObject).transform : null; };
			}

			//component to gameobject
			if (toType == typeof(GameObject) && typeof(Component).RTIsAssignableFrom(varType)){
				return ()=> { return value != null? (value as Component).gameObject : null; };
			}

			//gameobject to vector3
			if (toType == typeof(Vector3) && varType == typeof(GameObject)){
				return ()=>{ return value != null? (value as GameObject).transform.position : Vector3.zero; };
			}

			//transform to vector3
			if (toType == typeof(Vector3) && varType == typeof(Transform)){
				return ()=>{ return value != null? (value as Transform).position : Vector3.zero;  };
			}

			//quaternion to vector3
			if (toType == typeof(Vector3) && varType == typeof(Quaternion)){
				return ()=>{ return (Vector3)((Quaternion)value).eulerAngles; };
			}

			//vector3 to quaternion
			if (toType == typeof(Quaternion) && varType == typeof(Vector3)){
				return ()=>{ return (Quaternion)Quaternion.Euler((Vector3)value); };
			}

			//vector2 to Vector3
			if (toType == typeof(Vector3) && varType == typeof(Vector2)){
				return ()=>{ return (Vector3)(Vector2)value; };
			}

			//vector3 to Vector2
			if (toType == typeof(Vector2) && varType == typeof(Vector3)){
				return ()=>{ return (Vector2)(Vector3)value; };
			}

			return null;
		}

		///Checks whether a convertion from type is possible
		public bool CanConvertFrom(Type fromType){ return GetSetConverter(fromType) != null; }
		///Gets an Action<object> that converts the value fromType if possible. Null if not.
		public Action<object> GetSetConverter(Type fromType){
			
			//normal assignment
			if (varType.RTIsAssignableFrom(fromType)){
				return (o)=>{ value = o; };
			}

			//convertible to convertible
			if (typeof(IConvertible).RTIsAssignableFrom(varType) && typeof(IConvertible).RTIsAssignableFrom(fromType)){
				return (o)=> { try {value = Convert.ChangeType(o, varType);} catch {value = !varType.RTIsAbstract()? Activator.CreateInstance(varType) : null;} };
			}
/*
			//bool to Unity Object
			if ( typeof(UnityEngine.Object).RTIsAssignableFrom(varType) && fromType == typeof(bool) ){
				return (o)=>{ value = ((bool)o) == false? null : value; };
			}
*/
			//gameobject to transform
			if (varType == typeof(Transform) && fromType == typeof(GameObject)){
				return (o)=> { value = o != null? (o as GameObject).transform : null; };
			}

			//component to gameobject
			if (varType == typeof(GameObject) && typeof(Component).RTIsAssignableFrom(fromType)){
				return (o)=> { value = o != null? (o as Component).gameObject : null; };
			}

			//Vector3 to gameobject
			if (varType == typeof(GameObject) && fromType == typeof(Vector3)){
				return (o)=> { if (value != null) (value as GameObject).transform.position = (Vector3)o; };
			}

			//Vector3 to transform
			if (varType == typeof(Transform) && fromType == typeof(Vector3)){
				return (o)=> { if (value != null) (value as Transform).position = (Vector3)o; };
			}

			//quaternion to vector3
			if (varType == typeof(Vector3) && fromType == typeof(Quaternion)){
				return (o)=>{ value = ((Quaternion)o).eulerAngles; };
			}

			//vector3 to quaternion
			if (varType == typeof(Quaternion) && fromType == typeof(Vector3)){
				return (o)=>{ value = Quaternion.Euler( (Vector3)o ); };
			}

			//vector2 to Vector3
			if (fromType == typeof(Vector3) && varType == typeof(Vector2)){
				return (o)=>{ value = (Vector2)(Vector3)o; };
			}

			//vector3 to Vector2
			if (fromType == typeof(Vector2) && varType == typeof(Vector3)){
				return (o)=>{ value = (Vector3)(Vector2)o; };
			}

			return null;
		}

		public override string ToString(){
			return name;
		}
	}

	///----------------------------------------------------------------------------------------------

	///The actual Variable
	[Serializable]
	public class Variable<T> : Variable {

		[SerializeField]
		private T _value;
		[SerializeField]
		private string _propertyPath;

		//required
		public Variable(){}

		//delegates for property binding
		private Func<T> getter;
		private Action<T> setter;
		//

		public override string propertyPath{
			get {return _propertyPath;}
			set {_propertyPath = value;}
		}

		public override bool hasBinding{
			get {return !string.IsNullOrEmpty(_propertyPath);}
		}

		protected override object objectValue{
			get {return value;}
			set {this.value = (T)value;}
		}

		public override Type varType{
			get {return typeof(T);}
		}

		///The value as correct type when accessing as this type
		new public T value{
			get	{ return getter != null? getter() : _value; }
			set
			{
				if (base.HasValueChangeEvent()){ //check this first to avoid possible unescessary value boxing
					if (!object.Equals(_value, value)){
						this._value = value;
						if (setter != null) setter(value);
						base.OnValueChanged(name, value);
					}
					return;
				}

				if (setter != null){
					setter(value);
					return;
				}

				this._value = value;
			}
		}

		///Used to bind this to BBParameters
		public T GetValue(){ return value; }
		///Used to bind this to BBParameters
		public void SetValue(T newValue){ value = newValue; }


		///Set the property binding
		public override void BindProperty(MemberInfo prop, GameObject target = null){
			if (prop is PropertyInfo || prop is FieldInfo){
				_propertyPath = string.Format("{0}.{1}", prop.RTReflectedType().FullName, prop.Name);
				if (target != null){
					InitializePropertyBinding(target, false);
				}
			}
		}

		///Removes the property binding
		public override void UnBindProperty(){
			_propertyPath = null;
			getter = null;
			setter = null;
		}

		///Initialize the property binding for target gameobject. The gameobject is only used in case the binding is not static.
		public override void InitializePropertyBinding(GameObject go, bool callSetter = false){
		    
            if (!hasBinding || !Application.isPlaying){
                return;
            }
		    
            getter = null;
		    setter = null;

		    var idx = _propertyPath.LastIndexOf('.');
		    var typeString = _propertyPath.Substring(0, idx);
		    var memberString = _propertyPath.Substring(idx + 1);
		    var type = ReflectionTools.GetType(typeString, /*fallback?*/ true);

		    if (type == null){
		    	Debug.LogError(string.Format("Type '{0}' not found for Blackboard Variable '{1}' Binding.", typeString, name), go);
		    	return;
		    }

		    var prop = type.RTGetProperty(memberString);
		    if (prop != null){
		        var getMethod = prop.RTGetGetMethod();
		        var setMethod = prop.RTGetSetMethod();
			    var isStatic = (getMethod != null && getMethod.IsStatic) || (setMethod != null && setMethod.IsStatic);
		    	var instance = isStatic? null : go.GetComponent(type);
		    	if (instance == null && !isStatic){
					Debug.LogError(string.Format("A Blackboard Variable '{0}' is due to bind to a Component type that is missing '{1}'. Binding ignored.", name, typeString), go);
		    		return;
		    	}

			    if (prop.CanRead){
		            try {getter = getMethod.RTCreateDelegate<Func<T>>(instance);} //JIT
		            catch {getter = ()=>{ return (T)getMethod.Invoke(instance, null); };} //AOT
			    } else {
			    	getter = ()=> { Debug.LogError(string.Format("You tried to Get a Property Bound Variable '{0}', but the Bound Property '{1}' is Write Only!", name, _propertyPath), go ); return default(T); };
			    }

			    if (prop.CanWrite){
		            try {setter = setMethod.RTCreateDelegate<Action<T>>(instance);} //JIT
		            catch {setter = (o)=>{ setMethod.Invoke(instance, new object[]{o}); };} //AOT

		            if (callSetter){
		                setter(_value);
		            }
		            
			    } else {
			    	setter = (o)=> { Debug.LogError(string.Format("You tried to Set a Property Bound Variable '{0}', but the Bound Property '{1}' is Read Only!", name, _propertyPath), go ); };
			    }

			    return;
			}

		    var field = type.RTGetField(memberString);
			if (field != null){
		    	var instance = field.IsStatic? null : go.GetComponent(type);
		    	if (instance == null && !field.IsStatic){
					Debug.LogError(string.Format("A Blackboard Variable '{0}' is due to bind to a Component type that is missing '{1}'. Binding ignored", name, typeString), go);
		    		return;
		    	}
		    	if (field.IsConstant()){
		    		T value = (T)field.GetValue(instance);
		    		getter = ()=>{ return value; };
		    	} else {
					getter = ()=>{ return (T)field.GetValue(instance); };
					setter = (o)=>{ field.SetValue(instance, o); };
				}
				return;
			}

	        Debug.LogError(string.Format("A Blackboard Variable '{0}' is due to bind to a property/field named '{1}' that does not exist on type '{2}'. Binding ignored", name, memberString, type.FullName), go);
		}
	}

	///----------------------------------------------------------------------------------------------

	///This is a very special dummy class for variable header separators
	public class VariableSeperator{
		public bool isEditingName{get;set;}
	}
	
}