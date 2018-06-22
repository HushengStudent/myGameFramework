#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace ParadoxNotion.Design{

    /// ContextMenus, mostly reflection ones
	partial class EditorUtils {

		///A generic purpose menu to pick an item
		public static GenericMenu GetMenu<T>(List<T> options, T current, Action<T> callback){
			var menu = new GenericMenu();
			foreach(var _option in options){
				var option = _option;
				var label = option != null? option.ToString(): "null";
				menu.AddItem(new GUIContent(label), object.Equals(current, option), ()=>{ callback(option); });
			}
			return menu;
		}

		//A generic menu selection
		public static void ShowMenu<T>(List<T> options, Action<T> callback){
			GetMenu<T>(options, default(T), callback).ShowAsContext();
		}

		///Get a selection menu of types deriving base type
		public static GenericMenu GetTypeSelectionMenu(Type baseType, Action<Type> callback, GenericMenu menu = null, string subCategory = null){

			if (menu == null){
				menu = new GenericMenu();
			}

			if (subCategory != null){
				subCategory = subCategory + "/";
			}

			GenericMenu.MenuFunction2 Selected = delegate(object selectedType){
				callback((Type)selectedType);
			};

			var scriptInfos = GetScriptInfosOfType(baseType);

			foreach (var info in scriptInfos.Where(info => string.IsNullOrEmpty(info.category))) {
			    menu.AddItem(new GUIContent(subCategory + info.name, info.icon, info.description), false, info.type != null? Selected : null, info.type);
			}

			//menu.AddSeparator("/");

			foreach (var info in scriptInfos.Where(info => !string.IsNullOrEmpty(info.category))) {
			    menu.AddItem(new GUIContent(subCategory + info.category + "/" + info.name, info.icon, info.description), false, info.type != null? Selected : null, info.type);
			}

			return menu;
		}


		/// !* Providing an open GenericTypeDefinition for 'baseType', wraps the Preferred Types wihin the 1st Generic Argument of that Definition *!
		public static GenericMenu GetPreferedTypesSelectionMenu(Type baseType, Action<Type> callback, GenericMenu menu = null, string subCategory = null, bool showAddTypeOption = false){
			
			if (menu == null){
				menu = new GenericMenu();
			}

			if (subCategory != null){
				subCategory = subCategory + "/";
			}

			var constrainType = baseType;
			var isGeneric = baseType.IsGenericTypeDefinition && baseType.GetGenericArguments().Length == 1;
			var genericDefinition = isGeneric? baseType : null;
			if (isGeneric){
				var arg1 = genericDefinition.GetGenericArguments().First();
				var constrains = arg1.GetGenericParameterConstraints();
				constrainType = constrains.Length == 0? typeof(object) : constrains.First();
			}

			GenericMenu.MenuFunction2 Selected = delegate(object t){
				callback((Type)t);
			};							

			var listTypes = new Dictionary<Type, string>();
			var dictTypes = new Dictionary<Type, string>();

			foreach (var t in UserTypePrefs.GetPreferedTypesList(constrainType, true)){
				var nsString = t.NamespaceToPath() + "/";
				var finalType = isGeneric? genericDefinition.MakeGenericType(t) : t;
				var finalString = nsString + finalType.FriendlyName();
				menu.AddItem(new GUIContent(subCategory + finalString), false, Selected, finalType);

				var listType = typeof(List<>).MakeGenericType( t );
				var finalListType = isGeneric? genericDefinition.MakeGenericType(listType) : listType;
				if (constrainType.IsAssignableFrom(finalListType)){
					listTypes[ finalListType ] = nsString;
				}

				var dictType = typeof(Dictionary<,>).MakeGenericType( typeof(string), t );
				var finalDictType = isGeneric? genericDefinition.MakeGenericType(dictType) : dictType;
				if (constrainType.IsAssignableFrom(finalDictType)){
					dictTypes[ finalDictType ] = nsString;
				}
			}

			foreach(var tPair in listTypes){
				menu.AddItem(new GUIContent(subCategory + UserTypePrefs.LIST_MENU_STRING + tPair.Value + tPair.Key.FriendlyName()), false, Selected, tPair.Key);
			}

			foreach(var tPair in dictTypes){
				menu.AddItem(new GUIContent(subCategory + UserTypePrefs.DICT_MENU_STRING + tPair.Value + tPair.Key.FriendlyName()), false, Selected, tPair.Key);
			}

			if (showAddTypeOption){
				menu.AddItem(new GUIContent(subCategory + "Add Type..."), false, ()=>{ PreferedTypesEditorWindow.ShowWindow(); });
			}

			return menu;
		}

		//...
		public static void ShowPreferedTypesSelectionMenu(Type type, Action<Type> callback){
			GetPreferedTypesSelectionMenu(type, callback).ShowAsContext();
		}

		///----------------------------------------------------------------------------------------------

		public static GenericMenu GetInstanceFieldSelectionMenu(Type type, Type fieldType, Action<FieldInfo> callback, GenericMenu menu = null, string subMenu = null){
			return Internal_GetFieldSelectionMenu(BindingFlags.Public | BindingFlags.Instance, type, fieldType, callback, menu, subMenu);
		}

		public static GenericMenu GetStaticFieldSelectionMenu(Type type, Type fieldType, Action<FieldInfo> callback, GenericMenu menu = null, string subMenu = null){
			return Internal_GetFieldSelectionMenu(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, type, fieldType, callback, menu, subMenu);
		}

		///Get a GenericMenu for field selection in a type
		static GenericMenu Internal_GetFieldSelectionMenu(BindingFlags flags, Type type, Type fieldType, Action<FieldInfo> callback, GenericMenu menu = null, string subMenu = null){
			
			if (menu == null){
				menu = new GenericMenu();
			}

			if (subMenu != null){
				subMenu = subMenu + "/";
			}

			GenericMenu.MenuFunction2 Selected = delegate(object selectedField){
				callback((FieldInfo)selectedField);
			};

			var itemAdded = false;
			var more = false;
			foreach (var field in type.GetFields(flags).Where(field => fieldType.IsAssignableFrom(field.FieldType))) {

				if (field.DeclaringType != type){
					more = true;
				}

				var category = more? subMenu + type.FriendlyName() + "/More" : subMenu + type.FriendlyName();
				var finalField = field.GetBaseDefinition();
			    menu.AddItem(new GUIContent(string.Format("{0}/{1} : {2}", category, finalField.Name, finalField.FieldType.FriendlyName())), false, Selected, finalField);
			    itemAdded = true;
			}

			if (!itemAdded){
				menu.AddDisabledItem(new GUIContent(subMenu + type.FriendlyName()));
			}

			return menu;
		}


		///----------------------------------------------------------------------------------------------

		public static GenericMenu GetInstancePropertySelectionMenu(Type type, Type propType, Action<PropertyInfo> callback, bool mustRead = true, bool mustWrite = true, GenericMenu menu = null, string subMenu = null){
			return Internal_GetPropertySelectionMenu(BindingFlags.Public | BindingFlags.Instance, type, propType, callback, mustRead, mustWrite, menu, subMenu);
		}

		public static GenericMenu GetStaticPropertySelectionMenu(Type type, Type propType, Action<PropertyInfo> callback, bool mustRead = true, bool mustWrite = true, GenericMenu menu = null, string subMenu = null){
			return Internal_GetPropertySelectionMenu(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, type, propType, callback, mustRead, mustWrite, menu, subMenu);
		}

		///Get a GenericMenu for properties of a type optionaly specifying mustRead & mustWrite
		static GenericMenu Internal_GetPropertySelectionMenu(BindingFlags flags, Type type, Type propType, Action<PropertyInfo> callback, bool mustRead = true, bool mustWrite = true, GenericMenu menu = null, string subMenu = null){
			
			if (menu == null){
				menu = new GenericMenu();
			}

			if (subMenu != null){
				subMenu = subMenu + "/";
			}

			GenericMenu.MenuFunction2 Selected = delegate(object selectedProperty){
				callback((PropertyInfo)selectedProperty);
			};

			var itemAdded = false;
			var more = false;
			foreach (var prop in type.GetProperties(flags)){

				if (!prop.CanRead && mustRead){
					continue;
				}

				if (!prop.CanWrite && mustWrite){
					continue;
				}

				if (!propType.IsAssignableFrom(prop.PropertyType)){
					continue;
				}

				if (prop.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).FirstOrDefault() != null){
					continue;
				}

				if (prop.DeclaringType != type){
					more = true;
				}

				var category = more? subMenu + type.FriendlyName() + "/More" : subMenu + type.FriendlyName();
				var finalProp = prop.GetBaseDefinition();
				menu.AddItem( new GUIContent( string.Format("{0}/{1} : {2}", category, finalProp.Name, finalProp.PropertyType.FriendlyName())), false, Selected, finalProp );
				itemAdded = true;
			}

			if (!itemAdded){
				menu.AddDisabledItem(new GUIContent(subMenu + type.FriendlyName()));
			}

			return menu;
		}

		///----------------------------------------------------------------------------------------------

		///Get a menu for instance methods
		public static GenericMenu GetInstanceMethodSelectionMenu(Type type, Type returnType, Type acceptedParamsType, System.Action<MethodInfo> callback, int maxParameters, bool propertiesOnly, bool excludeVoid = false, GenericMenu menu = null, string subMenu = null){
			return Internal_GetMethodSelectionMenu(BindingFlags.Public | BindingFlags.Instance, type, returnType, acceptedParamsType, callback, maxParameters, propertiesOnly, excludeVoid, menu, subMenu);
		}

		///Get a menu for static methods
		public static GenericMenu GetStaticMethodSelectionMenu(Type type, Type returnType, Type acceptedParamsType, System.Action<MethodInfo> callback, int maxParameters, bool propertiesOnly, bool excludeVoid = false, GenericMenu menu = null, string subMenu = null){
			return Internal_GetMethodSelectionMenu(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, type, returnType, acceptedParamsType, callback, maxParameters, propertiesOnly, excludeVoid, menu, subMenu);
		}

		///Get a GenericMenu for method or property get/set methods selection in a type
		static GenericMenu Internal_GetMethodSelectionMenu(BindingFlags flags, Type type, Type returnType, Type acceptedParamsType, System.Action<MethodInfo> callback, int maxParameters, bool propertiesOnly, bool excludeVoid = false, GenericMenu menu = null, string subMenu = null){

			if (menu == null){
				menu = new GenericMenu();
			}

			if (subMenu != null){
				subMenu = subMenu + "/";
			}

			GenericMenu.MenuFunction2 Selected = delegate(object selectedMethod){
				callback((MethodInfo)selectedMethod);
			};

			var itemAdded = false;
			var more = false;
			foreach (var method in type.GetMethods(flags)){

				if (propertiesOnly != method.IsSpecialName){
					continue;
				}

				if (method.IsGenericMethod){
					continue;
				}

				if (!returnType.IsAssignableFrom(method.ReturnType)){
					continue;
				}

				if (method.ReturnType == typeof(void) && excludeVoid){
					continue;
				}

				var parameters = method.GetParameters();
				if (parameters.Length > maxParameters && maxParameters != -1){
					continue;
				}

				if (parameters.Length > 0){
					if ( parameters.Any(param => !acceptedParamsType.IsAssignableFrom(param.ParameterType)) ) {
						continue;
					}
				}

				
				MemberInfo member = method;
	            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")){
	                member = method.DeclaringType.GetProperty(method.Name.Replace("get_", "").Replace("set_", "") );
	            }
	            if (member != null && member.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).FirstOrDefault() != null){
	            	continue;
	            }

				if (method.DeclaringType != type){
					more = true;
				}

				var category = more? subMenu + type.FriendlyName() + "/More" : subMenu + type.FriendlyName();
				var finalMethod = method.GetBaseDefinition();
				menu.AddItem(new GUIContent( category + "/" + finalMethod.SignatureName()), false, Selected, finalMethod);
				itemAdded = true;
			}
			
			if (!itemAdded){
				menu.AddDisabledItem(new GUIContent(subMenu + type.FriendlyName()) );
			}

			return menu;
		}

		///----------------------------------------------------------------------------------------------

		///Get a GenericMenu for Instance Events of the type and only event handler type of System.Action
		public static GenericMenu GetInstanceEventSelectionMenu(Type type, Type argType, Action<EventInfo> callback, GenericMenu menu = null, string subMenu = null){
			return Internal_GetEventSelectionMenu(BindingFlags.Public | BindingFlags.Instance, type, argType, callback, menu, subMenu);
		}

		///Get a GenericMenu for Static Events of the type and only event handler type of System.Action
		public static GenericMenu GetStaticEventSelectionMenu(Type type, Type argType, Action<EventInfo> callback, GenericMenu menu = null, string subMenu = null){
			return Internal_GetEventSelectionMenu(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, type, argType, callback, menu, subMenu);
		}

		///Get a GenericMenu for Events of the type and only event handler type of System.Action
		static GenericMenu Internal_GetEventSelectionMenu(BindingFlags flags, Type type, Type argType, Action<EventInfo> callback, GenericMenu menu = null, string subMenu = null){

			if (menu == null){
				menu = new GenericMenu();
			}
			
			if (subMenu != null){
				subMenu = subMenu + "/";
			}
			
			GenericMenu.MenuFunction2 Selected = delegate(object selectedEvent){
				callback((EventInfo)selectedEvent);
			};
			
			var itemAdded = false;
			var eventType = argType == null? typeof(System.Action) : typeof(System.Action<>).MakeGenericType(new Type[]{argType});
			foreach (var e in type.GetEvents(flags)){
				if (e.EventHandlerType == eventType){
					var eventInfoString = string.Format("{0}({1})", e.Name, argType != null? argType.FriendlyName() : "");
					menu.AddItem(new GUIContent(subMenu + type.FriendlyName() + "/" + eventInfoString), false, Selected, e);
					itemAdded = true;					
				}
			}

			if (!itemAdded){
				menu.AddDisabledItem(new GUIContent(subMenu + type.FriendlyName()) );
			}
			
			return menu;
		}

		///----------------------------------------------------------------------------------------------

		///Shows a GenericMenu for fields of all components of a game object
		public static void ShowGameObjectFieldSelectionMenu(GameObject go, Type fieldType, System.Action<FieldInfo> callback){
			var menu = new GenericMenu();
			foreach (var comp in go.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ){
				menu = GetInstanceFieldSelectionMenu(comp.GetType(), fieldType, callback, menu);
			}
			menu.ShowAsContext();
			Event.current.Use();
		}

		///Shows a GenericMenu for properties of all components of a game object
		public static void ShowGameObjectPropertySelectionMenu(GameObject go, Type propType, Action<PropertyInfo> callback, bool mustRead = true, bool mustWrite = true){
			var menu = new GenericMenu();
			foreach (var comp in go.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ){
				menu = GetInstancePropertySelectionMenu(comp.GetType(), propType, callback, mustRead, mustWrite, menu);
			}
			menu.ShowAsContext();
			Event.current.Use();
		}

		///Shows a GenericMenu for methods of all components of a game object
		public static void ShowGameObjectMethodSelectionMenu(GameObject go, Type returnType, Type paramsType, System.Action<MethodInfo> callback, int maxParameters, bool propertiesOnly, bool excludeVoid = false){
			var menu = new GenericMenu();
			foreach (var comp in go.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ){
				menu = GetInstanceMethodSelectionMenu(comp.GetType(), returnType, paramsType, callback, maxParameters, propertiesOnly, excludeVoid, menu);
			}
			menu.ShowAsContext();
			Event.current.Use();
		}


		///Show an Event selection menu for all components on a game object
		public static void ShowGameObjectEventSelectionMenu(GameObject go, Type argType, System.Action<EventInfo> callback){
			var menu = new GenericMenu();
			foreach(var comp in go.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ){
				menu = GetInstanceEventSelectionMenu(comp.GetType(), argType, callback, menu);
			}
			menu.ShowAsContext();
			Event.current.Use();
		}

		///----------------------------------------------------------------------------------------------


		///MenuItemInfo exposition
		public class MenuItemInfo{
			public GUIContent content;
			public bool separator;
			public bool selected;
			public GenericMenu.MenuFunction func;
			public GenericMenu.MenuFunction2 func2;
			public object userData;
			public MenuItemInfo(GUIContent c, bool sep, bool slc, GenericMenu.MenuFunction f1, GenericMenu.MenuFunction2 f2, object o){
				content = c;
				separator = sep;
				selected = slc;
				func = f1;
				func2 = f2;
				userData = o;
			}
		}

		///Gets an array of MenuItemInfo out of the GenericMenu provided
		public static MenuItemInfo[] GetMenuItems(GenericMenu menu){

			var result = new List<MenuItemInfo>();
			var field = typeof(GenericMenu).GetField("menuItems", BindingFlags.Instance | BindingFlags.NonPublic);
			var items = field.GetValue(menu) as ArrayList;

			foreach (var item in items){
				var type = item.GetType();
				var content = type.GetField("content").GetValue(item) as GUIContent;
				var separator = (bool)type.GetField("separator").GetValue(item);
				var selected = (bool)type.GetField("on").GetValue(item);
				var func1 = type.GetField("func").GetValue(item) as GenericMenu.MenuFunction;
				var func2 = type.GetField("func2").GetValue(item) as GenericMenu.MenuFunction2;
				var userData = type.GetField("userData").GetValue(item);
				result.Add( new MenuItemInfo( content, separator, selected, func1, func2, userData ) );
			}
			
			return result.ToArray();
		}

		///Shows the Generic Menu as a browser with CompleteContextMenu.
		public static void ShowAsBrowser(this GenericMenu menu, Vector2 pos, string title, System.Type keyType = null){
			if (menu != null){ GenericMenuBrowser.Show(menu, pos, title, keyType); }
		}

		///Shows the Generic Menu as a browser with CompleteContextMenu.
		public static void ShowAsBrowser(this GenericMenu menu, string title, System.Type keyType = null){
			if (menu != null){ GenericMenuBrowser.Show(menu, Event.current.mousePosition, title, keyType); }
		}

		///Shortcut
		public static void Show(this GenericMenu menu, bool asBrowser, string title, System.Type keyType = null){
			if (asBrowser){ menu.ShowAsBrowser(title, keyType); }
			else { menu.ShowAsContext(); Event.current.Use(); }
		}
	}
}

#endif