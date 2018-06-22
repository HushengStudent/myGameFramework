#if UNITY_EDITOR

using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Serialization;
using UnityEditor;
using UnityEngine;
using System.Linq;


namespace ParadoxNotion.Design{

	///An editor for preferred types
	public class PreferedTypesEditorWindow : EditorWindow {

		private List<System.Type> typeList;
		private List<System.Type> alltypes;
		private Vector2 scrollPos;

		///Open window
		public static void ShowWindow(){
			var window = GetWindow<PreferedTypesEditorWindow>();
			window.Show();
		}

		//...
		void OnEnable(){
	        titleContent = new GUIContent("Preferred Types");
			typeList = UserTypePrefs.GetPreferedTypesList();
			alltypes = ReflectionTools.GetAllTypes(true).Where( t => !t.IsGenericType && !t.IsGenericTypeDefinition).ToList();
		}

		//...
		void OnGUI(){

			GUI.skin.label.richText = true;
			EditorGUILayout.HelpBox("Here you can specify frequently used types for your project and for easier access wherever you need to select a type, like for example when you create a new blackboard variable or using any refelection based actions.\nFurthermore, it is essential when working with AOT platforms like iOS or WebGL, that you generate an AOT Classes and link.xml files with the relevant button bellow.\nTo add types in the list quicker, you can also Drag&Drop an object, or a Script file in this editor window.", MessageType.Info);

			if (GUILayout.Button("Add New Type")){
				GenericMenu.MenuFunction2 Selected = delegate(object o){
					if (o is System.Type){
						AddType( (System.Type)o );
					}
					if (o is string){ //namespace
						foreach(var type in alltypes){
							if ( type.Namespace == (string)o ){
								AddType(type);
							}
						}
					}
				};	

				var menu = new GenericMenu();
				var namespaces = new List<string>();
				menu.AddItem(new GUIContent("Classes/System/Object"), false, Selected, typeof(object));
				foreach(var t in alltypes){
					var friendlyName = (string.IsNullOrEmpty(t.Namespace)? "No Namespace/" : t.Namespace.Replace(".", "/") + "/") + t.FriendlyName();
					var category = "Classes/";
					if (t.IsValueType) category = "Structs/";
					if (t.IsInterface) category = "Interfaces/";
					if (t.IsEnum) category = "Enumerations/";
					var icon = UserTypePrefs.GetTypeIcon(t);
					menu.AddItem(new GUIContent( category + friendlyName, icon), typeList.Contains(t), Selected, t);
					if (t.Namespace != null && !namespaces.Contains(t.Namespace)){
						namespaces.Add(t.Namespace);
					}
				}

				menu.AddSeparator("/");
				foreach(var ns in namespaces){
					var path = "Whole Namespaces/" + ns.Replace(".", "/") + "/Add " + ns;
					menu.AddItem( new GUIContent(path), false, Selected, ns );
				}

				menu.ShowAsBrowser("Add Preferred Type");
			}


			if (GUILayout.Button("Generate AOTClasses.cs and link.xml Files")){
				if (EditorUtility.DisplayDialog("Generate AOT Classes", "A script relevant to AOT compatibility for certain platforms will now be generated.", "OK")){
					var path = EditorUtility.SaveFilePanelInProject ("AOT Classes File", "AOTClasses", "cs", "");
		            if (!string.IsNullOrEmpty(path)){
						AOTClassesGenerator.GenerateAOTClasses( path, UserTypePrefs.GetPreferedTypesList(true).ToArray() );
		            }
				}

				if (EditorUtility.DisplayDialog("Generate link.xml File", "A file relevant to 'code stripping' for platforms that have code stripping enabled will now be generated.", "OK")){
					var path = EditorUtility.SaveFilePanelInProject ("AOT link.xml", "link", "xml", "");
		            if (!string.IsNullOrEmpty(path)){
						AOTClassesGenerator.GenerateLinkXML( path, UserTypePrefs.GetPreferedTypesList().ToArray() );
		            }
				}

                AssetDatabase.Refresh();
			}

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("RESET DEFAULTS")){
				if (EditorUtility.DisplayDialog("Reset Preferred Types", "Are you sure?", "Yes", "NO!")){
					UserTypePrefs.ResetTypeConfiguration();
					typeList = UserTypePrefs.GetPreferedTypesList();
					Save();
				}
			}

			if (GUILayout.Button("Save Preset")){
				var path = EditorUtility.SaveFilePanelInProject ("Save Types Preset", "", "typePrefs", "");
	            if (!string.IsNullOrEmpty(path)){
	                System.IO.File.WriteAllText( path, JSONSerializer.Serialize(typeof(List<System.Type>), typeList, true) );
	                AssetDatabase.Refresh();
	            }		
			}

			if (GUILayout.Button("Load Preset")){
	            var path = EditorUtility.OpenFilePanel("Load Types Preset", "Assets", "typePrefs");
	            if (!string.IsNullOrEmpty(path)){
	                var json = System.IO.File.ReadAllText(path);
	                typeList = JSONSerializer.Deserialize<List<System.Type>>(json);
	                Save();
	            }	
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(5);

			scrollPos = GUILayout.BeginScrollView(scrollPos);
			for (int i = 0; i < typeList.Count; i++){
				GUILayout.BeginHorizontal("box");
				var type = typeList[i];
				if (type == null){
					GUILayout.Label("MISSING TYPE", GUILayout.Width(300));
					GUILayout.Label("---");
				} else {
					var name = type.FriendlyName();
					var icon = UserTypePrefs.GetTypeIcon(type);
					var color = icon != null && icon.name == UserTypePrefs.DEFAULT_TYPE_ICON_NAME? UserTypePrefs.GetTypeColor(type) : Color.white;
					GUI.color = color;
					GUILayout.Label(icon, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color = Color.white;
					GUILayout.Label(name, GUILayout.Width(300));
					GUILayout.Label(type.Namespace);
				}
				if (GUILayout.Button("X", GUILayout.Width(18))){
					RemoveType(type);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();

			AcceptDrops();
			Repaint();
		}


 		//Handles Drag&Drop operations
		void AcceptDrops(){
			var e = Event.current;
			if (e.type == EventType.DragUpdated){
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
			}

			if (e.type == EventType.DragPerform){
				DragAndDrop.AcceptDrag();

				foreach(var o in DragAndDrop.objectReferences){
					
					if (o == null){
						continue;
					}

					if (o is MonoScript){
						var type = (o as MonoScript).GetClass();
						if (type != null){
							AddType(type);
						}
						continue;
					}

					AddType(o.GetType());
				}
			}
		}

		///Add a type
		void AddType(System.Type t){
			if (!typeList.Contains(t)){
				typeList.Add(t);
				Save();
				ShowNotification(new GUIContent(string.Format("Type '{0}' Added!", t.FriendlyName()) ));
				return;
			}

			ShowNotification(new GUIContent(string.Format("Type '{0}' is already in the list.", t.FriendlyName()) ) );
		}

		///Remove a type
		void RemoveType(System.Type t){
			typeList.Remove(t);
			Save();
			ShowNotification(new GUIContent(string.Format("Type '{0}' Removed.", t.FriendlyName()) ));
		}

		///Save changes
		void Save(){
			UserTypePrefs.SetPreferedTypesList(typeList);
		}
	}
}

#endif