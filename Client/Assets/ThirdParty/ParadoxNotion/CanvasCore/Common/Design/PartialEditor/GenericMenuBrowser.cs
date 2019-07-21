#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using ParadoxNotion.Serialization;
using ParadoxNotion.Services;

namespace ParadoxNotion.Design
{

    //TODO: This needs a rewrite.
    ///Proviving a UnityEditor.GenericMenu, shows a complete popup browser.
    public class GenericMenuBrowser : PopupWindowContent
    {

        //class node used in browser
        class Node
        {
            public EditorUtils.MenuItemInfo item = null;
            public Node parent = null;
            public Dictionary<string, Node> children = new Dictionary<string, Node>(System.StringComparer.OrdinalIgnoreCase);
            public string name = null;
            public bool unfolded = true;
            public string fullPath = null;

            //Leafs have menu items
            public bool isLeaf {
                get { return item != null; }
            }

            public string category {
                get { return parent != null ? parent.fullPath : string.Empty; }
            }

            //Is node favorite?
            public bool isFavorite {
                get
                {
                    if ( GenericMenuBrowser.favorites.Contains(fullPath) ) {
                        return true;
                    }

                    var p = parent;
                    while ( p != null ) {
                        if ( p.isFavorite ) {
                            return true;
                        }
                        p = p.parent;
                    }
                    return false;
                }
            }

            //Does node has any favorite children?
            public bool HasAnyFavoriteChild() {
                if ( !string.IsNullOrEmpty(fullPath) ) {
                    return GenericMenuBrowser.favorites.Any(p => p.StartsWith(fullPath + "/"));
                }
                return false;
            }

            //Toggle favorite state
            public void ToggleFavorite() {
                SetFavorite(!isFavorite);
            }

            //Set favorite state
            void SetFavorite(bool fav) {
                if ( fav == true && !isFavorite ) {
                    GenericMenuBrowser.AddFavorite(fullPath);
                }

                if ( fav == false && isFavorite ) {
                    GenericMenuBrowser.RemoveFavorite(fullPath);
                }
            }
        }

        ///----------------------------------------------------------------------------------------------

        ///Browser preferences and saved favorites per key
        class SerializationData
        {
            public Dictionary<string, List<string>> allFavorites = new Dictionary<string, List<string>>(System.StringComparer.OrdinalIgnoreCase);
            public bool filterFavorites = false;
        }

        ///----------------------------------------------------------------------------------------------

        private const string PREFERENCES_KEY = "ParadoxNotion.ContextBrowserPreferences_2";
        private static System.Threading.Thread generationThread;
        private static GenericMenuBrowser current;
        private static SerializationData data;
        private static System.Type currentKeyType;
        private static List<string> favorites;

        private static bool _filterFavorites;
        private static bool filterFavorites {
            get { return _filterFavorites && currentKeyType != null; }
            set { _filterFavorites = value; }
        }

        private static GenericMenu boundMenu;
        private static EditorUtils.MenuItemInfo[] items;
        private static Node rootNode = new Node();
        private static List<Node> leafNodes;
        private static Node currentNode = null;

        private static float loadProgress;
        private const float HELP_RECT_HEIGHT = 58;
        private string title;
        private Color hoverColor = new Color(0.5f, 0.5f, 1, 0.3f);
        private Vector2 scrollPos;
        private string lastSearch;
        private string search;
        private GUIStyle headerStyle;
        private int lastHoveringIndex;
        private int hoveringIndex;
        private bool init;
        private float helpRectRequiredHeight = 0;
        private EditorWindow wasFocusedWindow;


        ///----------------------------------------------------------------------------------------------

        private GUIStyle _helpStyle;
        private GUIStyle helpStyle {
            get
            {
                if ( _helpStyle == null ) {
                    _helpStyle = new GUIStyle("label");
                    _helpStyle.wordWrap = true;
                    _helpStyle.richText = true;
                    _helpStyle.alignment = TextAnchor.UpperLeft;
                }
                return _helpStyle;
            }
        }

