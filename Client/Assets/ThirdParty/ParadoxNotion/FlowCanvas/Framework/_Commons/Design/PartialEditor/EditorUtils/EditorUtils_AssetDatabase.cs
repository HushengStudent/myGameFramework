#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ParadoxNotion.Design{

    /// AssetDatabase related utility
	partial class EditorUtils {

		///Create asset of type T and show or not the File Panel
		public static T CreateAsset<T> (bool displayFilePanel) where T : ScriptableObject {
			return (T)CreateAsset(typeof(T), displayFilePanel);
		}
		
		///Create asset of type T
		public static T CreateAsset<T> () where T : ScriptableObject {
			return (T)CreateAsset(typeof(T));
		}
		
		///Create asset of type T at target path
		public static T CreateAsset<T> (string path) where T : ScriptableObject {
			return (T)CreateAsset(typeof(T), path);
		}

		///Create asset of type and show or not the File Panel
		public static ScriptableObject CreateAsset(System.Type type, bool displayFilePanel){
			ScriptableObject asset = null;
			var path = EditorUtility.SaveFilePanelInProject (
	         	       "Create Asset of type " + type.ToString(),
	            	   	type.Name + ".asset",
	                	"asset", "");
			asset = CreateAsset(type, path);
			return asset;
		}
		
		///Create asset of type
		public static ScriptableObject CreateAsset(System.Type type){
			var asset = CreateAsset(type, GetAssetUniquePath(type.Name + ".asset") );
			return asset;
		}
		
		///Create asset of type at target path
		public static ScriptableObject CreateAsset(System.Type type, string path){
			if (string.IsNullOrEmpty(path)){
				return null;
			}
			ScriptableObject data = null;
			data = ScriptableObject.CreateInstance(type);
			AssetDatabase.CreateAsset(data, path);
			AssetDatabase.SaveAssets();
			return data;
		}

		
		///Get a unique path at current project selection for creating an asset, providing the "filename.type"
		public static string GetAssetUniquePath(string fileName){
			var path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == ""){
			    path = "Assets";
			}
			if (System.IO.Path.GetExtension(path) != ""){
			    path = path.Replace(System.IO.Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}
			return AssetDatabase.GenerateUniqueAssetPath(path + "/" + fileName);
		}

		///Opens the MonoScript of a type if existant
		public static bool OpenScriptOfType(System.Type type){
			foreach (var path in AssetDatabase.GetAllAssetPaths()){
				if (path.EndsWith(type.Name + ".cs") || path.EndsWith(type.Name + ".js")){
					var script = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
					if (type == script.GetClass()){
						AssetDatabase.OpenAsset(script);
						return true;
					}
				}
			}

			Debug.Log(string.Format("Can't open script of type '{0}', because a script with the same name does not exist.", type.FriendlyName() ));
			return false;
		}

		//Get all scene names (added in build settings)
		public static List<string> GetSceneNames(){
			var allSceneNames = new List<string>();
			foreach (var scene in EditorBuildSettings.scenes){
				if (scene.enabled){
					var name = scene.path.Substring(scene.path.LastIndexOf("/") + 1);
					name = name.Substring(0, name.Length - 6);
					allSceneNames.Add(name);
				}
			}

			return allSceneNames;
		}

	}
}


#endif