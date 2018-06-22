using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using ParadoxNotion;

namespace FlowCanvas.Nodes {

    ///Base class for arbitrary unity object wrappers
    abstract public class CustomObjectWrapper : FlowNode {

		///Returns whether there is a CustomObjectWrapper implemented for target type
		public static System.Type[] FindCustomObjectWrappersForType(System.Type targetType){
			var results = new List<System.Type>();
			foreach(var type in ReflectionTools.GetImplementationsOf(typeof(CustomObjectWrapper))){
				var args = type.BaseType.GetGenericArguments();
				if (args.Length == 1 && args[0] == targetType ){
					results.Add(type);
				}
			}
			return results.ToArray();
		}

		///Set's the target object
		abstract public void SetTarget(UnityEngine.Object target);
	}

	[Icon(runtimeIconTypeCallback:"GetRuntimeIconType")]
	///Derive this to create custom object wrappers for any arbitrary UnityObject.
	abstract public class CustomObjectWrapper<T> : CustomObjectWrapper where T:UnityEngine.Object {

		[SerializeField]
		private T _target;
		public T target{
			get {return _target;}
			set
			{
				if (_target != value){
					_target = value;
					GatherPorts();
				}
			}
		}		

		public override string name{
			get {return target != null? target.name : base.name;}
		}

		public override void SetTarget(UnityEngine.Object target){
			if (target is T){
				this.target = (T)target;
			}
		}

		///callback used for [Icon] attribute
		protected System.Type GetRuntimeIconType(){
			return target != null? target.GetType() : null;
		}


		///----------------------------------------------------------------------------------------------
		///---------------------------------------UNITY EDITOR-------------------------------------------
		#if UNITY_EDITOR
		
		protected override void OnNodeInspectorGUI(){
			target = (T)UnityEditor.EditorGUILayout.ObjectField("Target", target, typeof(T), true);
			base.OnNodeInspectorGUI();
		}
		
		#endif
		///----------------------------------------------------------------------------------------------
	}
}