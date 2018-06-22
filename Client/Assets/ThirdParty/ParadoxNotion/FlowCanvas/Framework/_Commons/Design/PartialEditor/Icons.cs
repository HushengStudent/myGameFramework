#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace ParadoxNotion.Design {

    ///Common Icons Database
    public static class Icons {

		readonly public static Texture2D playIcon      = EditorGUIUtility.FindTexture("d_PlayButton");
		readonly public static Texture2D pauseIcon     = EditorGUIUtility.FindTexture("d_PauseButton");
		readonly public static Texture2D stepIcon      = EditorGUIUtility.FindTexture("d_StepButton");
		readonly public static Texture2D viewIcon      = EditorGUIUtility.FindTexture("d_ViewToolOrbit On");
		readonly public static Texture2D csIcon        = EditorGUIUtility.FindTexture("cs Script Icon");
		readonly public static Texture2D jsIcon        = EditorGUIUtility.FindTexture("Js Script Icon");
		readonly public static Texture2D tagIcon       = EditorGUIUtility.FindTexture("d_FilterByLabel");
		readonly public static Texture2D searchIcon    = EditorGUIUtility.FindTexture("Search Icon");
		readonly public static Texture2D infoIcon      = EditorGUIUtility.FindTexture("d_console.infoIcon.sml");
		readonly public static Texture2D warningIcon   = EditorGUIUtility.FindTexture("d_console.warnicon.sml");
		readonly public static Texture2D errorIcon     = EditorGUIUtility.FindTexture("d_console.erroricon.sml");
		readonly public static Texture2D redCircle     = EditorGUIUtility.FindTexture("d_winbtn_mac_close");
		readonly public static Texture2D folderIcon    = EditorGUIUtility.FindTexture("Folder Icon");
		readonly public static Texture2D favoriteIcon  = EditorGUIUtility.FindTexture("Favorite Icon");
		readonly public static Texture2D gearPopupIcon = EditorGUIUtility.FindTexture("d__Popup");
		readonly public static Texture2D gearIcon      = EditorGUIUtility.FindTexture("EditorSettings Icon");
		

		///----------------------------------------------------------------------------------------------

		///Returns a type icon
		public static Texture GetTypeIcon(System.Type type){
			return UserTypePrefs.GetTypeIcon(type);
		}
	}
}

#endif