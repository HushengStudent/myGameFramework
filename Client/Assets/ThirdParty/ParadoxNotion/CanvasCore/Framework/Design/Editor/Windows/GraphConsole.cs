#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEditor;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;

namespace NodeCanvas.Editor
{

    public class GraphConsole : EditorWindow
    {

        private static GraphConsole current;
        private static Vector2 scrollPos;
        private static bool willRepaint;
        private static List<Logger.Message> messages;
        private static Dictionary<LogType, ConsoleStyle> styleMap;
        private static Dictionary<Graph, List<Logger.Message>> graphsMap;

        private static GUIStyle _labelStyle;
        private static GUIStyle labelStyle {
            get
            {
                if ( _labelStyle == null ) {
                    _labelStyle = new GUIStyle("label");
                    _labelStyle.richText = true;
                }
                return _labelStyle;
            }
        }

        private static GUIStyle _logTypeFilterStyle;
        private static GUIStyle logTypeFilterStyle {
            get
            {
                if ( _logTypeFilterStyle == null ) {
                    _logTypeFilterStyle = new GUIStyle(EditorStyles.toolbarButton);
                    _logTypeFilterStyle.margin = new RectOffset();
                    _logTypeFilterStyle.padding = new RectOffset();
                }
                return _logTypeFilterStyle;
            }
        }

        private struct ConsoleStyle
        {
            public Texture icon;
            public string hex;
            public ConsoleStyle(Texture icon, string hex) {
                this.icon = icon;
                this.hex = EditorGUIUtility.isProSkin ? hex : "eeeeee";
            }
        }

        [InitializeOnLoadMethod]
        static void Init() {
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= PlayModeChange;
            EditorApplication.playModeStateChanged += PlayModeChange;
#else
			EditorApplication.playmodeStateChanged -= PlayModeChange;
			EditorApplication.playmodeStateChanged += PlayModeChange;
#endif
            Logger.RemoveListener(OnLogMessageReceived);
            Logger.AddListener(OnLogMessageReceived);
            messages = new List<Logger.Message>();
            graphsMap = new Dictionary<Graph, List<Logger.Message>>();
            styleMap = new Dictionary<LogType, ConsoleStyle>
            {
                {LogType.Log,       new ConsoleStyle(Icons.infoIcon, "eeeeee")},
                {LogType.Warning,   new ConsoleStyle(Icons.warningIcon, "f6ff00")},
                {LogType.Error,     new ConsoleStyle(Icons.errorIcon, "db3b3b")},
                {LogType.Exception, new ConsoleStyle(Icons.errorIcon, "db3b3b")},
                {LogType.Assert,    new ConsoleStyle(Icons.infoIcon, "eeeeee")},
            };
        }

        //open the NC console window
        public static void ShowWindow() {
            var window = GetWindow<GraphConsole>();
            window.Show();
        }

        //...
        void OnEnable() {
            current = this;
            willRepaint = true;
            titleContent = new GUIContent(StyleSheet.canvasIcon);
            RefreshTitle();
        }

        //...
        void OnDisable() {
            current = null;
        }

        //...
        void RefreshTitle() {
            titleContent.text = messages != null && messages.Count > 0 ? "(!) Console" : "Console"; ;
        }

        //...
        static void PlayModeChange
            (
#if UNITY_2017_2_OR_NEWER
            PlayModeStateChange state
#endif
            ) {
            if ( !EditorApplication.isPlayingOrWillChangePlaymode ) {
                messages.Clear();
                graphsMap.Clear();
            }
            willRepaint = true;
        }

        //...
        static bool OnLogMessageReceived(Logger.Message msg) {
            if ( msg.tag == "Editor" ) {
                return false;
            }
            var graph = Graph.GetElementGraph(msg.context);
            if ( graph == null ) {
                return false;
            }

            if ( !graphsMap.ContainsKey(graph) ) {
                graphsMap[graph] = new List<Logger.Message>();
            }
            graphsMap[graph].Add(msg);
            messages.Add(msg);

            if ( current != null ) {
                current.RefreshTitle();
                willRepaint = true;
                return true;
            }

            return false;
        }

        //...
        void Update() {
            if ( willRepaint ) {
                willRepaint = false;
                Repaint();
            }
        }

        //show stuff
        void OnGUI() {

            if ( EditorGUIUtility.isProSkin ) {
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), string.Empty, Styles.shadowedBackground);
            }

