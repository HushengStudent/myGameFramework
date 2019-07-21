#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace ParadoxNotion.Design
{

    ///Common Icons Database
	[InitializeOnLoad]
    public static class Icons
    {

        static Icons() { Load(); }

        [InitializeOnLoadMethod]
        static void Load() {
            playIcon = EditorGUIUtility.FindTexture("d_PlayButton");
            pauseIcon = EditorGUIUtility.FindTexture("d_PauseButton");
            stepIcon = EditorGUIUtility.FindTexture("d_StepButton");
            viewIcon = EditorGUIUtility.FindTexture("d_ViewToolOrbit On");
            csIcon = EditorGUIUtility.FindTexture("cs Script Icon");
            tagIcon = EditorGUIUtility.FindTexture("d_FilterByLabel");
            searchIcon = EditorGUIUtility.FindTexture("Search Icon");
            infoIcon = EditorGUIUtility.FindTexture("d_console.infoIcon.sml");
            warningIcon = EditorGUIUtility.FindTexture("d_console.warnicon.sml");
            errorIcon = EditorGUIUtility.FindTexture("d_console.erroricon.sml");
            redCircle = EditorGUIUtility.FindTexture("d_winbtn_mac_close");
            folderIcon = EditorGUIUtility.FindTexture("Folder Icon");
            favoriteIcon = EditorGUIUtility.FindTexture("Favorite Icon");
            gearPopupIcon = EditorGUIUtility.FindTexture("d__Popup");
            gearIcon = EditorGUIUtility.FindTexture("EditorSettings Icon");

        }

        public static Texture2D playIcon { get; private set; }
        public static Texture2D pauseIcon { get; private set; }
        public static Texture2D stepIcon { get; private set; }
        public static Texture2D viewIcon { get; private set; }
        public static Texture2D csIcon { get; private set; }
        public static Texture2D tagIcon { get; private set; }
        public static Texture2D searchIcon { get; private set; }
        public static Texture2D infoIcon { get; private set; }
        public static Texture2D warningIcon { get; private set; }
        public static Texture2D errorIcon { get; private set; }
        public static Texture2D redCircle { get; private set; }
        public static Texture2D folderIcon { get; private set; }
        public static Texture2D favoriteIcon { get; private set; }
        public static Texture2D gearPopupIcon { get; private set; }
        public static Texture2D gearIcon { get; private set; }


        ///----------------------------------------------------------------------------------------------

        ///Returns a type icon
        public static Texture GetTypeIcon(System.Type type) {
            return TypePrefs.GetTypeIcon(type);
        }
    }
}

#endif