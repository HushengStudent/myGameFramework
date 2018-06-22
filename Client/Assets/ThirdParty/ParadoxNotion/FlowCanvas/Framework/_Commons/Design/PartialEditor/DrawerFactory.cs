#if UNITY_EDITOR

using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ParadoxNotion.Design {

    ///Provides object and attribute property drawers
    public static class DrawerFactory {

		//Type to drawer instance map
		private static Dictionary<Type, IObjectDrawer> objectDrawers = new Dictionary<Type, IObjectDrawer>();
		private static Dictionary<Type, IAttributeDrawer> attributeDrawers = new Dictionary<Type, IAttributeDrawer>();

		///Return an object drawer instance of target inspected type
		public static IObjectDrawer GetObjectDrawer(Type objectType){
			IObjectDrawer result = null;
			if (objectDrawers.TryGetValue(objectType, out result)){
				return result;
			}

			foreach(var drawerType in ReflectionTools.GetImplementationsOf(typeof(IObjectDrawer))){
				if (drawerType != typeof(DefaultObjectDrawer)){
					var args = drawerType.BaseType.GetGenericArguments();
					if (args.Length == 1 && args[0].IsAssignableFrom(objectType)){
						return objectDrawers[objectType] = Activator.CreateInstance(drawerType) as IObjectDrawer;
					}
				}
			}

			return objectDrawers[objectType] = new DefaultObjectDrawer(objectType);
		}
		
		///Return an attribute drawer instance of target attribute instance
		public static IAttributeDrawer GetAttributeDrawer(DrawerAttribute att){ return GetAttributeDrawer(att.GetType()); }
		///Return an attribute drawer instance of target attribute type
		public static IAttributeDrawer GetAttributeDrawer(Type attributeType){
			IAttributeDrawer result = null;
			if (attributeDrawers.TryGetValue(attributeType, out result)){
				return result;
			}

			foreach(var drawerType in ReflectionTools.GetImplementationsOf(typeof(IAttributeDrawer))){
				if (drawerType != typeof(DefaultAttributeDrawer)){
					var args = drawerType.BaseType.GetGenericArguments();
					if (args.Length == 1 && args[0].IsAssignableFrom(attributeType)){
						return attributeDrawers[attributeType] = Activator.CreateInstance(drawerType) as IAttributeDrawer;
					}
				}
			}

			return attributeDrawers[attributeType] = new DefaultAttributeDrawer(attributeType);
		}
	}

	///----------------------------------------------------------------------------------------------

	public interface IObjectDrawer{
		object DrawGUI(GUIContent content, object instance, FieldInfo fieldInfo, object context, DrawerAttribute[] attributes);
		object MoveNextDrawer();
	}
	
	public interface IAttributeDrawer{
		object DrawGUI(IObjectDrawer objectDrawer, GUIContent content, object instance, FieldInfo fieldInfo, object context, DrawerAttribute attribute);
	}

	///----------------------------------------------------------------------------------------------

	///Derive this to create custom drawers for T assignable object types.
	abstract public class ObjectDrawer<T> : IObjectDrawer{

		///The GUIContent
		protected GUIContent content{get; private set;}
		///The instance of the object being drawn
		protected T instance{get; private set;}
		///The reflected FieldInfo representation
		protected FieldInfo fieldInfo{get; private set;}
		///The parent object the instance is drawn within
		protected object context{get; private set;}
		///The set of Drawer Attributes found on field
		protected DrawerAttribute[] attributes{get; private set;}
		///Current attribute index drawn
		private int attributeIndex{get;set;}

		///Begin GUI
		public object DrawGUI(GUIContent content, object instance, FieldInfo fieldInfo, object context, DrawerAttribute[] attributes){
			this.content    = content;
			this.instance   = (T)instance;
			this.fieldInfo  = fieldInfo;
			this.context    = context;
			this.attributes = attributes;

			this.attributeIndex = -1;
			return MoveNextDrawer();
		}

		///Show the next attribute drawer in order, or the object drawer itself of no attribute drawer is left to show.
		public object MoveNextDrawer(){
			attributeIndex++;
			if (attributes != null && attributeIndex < attributes.Length){
				var att = attributes[attributeIndex];
				var drawer = DrawerFactory.GetAttributeDrawer(att);
				return drawer.DrawGUI(this, content, instance, fieldInfo, context, att);
			}
			return OnGUI(content, instance);
		}

		///Override to implement GUI. Return the modified instance at the end.
		abstract public T OnGUI(GUIContent content, T instance);
	}

	///The default object drawer implementation able to inspect most types
	public class DefaultObjectDrawer : ObjectDrawer<object>{
		
		private Type objectType;
		
		public DefaultObjectDrawer(Type objectType){
			this.objectType = objectType;
		}

		public override object OnGUI(GUIContent content, object instance){
			return EditorUtils.DrawEditorFieldDirect(content, instance, objectType, fieldInfo, context, attributes);
		}
	}

	///----------------------------------------------------------------------------------------------

	///Derive this to create custom drawers for T ObjectDrawerAttribute.
	abstract public class AttributeDrawer<T> : IAttributeDrawer where T:DrawerAttribute{

		///The ObjectDrawer currently in use
		protected IObjectDrawer objectDrawer{get; private set;}
		///The GUIContent
		protected GUIContent content{get; private set;}
		///The instance of the object being drawn
		protected object instance{get; private set;}
		///The reflected FieldInfo representation
		protected FieldInfo fieldInfo{get; private set;}
		///The parent object the instance is drawn within
		protected object context{get; private set;}
		///The attribute instance
		protected T attribute{get; private set;}

		///Begin GUI
		public object DrawGUI(IObjectDrawer objectDrawer, GUIContent content, object instance, FieldInfo fieldInfo, object context, DrawerAttribute attribute){
			this.objectDrawer = objectDrawer;
			this.content      = content;
			this.instance     = instance;
			this.fieldInfo    = fieldInfo;
			this.context      = context;
			this.attribute    = (T)attribute;
			return OnGUI(content, instance);
		}

		///Override to implement GUI. Return the modified instance at the end.
		abstract public object OnGUI(GUIContent content, object instance);
		///Show the next attribute drawer in order, or the object drawer itself of no attribute drawer is left to show.
		protected object MoveNextDrawer(){ return objectDrawer.MoveNextDrawer(); }
	}

	///The default attribute drawer implementation for when an actual implementation is not found
	public class DefaultAttributeDrawer : AttributeDrawer<DrawerAttribute>{
		
		private Type attributeType;

		public DefaultAttributeDrawer(Type attributeType){
			this.attributeType = attributeType;
		}

		public override object OnGUI(GUIContent content, object instance){
			GUILayout.Label(string.Format("Implementation of '{0}' attribute not found.", attributeType));
			return MoveNextDrawer();
		}
	}

}

#endif