            RefreshTitle();
            var e = Event.current;
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Space(4);
            var ascending = Prefs.consoleLogOrder == Prefs.ConsoleLogOrder.Ascending;
            var newValue = GUILayout.Toggle(ascending, new GUIContent(ascending ? "▲" : "▼"), "label", GUILayout.Width(14));
            if ( ascending != newValue ) { Prefs.consoleLogOrder = newValue ? Prefs.ConsoleLogOrder.Ascending : Prefs.ConsoleLogOrder.Descending; }
            GUILayout.Space(2);
            Prefs.consoleLogInfo = GUILayout.Toggle(Prefs.consoleLogInfo, new GUIContent(Icons.infoIcon), logTypeFilterStyle, GUILayout.Width(20));
            GUILayout.Space(2);
            Prefs.consoleLogWarning = GUILayout.Toggle(Prefs.consoleLogWarning, new GUIContent(Icons.warningIcon), logTypeFilterStyle, GUILayout.Width(20));
            GUILayout.Space(2);
            Prefs.consoleLogError = GUILayout.Toggle(Prefs.consoleLogError, new GUIContent(Icons.errorIcon), logTypeFilterStyle, GUILayout.Width(20));

            GUILayout.FlexibleSpace();
            if ( GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(70)) ) {
                messages.Clear();
            }
            GUILayout.EndHorizontal();

            if ( messages.Count == 0 ) {
                EditorGUILayout.HelpBox("This console will catch graph related logs and display them here instead of the normal Unity console, thus save bloat from the Unity console.\nFurthermore, any log displayed here can be clicked to focus the relevant graph element that the log relates to automatically.\nIt is recommended to dock this console bellow the Graph Editor for easier debugging.", MessageType.Info);
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            for ( var i = ( ascending ? messages.Count - 1 : 0 ); ( ascending ? i >= 0 : i < messages.Count ); i = ( ascending ? i - 1 : i + 1 ) ) {
                var msg = messages[i];
                if ( IsFiltered(msg.type) ) {
                    continue;
                }

                GUI.color = EditorGUIUtility.isProSkin ? Color.black : Color.grey;
                GUILayout.BeginHorizontal("box");
                GUI.color = Color.white;
                var content = GetFormatedGUIContentForMessage(msg);
                GUILayout.Label(content, labelStyle);
                if ( GUILayout.Button("X", GUILayout.Width(18)) ) {
                    messages.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
                var lastRect = GUILayoutUtility.GetLastRect();
                EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.Link);
                if ( lastRect.Contains(e.mousePosition) && e.type == EventType.MouseDown ) {
                    ProccessMessage(msg);
                }
            }
            GUILayout.EndScrollView();
        }

        //helper method
        public static GUIContent GetFormatedGUIContentForMessage(Logger.Message msg) {
            if ( !msg.IsValid() ) { return null; }
            var tagText = string.Format("<b>({0} {1})</b>", msg.tag, msg.type.ToString());
            var map = styleMap[msg.type];
            return new GUIContent(string.Format("<color=#{0}>{1}: {2}</color>", map.hex, tagText, msg.text), map.icon);
        }

        ///Fetch first logger message of target graph
        public static Logger.Message GetFirstMessageForGraph(Graph graph) {
            List<Logger.Message> list = null;
            if ( graphsMap.TryGetValue(graph, out list) ) {
                return list.FirstOrDefault();
            }
            return default(Logger.Message);
        }

        //...
        static bool IsFiltered(LogType type) {
            if ( type == LogType.Log && !Prefs.consoleLogInfo ) return true;
            if ( type == LogType.Warning && !Prefs.consoleLogWarning ) return true;
            if ( type == LogType.Error && !Prefs.consoleLogError ) return true;
            return false;
        }

        //...
        static void ProccessMessage(Logger.Message msg) {

            Object unityContext = null;

            var graph = Graph.GetElementGraph(msg.context);
            if ( graph != null ) {
                unityContext = graph.agent != null ? (Object)graph.agent.gameObject : (Object)graph;
            } else {
                unityContext = msg.context as Object;
            }

            if ( unityContext != null ) {
                Selection.activeObject = unityContext;
                EditorGUIUtility.PingObject(unityContext);
            }

            //cease here if no graph
            if ( graph == null ) {
                return;
            }

            var editor = GraphEditor.current;
            if ( editor == null || GraphEditor.currentGraph != graph ) {
                editor = GraphEditor.OpenWindow(graph);
            }

            IGraphElement element = null;
            if ( msg.context is IGraphElement ) {
                element = (IGraphElement)msg.context;
            }
            if ( msg.context is Task ) {
                element = graph.GetTaskParentElement((Task)msg.context);
            }
            if ( msg.context is BBParameter ) {
                element = graph.GetParameterParentElement((BBParameter)msg.context);
            }

            EditorApplication.delayCall += () => GraphEditor.FocusElement(element, true);

        }
    }
}

#endif