        ///----------------------------------------------------------------------------------------------

        //...
        public override Vector2 GetWindowSize() { return new Vector2(480, Mathf.Max(500 + helpRectRequiredHeight, 500)); }

        ///Shows the popup menu at position and with title
        public static void Show(GenericMenu newMenu, Vector2 pos, string title, System.Type keyType) {
            PopupWindow.Show(new Rect(pos.x, pos.y, 0, 0), new GenericMenuBrowser(newMenu, title, keyType));
        }

        //Set the menu.
        public static void SetMenu(GenericMenu newMenu) {
            if ( newMenu == null ) {
                return;
            }
            boundMenu = newMenu;
            if ( generationThread != null ) {
                generationThread.Abort();
            }
            generationThread = Threader.StartAction(GenerateTree, () =>
           {
               generationThread = null;
               if ( current != null ) {
                   current.editorWindow.Repaint();
               }
           });
        }

        //init
        public GenericMenuBrowser(GenericMenu newMenu, string title, System.Type keyType) {
            current = this;
            this.title = title;
            currentKeyType = keyType;
            rootNode = new Node();
            currentNode = rootNode;
            headerStyle = new GUIStyle("label");
            headerStyle.alignment = TextAnchor.UpperCenter;
            lastHoveringIndex = -1;
            hoveringIndex = -1;
            search = null;
            lastSearch = null;
            SetMenu(newMenu);
        }

        public override void OnOpen() {
            wasFocusedWindow = EditorWindow.focusedWindow;
            LoadPrefs();
        }
        public override void OnClose() {
            SavePrefs();
            EditorWindow.FocusWindowIfItsOpen(wasFocusedWindow != null ? wasFocusedWindow.GetType() : null);
            current = null;
        }

        //...
        private static void LoadPrefs() {
            if ( data == null ) {
                var json = EditorPrefs.GetString(PREFERENCES_KEY);
                if ( !string.IsNullOrEmpty(json) ) { data = JSONSerializer.Deserialize<SerializationData>(json); }
                if ( data == null ) { data = new SerializationData(); }

                filterFavorites = data.filterFavorites;
                if ( currentKeyType != null ) {
                    data.allFavorites.TryGetValue(currentKeyType.Name, out favorites);
                }

                if ( favorites == null ) {
                    favorites = new List<string>();
                }
            }
        }

        //...
        private static void SavePrefs() {
            data.filterFavorites = filterFavorites;
            if ( currentKeyType != null ) {
                data.allFavorites[currentKeyType.Name] = favorites;
            }
            EditorPrefs.SetString(PREFERENCES_KEY, JSONSerializer.Serialize(typeof(SerializationData), data));
        }

        //...
        public static void AddFavorite(string path) {
            if ( !favorites.Contains(path) ) {
                favorites.Add(path);
            }
        }

        //...
        public static void RemoveFavorite(string path) {
            if ( favorites.Contains(path) ) {
                favorites.Remove(path);
            }
        }

        //Generate the tree node structure out of the items
        static void GenerateTree() {
            loadProgress = 0;
            items = EditorUtils.GetMenuItems(boundMenu);
            leafNodes = new List<Node>();
            for ( var i = 0; i < items.Length; i++ ) {
                loadProgress = i / (float)items.Length;
                var item = items[i];
                var itemPath = item.content.text;
                var parts = itemPath.Split('/');
                Node current = rootNode;
                var path = string.Empty;
                for ( var j = 0; j < parts.Length; j++ ) {
                    var part = parts[j];
                    path += "/" + part;
                    Node child = null;
                    if ( !current.children.TryGetValue(part, out child) ) {
                        child = new Node { name = part, parent = current };
                        child.fullPath = path;
                        current.children[part] = child;
                        if ( part == parts.Last() ) {
                            child.item = item;
                            leafNodes.Add(child);
                        }
                    }
                    current = child;
                }
            }
        }


