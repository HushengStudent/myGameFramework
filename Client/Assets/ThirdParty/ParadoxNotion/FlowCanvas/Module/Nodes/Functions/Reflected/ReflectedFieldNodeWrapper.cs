using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using System.Reflection;
using System;
using FlowCanvas.Nodes.Legacy;

namespace FlowCanvas.Nodes{

    [DoNotList]
	[Icon(runtimeIconTypeCallback:"GetRuntimeIconType")]
	public class ReflectedFieldNodeWrapper : FlowNode {

		System.Type GetRuntimeIconType(){
			return field != null? field.DeclaringType : null;
		}

		public enum AccessMode{
			GetField,
			SetField
		}

		[SerializeField]
		private string fieldName;
		[SerializeField]
		private Type targetType;
		[SerializeField]
		private AccessMode accessMode;

        private BaseReflectedFieldNode reflectedFieldNode{get;set;}

		private FieldInfo _field;
		private FieldInfo field {
			get
			{
				if (_field != null){ return _field; }
				return _field = targetType != null? targetType.GetField(fieldName) : null;
			}
		}

		public override string name {
			get
			{
				if (field != null){
					var isGet = accessMode == AccessMode.GetField;
					var isStatic = field.IsStatic;
					var isConstant = field.IsConstant();
					if (isConstant){
						return string.Format("{0}.{1}", field.DeclaringType.FriendlyName(), field.Name);
					}
					if (isStatic){
						return string.Format("{0} {1}.{2}", (isGet? "Get" : "Set"), field.DeclaringType.FriendlyName(), field.Name);
					}
					return string.Format("{0} {1}", (isGet? "Get" : "Set"), field.Name );
				}
				return string.Format("* Missing '{0}.{1}' *", targetType != null? targetType.Name : "null", fieldName);
			}
		}

#if UNITY_EDITOR
		public override string description {
			get {return field != null? DocsByReflection.GetMemberSummary(field) : "Missing Field"; }
		}
#endif

		public void SetField(FieldInfo newField, AccessMode mode, object instance = null){
			
			if (newField == null){
				return;
			}

			newField = newField.GetBaseDefinition();

			fieldName = newField.Name;
			targetType = newField.DeclaringType;
			accessMode = mode;
			GatherPorts();

			if (instance != null && !newField.IsStatic){
				var port = (ValueInput)GetFirstInputOfType(instance.GetType());
				if (port != null){
					port.serializedValue = instance;
				}			
			}
		}


		//new reflection
		protected override void RegisterPorts(){

			if (field == null){
				return;
			}

		    reflectedFieldNode = BaseReflectedFieldNode.GetFieldNode(field);
            if (reflectedFieldNode != null){
				reflectedFieldNode.RegisterPorts(this, accessMode);
			}
        }


		///----------------------------------------------------------------------------------------------
		///---------------------------------------UNITY EDITOR-------------------------------------------
		#if UNITY_EDITOR
		
	    protected override void OnNodeInspectorGUI(){
	        if (field != null && !field.IsReadOnly()){
	            var newMode = (AccessMode)UnityEditor.EditorGUILayout.EnumPopup(accessMode);
	            if (accessMode != newMode){
	                SetField(field, newMode);
	            }
	        }

	        if (field == null && !string.IsNullOrEmpty(fieldName)){
	            GUILayout.Label(string.Format("* Missing '{0}.{1}' *", targetType != null ? targetType.Name : "null", fieldName));
	        }

	        base.OnNodeInspectorGUI();
	    }
		
		#endif
		///----------------------------------------------------------------------------------------------

    }
}