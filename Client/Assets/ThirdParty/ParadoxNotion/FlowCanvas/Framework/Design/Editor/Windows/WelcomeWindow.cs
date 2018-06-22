#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Editor {

    public class WelcomeWindow : EditorWindow {

        private static Texture2D docsIcon;
        private static Texture2D resourcesIcon;
        private static Texture2D communityIcon;
        private static Texture2D paradoxHeader;
        private static System.Type assetType;

        void OnEnable() {
            titleContent  = new GUIContent("Welcome");
            docsIcon      = EditorGUIUtility.FindTexture("TextAsset Icon");
            resourcesIcon = EditorGUIUtility.FindTexture("d_WelcomeScreen.AssetStoreLogo");
            communityIcon = EditorGUIUtility.FindTexture("AudioChorusFilter Icon");
            paradoxHeader = Resources.Load("ParadoxNotionHeader") as Texture2D;
            var size = new Vector2(paradoxHeader.width, 500);
            minSize = size;
            maxSize = size;
        }

        void OnGUI() {
            
            var att = assetType != null? (GraphInfoAttribute)assetType.GetCustomAttributes(typeof(GraphInfoAttribute), true).FirstOrDefault() : null;
            var packageName = att != null? att.packageName : "NodeCanvas";
            var docsURL = att != null? att.docsURL : "http://nodecanvas.com";
            var resourcesURL = att != null? att.resourcesURL : "http://nodecanvas.com/";
            var forumsURL = att != null? att.forumsURL : "http://nodecanvas.com/";

            var headerRect = new Rect(0, 0, paradoxHeader.width, paradoxHeader.height);
            EditorGUIUtility.AddCursorRect(headerRect, MouseCursor.Link);
            if (GUI.Button(headerRect, paradoxHeader, GUIStyle.none )) {
                UnityEditor.Help.BrowseURL("http://www.paradoxnotion.com");
            }
            GUILayout.Space(paradoxHeader.height);

            GUI.skin.label.richText = true;
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();

            GUILayout.Space(10);
            GUILayout.Label(string.Format("<size=26><b>Welcome to {0}!</b></size>", packageName ));
            GUILayout.Label(string.Format("<i>Thanks for using {0}! Following are a few important links to get you started!</i>", packageName ) );
            GUILayout.Space(10);

            ///----------------------------------------------------------------------------------------------

            GUILayout.BeginHorizontal(Styles.roundedBox);
            GUI.backgroundColor = new Color(1,1,1,0f);
            if ( GUILayout.Button(docsIcon, GUILayout.Width(64), GUILayout.Height(64)) ) {
                UnityEditor.Help.BrowseURL(docsURL);
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.BeginVertical();
            GUILayout.Space(15);
            GUILayout.Label("<size=16><b>Documentation</b></size>\nRead thorough documentation and API reference online.");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUI.backgroundColor = Color.white;

            ///----------------------------------------------------------------------------------------------

            GUILayout.BeginHorizontal(Styles.roundedBox);
            GUI.backgroundColor = new Color(1, 1, 1, 0f);
            if (GUILayout.Button(resourcesIcon, GUILayout.Width(64), GUILayout.Height(64)))
            {
                UnityEditor.Help.BrowseURL(resourcesURL);
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.BeginVertical();
            GUILayout.Space(15);
            GUILayout.Label("<size=16><b>Resources</b></size>\nDownload samples, extensions and other resources.");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUI.backgroundColor = Color.white;

            ///----------------------------------------------------------------------------------------------

            GUILayout.BeginHorizontal(Styles.roundedBox);
            GUI.backgroundColor = new Color(1, 1, 1, 0f);
            if (GUILayout.Button(communityIcon, GUILayout.Width(64), GUILayout.Height(64)))
            {
                UnityEditor.Help.BrowseURL(forumsURL);
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.BeginVertical();
            GUILayout.Space(15);
            GUILayout.Label("<size=16><b>Community</b></size>\nJoin the online forums, give feedback and get support.");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUI.backgroundColor = Color.white;

            ///----------------------------------------------------------------------------------------------

            GUILayout.FlexibleSpace();

            GUILayout.Label("Please consider leaving a review to support the product. Thank you!");

            GUILayout.Space(5);

            NCPrefs.showWelcomeWindow = EditorGUILayout.ToggleLeft("Show On Startup", NCPrefs.showWelcomeWindow);

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
        }
        
        //...
        public static void ShowWindow(System.Type t) {
            var window = CreateInstance<WelcomeWindow>();
            assetType = t;
            window.ShowUtility();
        }
    }
}

#endif