#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace ParadoxNotion.Design{

    /// Specific Editor GUIs
	partial class EditorUtils {

        private static Texture2D _tex;
        private static Texture2D tex
        {
            get
            {
                if (_tex == null){
                    _tex = new Texture2D(1, 1);
                    _tex.hideFlags = HideFlags.HideAndDontSave;
                }
                return _tex;
            }
        }

		///A cool label :-P (for headers)
		public static void CoolLabel(string text){
			GUI.skin.label.richText = true;
			GUI.color = Colors.lightOrange;
			GUILayout.Label("<b><size=14>" + text + "</size></b>");
			GUI.color = Color.white;
			GUILayout.Space(2);
		}

		///Combines the rest functions for a header style label
		public static void TitledSeparator(string title){
			GUILayout.Space(1);
			BoldSeparator();
			CoolLabel(title + " ▼");
			Separator();
		}

		///A thin separator
		public static void Separator(){
			var lastRect = GUILayoutUtility.GetLastRect();
			GUILayout.Space(7);
			GUI.color = new Color(0, 0, 0, 0.3f);
			GUI.DrawTexture(Rect.MinMaxRect(lastRect.xMin, lastRect.yMax + 4, lastRect.xMax, lastRect.yMax + 6), tex);
			GUI.color = Color.white;
		}

		///A thick separator similar to ngui. Thanks
		public static void BoldSeparator(){
			var lastRect = GUILayoutUtility.GetLastRect();
			GUILayout.Space(14);
			GUI.color = new Color(0, 0, 0, 0.3f);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 4), tex);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 1), tex);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 9, Screen.width, 1), tex);
			GUI.color = Color.white;
		}

		///Just a fancy ending for inspectors
		public static void EndOfInspector(){
			var lastRect= GUILayoutUtility.GetLastRect();
			GUILayout.Space(8);
			GUI.color = new Color(0, 0, 0, 0.4f);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 4), tex);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 4, Screen.width, 1), tex);
			GUI.color = Color.white;
		}

		///A Search Field
		public static string SearchField(string search){
			GUILayout.BeginHorizontal();
			search = EditorGUILayout.TextField(search, Styles.toolbarSearchTextField);
			if (GUILayout.Button(string.Empty, Styles.toolbarSearchCancelButton)){
				search = string.Empty;
				GUIUtility.keyboardControl = 0;
			}
			GUILayout.EndHorizontal();
			return search;
		}

		///Used just after a textfield with no prefix to show an italic transparent text inside when empty
		public static void TextFieldComment(string check, string comment = "Comments..."){
			if (string.IsNullOrEmpty(check)){
				var lastRect = GUILayoutUtility.GetLastRect();
				GUI.color = new Color(1,1,1,0.3f);
				GUI.Label(lastRect, " <i>" + comment + "</i>");
				GUI.color = Color.white;
			}
		}

        ///Editor for LayerMask
		public static LayerMask LayerMaskField(string prefix, LayerMask layerMask, params GUILayoutOption[] layoutOptions){
        	return LayerMaskField( string.IsNullOrEmpty(prefix)? GUIContent.none : new GUIContent(prefix), layerMask, layoutOptions );
        }

		///Editor for LayerMask
		public static LayerMask LayerMaskField(GUIContent content, LayerMask layerMask, params GUILayoutOption[] layoutOptions){
		    var layers = UnityEditorInternal.InternalEditorUtility.layers;
		    var layerNumbers = new List<int>();
		 
		    for (var i = 0; i < layers.Length; i++){
				layerNumbers.Add(LayerMask.NameToLayer(layers[i]));
			}
		 
		    var maskWithoutEmpty = 0;
		    for (var i = 0; i < layerNumbers.Count; i++) {
		    	if (((1 << layerNumbers[i]) & layerMask.value) > 0){
		            maskWithoutEmpty |= (1 << i);
		        }
		    }
			
			maskWithoutEmpty = UnityEditor.EditorGUILayout.MaskField(content, maskWithoutEmpty, layers, layoutOptions);

		    var mask = 0;
		    for (var i = 0; i < layerNumbers.Count; i++){
		        if ((maskWithoutEmpty & (1 << i)) > 0){
		            mask |= (1 << layerNumbers[i]);
		        }
		    }
		    layerMask.value = mask;
		    return layerMask;
		}


        ///Stores fold states
		private static readonly Dictionary<object, bool> registeredEditorFoldouts = new Dictionary<object, bool>();
		
		///Do a cached editor Foldout based on provided key object
		public static bool CachedFoldout(object key, GUIContent content){
			var foldout = false;
			registeredEditorFoldouts.TryGetValue(key, out foldout);
			foldout = EditorGUILayout.Foldout(foldout, content);
			return registeredEditorFoldouts[key] = foldout;
		}

		///An IList editor (List<T> and Arrays)
		public static IList ListEditor(GUIContent content, IList list, Type listType, FieldInfo field = null, object context = null, object[] attributes = null){

			var argType = listType.GetEnumerableElementType();
			if (argType == null){
				return list;
			}

			if (object.Equals(list, null)){
				GUILayout.Label("Null List");
				return list;
			}

			if (!CachedFoldout(list, content)){
				return list;
			}

			GUILayout.BeginVertical();
			EditorGUI.indentLevel ++;

			var options = new ReorderableListOptions();
			options.allowAdd = true;
			options.allowRemove = true;
			list = EditorUtils.ReorderableList(list, options, (i, r) =>
			{
				list[i] = ReflectedFieldInspector("Element " + i, list[i], argType, field, context, attributes);
			});

			EditorGUI.indentLevel --;
			Separator();
			GUILayout.EndVertical();
			return list;
		}

		///A IDictionary editor
		public static IDictionary DictionaryEditor(GUIContent content, IDictionary dict, Type dictType, FieldInfo field = null, object context = null, object[] attributes = null){

			var keyType = dictType.GetGenericArguments()[0];
			var valueType = dictType.GetGenericArguments()[1];

			if (object.Equals(dict, null)){
				GUILayout.Label("Null Dictionary");
				return dict;
			}

			if (!CachedFoldout(dict, content)){
				return dict;
			}

			GUILayout.BeginVertical();

			var keys = dict.Keys.Cast<object>().ToList();
			var values = dict.Values.Cast<object>().ToList();

			if (GUILayout.Button("Add Element")) {
			    if (!typeof(UnityObject).IsAssignableFrom(keyType)){
					object newKey = null;
					if (keyType == typeof(string)){
						newKey = string.Empty;
					} else {
						newKey = Activator.CreateInstance(keyType);
					}

					if (dict.Contains(newKey)){
						Debug.LogWarning(string.Format("Key '{0}' already exists in Dictionary", newKey.ToString()));
						return dict;
					}

					keys.Add(newKey);

				} else {
					Debug.LogWarning("Can't add a 'null' Dictionary Key");
					return dict;
				}

			    values.Add(valueType.IsValueType? Activator.CreateInstance(valueType) : null);
			}

		    //clear before reconstruct
			dict.Clear();

			for (var i = 0; i < keys.Count; i++){
				GUILayout.BeginHorizontal("box");
				GUILayout.Box("", GUILayout.Width(6), GUILayout.Height(35));

				GUILayout.BeginVertical();
				keys[i] = ReflectedFieldInspector("K:", keys[i], keyType, field, context, attributes);
				values[i] = ReflectedFieldInspector("V:", values[i], valueType, field, context, attributes);
				GUILayout.EndVertical();

				if (GUILayout.Button("X", GUILayout.Width(18), GUILayout.Height(34) ) ){
					keys.RemoveAt(i);
					values.RemoveAt(i);
				}				

				GUILayout.EndHorizontal();

				try {dict.Add(keys[i], values[i]);}
				catch{ Debug.Log("Dictionary Key removed due to duplicate found"); }
			}

			Separator();

			GUILayout.EndVertical();
			return dict;
		}


		///An editor field where if the component is null simply shows an object field, but if its not, shows a dropdown popup to select the specific component
		///from within the gameobject
		public static Component ComponentField(string prefix, Component comp, Type type, bool allowNone = true){
			return ComponentField( string.IsNullOrEmpty(prefix)? GUIContent.none : new GUIContent(prefix), comp, type, allowNone );
		}

		///An editor field where if the component is null simply shows an object field, but if its not, shows a dropdown popup to select the specific component
		///from within the gameobject
		public static Component ComponentField(GUIContent content, Component comp, Type type, bool allowNone = true){

			if (comp == null){
				comp = EditorGUILayout.ObjectField(content, comp, type, true, GUILayout.ExpandWidth(true)) as Component;
				return comp;
			}

			var components = comp.GetComponents(type).ToList();
			var componentNames = components.Where(c => c != null).Select(c => c.GetType().FriendlyName() + " (" + c.gameObject.name + ")" ).ToList();

			if (allowNone){
				componentNames.Add("|NONE|");
			}

			int index;
			var contentOptions = componentNames.Select( n => new GUIContent(n) ).ToArray();
			index = EditorGUILayout.Popup(content, components.IndexOf(comp), contentOptions, GUILayout.ExpandWidth(true));
			
			if (allowNone && index == componentNames.Count - 1){
				return null;
			}

			return components[index];
		}

		
		///A popup that is based on the string rather than the index
		public static string StringPopup(string selected, List<string> options, bool showWarning = true, bool allowNone = false, params GUILayoutOption[] GUIOptions){
			return StringPopup(string.Empty, selected, options, showWarning, allowNone, GUIOptions);
		}

		///A popup that is based on the string rather than the index
		public static string StringPopup(string prefix, string selected, List<string> options, bool showWarning = true, bool allowNone = false, params GUILayoutOption[] GUIOptions){
			return StringPopup( string.IsNullOrEmpty(prefix)? GUIContent.none : new GUIContent(prefix), selected, options, showWarning, allowNone,  GUIOptions);
		}

		///A popup that is based on the string rather than the index
		public static string StringPopup(GUIContent content, string selected, List<string> options, bool showWarning = true, bool allowNone = false, params GUILayoutOption[] GUIOptions){

			EditorGUILayout.BeginVertical();
			if (options.Count == 0 && showWarning){
				EditorGUILayout.HelpBox("There are no options to select for '" + content.text + "'", MessageType.Warning);
				EditorGUILayout.EndVertical();
				return null;
			}

			var index = 0;
			var copy = new List<string>(options);
			if (allowNone){
				copy.Insert(0, "|NONE|");
			}

			if (copy.Contains(selected)) index = copy.IndexOf(selected);
			else index = allowNone? 0 : -1;

			index = EditorGUILayout.Popup(content, index, copy.Select(n => new GUIContent(n)).ToArray(), GUIOptions);

			if (index == -1 || (allowNone && index == 0)){
				if (showWarning){
					if (!string.IsNullOrEmpty(selected)){
						EditorGUILayout.HelpBox("The previous selection '" + selected + "' has been deleted or changed. Please select another", MessageType.Warning);
					} else {
						EditorGUILayout.HelpBox("Please make a selection", MessageType.Warning);
					}
				}
			}

			EditorGUILayout.EndVertical();
			if (allowNone){ return index == 0? string.Empty : copy[index]; }
			return index == -1? string.Empty : copy[index];
		}

		///Generic Popup for selection of any element within a list
		public static T Popup<T>(string prefix, T selected, List<T> options, params GUILayoutOption[] GUIOptions){
			return Popup<T>( string.IsNullOrEmpty(prefix)? GUIContent.none : new GUIContent(prefix), selected, options, GUIOptions );
		}

		///Generic Popup for selection of any element within a list
		public static T Popup<T>(GUIContent content, T selected, List<T> options, params GUILayoutOption[] GUIOptions){

			var index = 0;
			if (options.Contains(selected)){
				index = options.IndexOf(selected) + 1;
			}

			var stringedOptions = new List<string>();
			if (options.Count == 0){
				stringedOptions.Add("|NONE AVAILABLE|");
			} else {
				stringedOptions.Add("|NONE|");
				stringedOptions.AddRange( options.Select(o => o != null? o.ToString() : "|NONE|") );
			}

			GUI.enabled = stringedOptions.Count > 1;
			index = EditorGUILayout.Popup(content, index, stringedOptions.Select(s => new GUIContent(s)).ToArray(), GUIOptions);
			GUI.enabled = true;
			return index == 0? default(T) : options[index - 1];
		}


		///Generic Button Popup for selection of any element within a list
		public static void ButtonPopup<T>(string prefix, T selected, List<T> options, Action<T> Callback){
			ButtonPopup<T>(string.IsNullOrEmpty(prefix)? GUIContent.none : new GUIContent(prefix), selected, options, Callback);
		}
		
		///Generic Button Popup for selection of any element within a list
		public static void ButtonPopup<T>(GUIContent content, T selected, List<T> options, Action<T> Callback){
			var buttonText = selected != null? selected.ToString() : "|NONE|";
			GUILayout.BeginHorizontal();
			if (content != null && content != GUIContent.none){
				GUILayout.Label(content, GUILayout.Width(0), GUILayout.ExpandWidth(true));
			}
			if (GUILayout.Button(buttonText, (GUIStyle)"MiniPopup", GUILayout.Width(0), GUILayout.ExpandWidth(true))){
				var menu = new GenericMenu();
				foreach(var _option in options){
					var option = _option;
					menu.AddItem(new GUIContent(option != null? option.ToString() : "|NONE|"), object.Equals(selected, option), ()=>{ Callback(option); });
				}
				menu.ShowAsContext();
			}
			GUILayout.EndHorizontal();
		}

		///Specialized Type button popup
		public static void ButtonTypePopup(string prefix, Type selected, Action<Type> Callback){
			ButtonTypePopup(string.IsNullOrEmpty(prefix)? GUIContent.none : new GUIContent(prefix), selected, Callback);
		}

		///Specialized Type button popup
		public static void ButtonTypePopup(GUIContent content, Type selected, Action<Type> Callback){
			var buttonText = selected != null? selected.FriendlyName() : "|NONE|";
			GUILayout.BeginHorizontal();
			if (content != null && content != GUIContent.none){
				GUILayout.Label(content, GUILayout.Width(0), GUILayout.ExpandWidth(true));
			}
			if (GUILayout.Button(buttonText, (GUIStyle)"MiniPopup", GUILayout.Width(0), GUILayout.ExpandWidth(true))){
				var menu = EditorUtils.GetPreferedTypesSelectionMenu(typeof(object), Callback);
				menu.ShowAsContext();
			}
			GUILayout.EndHorizontal();
		}
	}
}

#endif