        //Show stuff
        public override void OnGUI(Rect rect) {

            var e = Event.current;
            EditorGUIUtility.SetIconSize(Vector2.zero);
            hoveringIndex = Mathf.Clamp(hoveringIndex, -1, currentNode.children.Count - 1);
            if ( EditorGUIUtility.isProSkin ) {
                GUI.Box(rect, string.Empty, Styles.shadowedBackground);
            }

            var headerHeight = currentNode.parent != null ? 95 : 60;
            var headerRect = new Rect(0, 0, rect.width, headerHeight);
            DoHeader(headerRect, e);

            if ( ( generationThread != null && generationThread.ThreadState != System.Threading.ThreadState.Stopped ) || items == null || items.Length == 0 ) {
                var progressRect = new Rect(0, 0, 200, 20);
                progressRect.center = rect.center;
                EditorGUI.ProgressBar(progressRect, loadProgress, "Loading...");
                editorWindow.Repaint();
            } else {
                var treeRect = Rect.MinMaxRect(0, headerHeight, rect.width, rect.height - HELP_RECT_HEIGHT);
                DoTree(treeRect, e);
            }

            var helpRect = Rect.MinMaxRect(2, rect.height - HELP_RECT_HEIGHT + 2, rect.width - 2, rect.height - 2);
            DoFooter(helpRect, e);

            //handle the events
            HandeEvents(e);
            if ( !init ) {
                init = true;
                EditorGUI.FocusTextInControl("SearchToolbar");
            }

            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        //...
        void DoHeader(Rect headerRect, Event e) {
            //HEADER
            GUILayout.Space(5);
            GUILayout.Label(string.Format("<color=#{0}><size=15><b>{1}</b></size></color>", EditorGUIUtility.isProSkin ? "dddddd" : "222222", title), headerStyle);

            ///SEARCH
            if ( e.keyCode == KeyCode.DownArrow ) { GUIUtility.keyboardControl = 0; }
            if ( e.keyCode == KeyCode.UpArrow ) { GUIUtility.keyboardControl = 0; }
            if ( e.keyCode == KeyCode.Return ) { GUIUtility.keyboardControl = 0; }
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("SearchToolbar");
            search = EditorUtils.SearchField(search);
            if ( currentKeyType != null ) {
                filterFavorites = EditorGUILayout.ToggleLeft("FavOnly", filterFavorites, GUILayout.Width(70));
            }
            GUILayout.EndHorizontal();
            EditorUtils.BoldSeparator();

            ///BACK
            if ( currentNode.parent != null && string.IsNullOrEmpty(search) ) {
                GUILayout.BeginHorizontal("box");
                if ( GUILayout.Button(string.Format("<b><size=14>◄ {0}/{1}</size></b>", currentNode.parent.name, currentNode.name), (GUIStyle)"label") ) {
                    currentNode = currentNode.parent;
                }
                GUILayout.EndHorizontal();
                var lastRect = GUILayoutUtility.GetLastRect();
                if ( lastRect.Contains(e.mousePosition) ) {
                    GUI.color = hoverColor;
                    GUI.DrawTexture(lastRect, EditorGUIUtility.whiteTexture);
                    GUI.color = Color.white;
                    base.editorWindow.Repaint();
                    hoveringIndex = -1;
                }
            }
        }

        //THE TREE
        void DoTree(Rect treeRect, Event e) {
            GUILayout.BeginArea(treeRect);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            GUILayout.BeginVertical();

            if ( search != lastSearch ) {
                hoveringIndex = 0;
                if ( !string.IsNullOrEmpty(search) ) {
                    var results = new List<Node>();
                    foreach ( var node in leafNodes ) {
                        if ( filterFavorites && !node.isFavorite ) {
                            continue;
                        }
                        if ( StringUtils.SearchMatch(search, node.name, node.category) ) {
                            results.Add(node);
                        }
                    }
                    var searchRootNode = new Node() { name = "Search Root" };
                    //Remark: scoring is done on category name instead of leaf name
                    searchRootNode.children = results
                        .OrderBy(r => StringUtils.ScoreSearchMatch(search, r.category, r.category) * ( r.isFavorite ? 0.5f : 1 ))
                        .ToDictionary(r => r.fullPath, r => r);
                    currentNode = searchRootNode;
                } else {
                    currentNode = rootNode;
                }
                lastSearch = search;
            }

            ///----------------------------------------------------------------------------------------------

            var i = 0;
            var itemAdded = false;
            string lastSearchCategory = null;
            foreach ( var childPair in currentNode.children ) {

                var isSearch = !string.IsNullOrEmpty(search);

                if ( isSearch && i >= 200 ) {
                    EditorGUILayout.HelpBox("There are more than 200 results. Please try refine your search input.", MessageType.Info);
                    break;
                }

                var node = childPair.Value;
                var leafItem = node.item;
                var memberInfo = leafItem != null ? leafItem.userData as MemberInfo : null;
                var isDisabled = leafItem != null && leafItem.func == null && leafItem.func2 == null;
                var icon = leafItem != null ? leafItem.content.image : Icons.folderIcon;
                if ( icon == null && memberInfo != null ) {
                    icon = TypePrefs.GetTypeIcon(memberInfo);
                }

                //when within search, show category on top
                if ( isSearch ) {
                    var searchCategory = lastSearchCategory;
                    if ( memberInfo == null || memberInfo is System.Type ) {
                        searchCategory = node.parent.fullPath != null ? node.parent.fullPath.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() : null;
                    } else {
                        searchCategory = memberInfo.ReflectedType.FriendlyName();
                    }

                    if ( searchCategory != lastSearchCategory ) {
                        lastSearchCategory = searchCategory;
                        GUI.color = EditorGUIUtility.isProSkin ? Color.black : Color.white;
                        GUILayout.BeginHorizontal("box");
                        GUI.color = Color.white;
                        GUILayout.Label(searchCategory, Styles.leftLabel, GUILayout.Height(16));
                        GUILayout.EndHorizontal();
                    }
                }
                //


                if ( filterFavorites && !node.isFavorite && !node.HasAnyFavoriteChild() ) {
                    continue;
                }

                if ( leafItem != null && leafItem.separator ) {
                    if ( itemAdded ) {
                        EditorUtils.Separator();
                    }
                    continue;
                }

                itemAdded = true;

                GUI.color = EditorGUIUtility.isProSkin ? Color.white : new Color(0.8f, 0.8f, 0.8f, 1);
                GUILayout.BeginHorizontal("box");

                //Prefix icon
                GUI.color = Color.white;
                GUILayout.Label(icon, GUILayout.Width(32), GUILayout.Height(16));
                GUI.enabled = !isDisabled;

                //Favorite
                if ( currentKeyType != null ) {
                    GUI.color = node.isFavorite ? Color.white : ( node.HasAnyFavoriteChild() ? new Color(1, 1, 1, 0.2f) : new Color(0f, 0f, 0f, 0.4f) );
                    if ( GUILayout.Button(Icons.favoriteIcon, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(16)) ) {
                        node.ToggleFavorite();
                    }
                    GUI.color = Color.white;
                }

                //Content
                var label = node.name;
                var text = string.Format("<size=9>{0}</size>", ( leafItem == null ? string.Format("<b>{0}</b>", label) : label ));
                GUILayout.Box(text, (GUIStyle)"label", GUILayout.Width(0), GUILayout.ExpandWidth(true));
                GUILayout.Label(leafItem != null ? "●" : "►", GUILayout.Width(20));
                GUILayout.EndHorizontal();

                var elementRect = GUILayoutUtility.GetLastRect();
                if ( e.type == EventType.MouseDown && e.button == 0 && elementRect.Contains(e.mousePosition) ) {
                    e.Use();
                    if ( leafItem != null ) {

                        ExecuteItemFunc(leafItem);
                        break;

                    } else {

                        currentNode = node;
                        hoveringIndex = 0;
                        break;
                    }
                }

                if ( e.type == EventType.MouseMove && elementRect.Contains(e.mousePosition) ) {
                    hoveringIndex = i;
                }

                if ( hoveringIndex == i ) {
                    GUI.color = hoverColor;
                    GUI.DrawTexture(elementRect, EditorGUIUtility.whiteTexture);
                    GUI.color = Color.white;
                }

                i++;
                GUI.enabled = true;
            }

            if ( hoveringIndex != lastHoveringIndex ) {
                base.editorWindow.Repaint();
                lastHoveringIndex = hoveringIndex;
            }

            if ( !itemAdded ) {
                GUILayout.Label("No results to display with current search and filter combination");
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        ///HELP AREA
        void DoFooter(Rect helpRect, Event e) {
            helpRectRequiredHeight = 0;
            var hoveringNode = hoveringIndex >= 0 && currentNode.children.Count > 0 ? currentNode.children.Values.ToList()[hoveringIndex] : null;
            GUI.color = new Color(0, 0, 0, 0.3f);
            GUI.Box(helpRect, string.Empty, (GUIStyle)"TextField");
            GUI.color = Color.white;
            GUILayout.BeginArea(helpRect);
            GUILayout.BeginVertical();
            var doc = string.Empty;
            if ( hoveringNode != null && hoveringNode.item != null ) {
                doc = hoveringNode.item.content.tooltip;
                var memberInfo = hoveringNode.item.userData as MemberInfo;
                if ( memberInfo != null && string.IsNullOrEmpty(doc) ) {
                    if ( memberInfo is System.Type ) {
                        doc = TypePrefs.GetTypeDoc(memberInfo);
                    } else {
                        doc = DocsByReflection.GetMemberSummary(memberInfo);
                    }
                }
            }

            GUILayout.Label(string.Format("<size=9>{0}</size>", doc), helpStyle);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        ///----------------------------------------------------------------------------------------------

        //Executes the item's registered delegate
        void ExecuteItemFunc(EditorUtils.MenuItemInfo item) {
            if ( item.func != null ) {
                item.func();
            } else {
                item.func2(item.userData);
            }
            base.editorWindow.Close();
        }

        //Handle events
        void HandeEvents(Event e) {

            //Go back with right click as well...
            if ( e.type == EventType.MouseDown && e.button == 1 ) {
                if ( currentNode.parent != null ) {
                    currentNode = currentNode.parent;
                }
                e.Use();
            }

            if ( e.type == EventType.KeyDown ) {

                if ( e.keyCode == KeyCode.RightArrow || e.keyCode == KeyCode.Return ) {
                    var next = currentNode.children.Values.ToList()[hoveringIndex];
                    if ( e.keyCode == KeyCode.Return && next.item != null ) {
                        ExecuteItemFunc(next.item);
                    } else if ( next.item == null ) {
                        currentNode = next;
                        hoveringIndex = 0;
                    }
                    return;
                }

                if ( e.keyCode == KeyCode.LeftArrow ) {
                    var previous = currentNode.parent;
                    if ( previous != null ) {
                        hoveringIndex = currentNode.parent.children.Values.ToList().IndexOf(currentNode);
                        currentNode = previous;
                    }
                    return;
                }

                if ( e.keyCode == KeyCode.DownArrow ) {
                    hoveringIndex++;
                    return;
                }

                if ( e.keyCode == KeyCode.UpArrow ) {
                    hoveringIndex = Mathf.Max(hoveringIndex - 1, 0);
                    return;
                }

                if ( e.keyCode == KeyCode.Escape ) {
                    base.editorWindow.Close();
                    return;
                }

                EditorGUI.FocusTextInControl("SearchToolbar");

            }
        }
    }
}

#endif