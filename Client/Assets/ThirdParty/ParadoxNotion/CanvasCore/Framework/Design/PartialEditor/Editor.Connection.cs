#if UNITY_EDITOR

using NodeCanvas.Editor;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;


namespace NodeCanvas.Framework
{

    partial class Connection
    {

        public enum RelinkState
        {
            None,
            Source,
            Target
        }

        public enum TipConnectionStyle
        {
            None,
            Circle,
            Arrow
        }


        [SerializeField]
        private bool _infoCollapsed;

        const float RELINK_DISTANCE_SNAP = 20f;
        const float STATUS_BLINK_DURATION = 0.25f;
        const float STATUS_BLINK_SIZE_ADD = 2f;
        const float STATUS_BLINK_PACKET_SPEED = 0.8f;
        const float STATUS_BLINK_PACKET_SIZE = 10f;
        const float STATUS_BLINK_PACKET_COUNT = 4f;

        public Rect startRect { get; private set; }
        public Rect endRect { get; private set; }
        private Rect centerRect;
        private Vector2 fromTangent;
        private Vector2 toTangent;

        private Status lastStatus = Status.Resting;
        private Color color = StyleSheet.GetStatusColor(Status.Resting);
        private float size = 3;
        private float statusChangeTime;

        private Vector2? relinkClickPos;
        private bool relinkSnaped;

        ///----------------------------------------------------------------------------------------------

        ///Editor. Is info expanded?
        private bool infoExpanded {
            get { return !_infoCollapsed; }
            set { _infoCollapsed = !value; }
        }

        ///Editor. Is currently actively relinking?
        private bool isRelinkingActive {
            get { return relinkClickPos != null && relinkSnaped; }
        }

        ///Editor. Current relinking state
        public RelinkState relinkState { get; private set; }

        ///Editor. Default Color of connection
        virtual public Color defaultColor {
            get { return StyleSheet.GetStatusColor(status); }
        }

        ///Editor. Defacult size of connection
        virtual public float defaultSize {
            get { return 3f; }
        }

        ///Editor. End Tip connection style
        virtual public TipConnectionStyle tipConnectionStyle {
            get { return TipConnectionStyle.Circle; }
        }

        ///Editor. Optional explicit "flow" direction
        virtual public ParadoxNotion.PlanarDirection direction {
            get { return ParadoxNotion.PlanarDirection.Auto; }
        }

        ///----------------------------------------------------------------------------------------------

        //Draw connection from-to
        public void DrawConnectionGUI(Vector2 fromPos, Vector2 toPos) {

            var _startRect = new Rect(0, 0, 12, 12);
            _startRect.center = fromPos;
            startRect = _startRect;

            var _endRect = new Rect(0, 0, 16, 16);
            _endRect.center = toPos;
            endRect = _endRect;

            ParadoxNotion.CurveUtils.ResolveTangents(fromPos, toPos, sourceNode.rect, targetNode.rect, 0.8f, direction, out fromTangent, out toTangent);

            HandleEvents(fromPos, toPos);
            DrawConnection(fromPos, toPos);

            if ( !isRelinkingActive ) {
                if ( Application.isPlaying && isActive ) {
                    UpdateBlinkStatus(fromPos, toPos);
                }
                DrawInfoRect(fromPos, toPos);
            }
        }

        ///Handle UI events
        void HandleEvents(Vector2 fromPos, Vector2 toPos) {

            var e = Event.current;

            //On click select this connection
            if ( GraphEditorUtility.allowClick && e.type == EventType.MouseDown ) {
                float norm;
                var onConnection = ParadoxNotion.CurveUtils.IsPosAlongCurve(fromPos, toPos, fromTangent, toTangent, e.mousePosition, out norm);
                var onStart = startRect.Contains(e.mousePosition);
                var onEnd = endRect.Contains(e.mousePosition);
                var onCenter = centerRect.Contains(e.mousePosition);
                if ( onConnection || onStart || onEnd || onCenter ) {
                    if ( e.button == 0 ) {
                        GraphEditorUtility.activeElement = this;
                        relinkClickPos = e.mousePosition;
                        relinkSnaped = false;
                        if ( onConnection ) { relinkState = norm <= 0.1f || e.shift ? RelinkState.Source : RelinkState.Target; }
                        if ( onStart ) { relinkState = RelinkState.Source; }
                        if ( onEnd ) { relinkState = RelinkState.Target; }
                        if ( onCenter ) { relinkState = e.shift ? RelinkState.Source : RelinkState.Target; }
                    }
                    e.Use();
                }
            }

            if ( relinkClickPos != null ) {

                if ( relinkSnaped == false ) {
                    if ( Vector2.Distance(relinkClickPos.Value, e.mousePosition) > RELINK_DISTANCE_SNAP ) {
                        relinkSnaped = true;
                        sourceNode.OnActiveRelinkStart(this);
                    }
                }

                if ( e.rawType == EventType.MouseUp && e.button == 0 ) {
                    if ( relinkSnaped == true ) {
                        sourceNode.OnActiveRelinkEnd(this);
                    }
                    relinkClickPos = null;
                    relinkSnaped = false;
                    relinkState = RelinkState.None;
                    e.Use();
                }
            }

            if ( GraphEditorUtility.allowClick && e.type == EventType.MouseUp && e.button == 1 && centerRect.Contains(e.mousePosition) ) {
                GraphEditorUtility.PostGUI += () => { GetConnectionMenu().ShowAsContext(); };
                e.Use();
            }
        }

        //The actual connection graphic
        void DrawConnection(Vector2 fromPos, Vector2 toPos) {

            color = isActive ? color : Colors.Grey(0.3f);
            if ( !Application.isPlaying ) {
                color = isActive ? defaultColor : Colors.Grey(0.3f);
                var highlight = GraphEditorUtility.activeElement == this || GraphEditorUtility.activeElement == sourceNode || GraphEditorUtility.activeElement == targetNode;
                if ( startRect.Contains(Event.current.mousePosition) || endRect.Contains(Event.current.mousePosition) ) {
                    highlight = true;
                }
                color.a = highlight ? 1 : color.a;
                size = highlight ? defaultSize + 2 : defaultSize;
            }

            //alter from/to if active relinking
            if ( isRelinkingActive ) {
                if ( relinkState == RelinkState.Source ) {
                    fromPos = Event.current.mousePosition;
                }
                if ( relinkState == RelinkState.Target ) {
                    toPos = Event.current.mousePosition;
                }
                ParadoxNotion.CurveUtils.ResolveTangents(fromPos, toPos, 0.8f, direction, out fromTangent, out toTangent);
                size = defaultSize;
            }

            var shadow = new Vector2(3.5f, 3.5f);
            Handles.DrawBezier(fromPos, toPos + shadow, fromPos + shadow + fromTangent + shadow, toPos + shadow + toTangent, Color.black.WithAlpha(0.1f), null, size + 10f);
            Handles.DrawBezier(fromPos, toPos, fromPos + fromTangent, toPos + toTangent, color, null, size);

            GUI.color = color.WithAlpha(1);
            if ( tipConnectionStyle == TipConnectionStyle.Arrow ) {
                GUI.DrawTexture(endRect, StyleSheet.GetDirectionArrow(toTangent.normalized));
            }
            if ( tipConnectionStyle == TipConnectionStyle.Circle ) {
                GUI.DrawTexture(endRect, StyleSheet.circle);
            }
            GUI.color = Color.white;
        }

        //Information showing in the middle
        void DrawInfoRect(Vector2 fromPos, Vector2 toPos) {

            centerRect.center = ParadoxNotion.CurveUtils.GetPosAlongCurve(fromPos, toPos, fromTangent, toTangent, 0.55f);

            var isExpanded = infoExpanded || GraphEditorUtility.activeElement == this || GraphEditorUtility.activeElement == sourceNode;
            var alpha = isExpanded ? 0.8f : 0.25f;
            var info = GetConnectionInfo();
            var extraInfo = sourceNode.GetConnectionInfo(sourceNode.outConnections.IndexOf(this));
            if ( !string.IsNullOrEmpty(info) || !string.IsNullOrEmpty(extraInfo) ) {

                if ( !string.IsNullOrEmpty(extraInfo) && !string.IsNullOrEmpty(info) ) {
                    extraInfo = "\n" + extraInfo;
                }

                var textToShow = isExpanded ? string.Format("<size=9>{0}{1}</size>", info, extraInfo) : "<size=9>...</size>";
                var finalSize = StyleSheet.box.CalcSize(new GUIContent(textToShow));

                centerRect.width = finalSize.x;
                centerRect.height = finalSize.y;

                EditorGUIUtility.AddCursorRect(centerRect, MouseCursor.Link);

                GUI.color = Colors.Grey(EditorGUIUtility.isProSkin ? 0.17f : 0.5f).WithAlpha(0.95f);
                GUI.DrawTexture(centerRect, Texture2D.whiteTexture);

                GUI.color = Color.white.WithAlpha(alpha);
                GUI.Label(centerRect, textToShow, Styles.centerLabel);
                GUI.color = Color.white;

            } else {

                centerRect.width = 0;
                centerRect.height = 0;
            }
        }

        ///Updates the blink status
        void UpdateBlinkStatus(Vector2 fromPos, Vector2 toPos) {
            if ( status != lastStatus ) {
                lastStatus = status;
                statusChangeTime = Time.time;
            }

            var deltaTimeSinceChange = ( Time.time - statusChangeTime );
            if ( status != Status.Resting || size != defaultSize ) {
                size = Mathf.Lerp(defaultSize + STATUS_BLINK_SIZE_ADD, defaultSize, deltaTimeSinceChange / STATUS_BLINK_DURATION);
            }

            if ( status != Status.Resting || size == defaultSize ) {
                color = defaultColor;
            }

            if ( status == Status.Running ) {
                var packetTraversal = deltaTimeSinceChange * STATUS_BLINK_PACKET_SPEED;
                for ( var i = 0f; i < STATUS_BLINK_PACKET_COUNT; i++ ) {
                    var progression = packetTraversal + ( i / STATUS_BLINK_PACKET_COUNT );
                    var normPos = Mathf.Repeat(progression, 1f);

                    var packetColor = this.color;
                    var pingPong = Mathf.PingPong(normPos, 0.5f);
                    var norm = ( pingPong * 2 ) / 0.5f;
                    var pSize = Mathf.Lerp(0.5f, 1f, norm) * STATUS_BLINK_PACKET_SIZE;
                    packetColor.a = norm * ( deltaTimeSinceChange / ( STATUS_BLINK_DURATION + 0.25f ) );

                    var rect = new Rect(0, 0, pSize, pSize);
                    rect.center = ParadoxNotion.CurveUtils.GetPosAlongCurve(fromPos, toPos, fromTangent, toTangent, normPos); ;
                    GUI.color = packetColor;
                    GUI.DrawTexture(rect, StyleSheet.circle);
                    GUI.color = Color.white;
                }
            }
        }

        ///----------------------------------------------------------------------------------------------

        //The connection's inspector
        public static void ShowConnectionInspectorGUI(Connection c) {

            UndoManager.CheckUndo(c.graph, "Connection Inspector");

            GUILayout.BeginHorizontal();
            GUI.color = new Color(1, 1, 1, 0.5f);

            if ( GUILayout.Button("◄", GUILayout.Height(14), GUILayout.Width(20)) ) {
                GraphEditorUtility.activeElement = c.sourceNode;
            }

            if ( GUILayout.Button("►", GUILayout.Height(14), GUILayout.Width(20)) ) {
                GraphEditorUtility.activeElement = c.targetNode;
            }

            c.isActive = EditorGUILayout.ToggleLeft("ACTIVE", c.isActive, GUILayout.Width(150));

            GUILayout.FlexibleSpace();

            if ( GUILayout.Button("X", GUILayout.Height(14), GUILayout.Width(20)) ) {
                GraphEditorUtility.PostGUI += delegate { c.graph.RemoveConnection(c); };
                return;
            }

            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            EditorUtils.BoldSeparator();
            c.OnConnectionInspectorGUI();
            c.sourceNode.OnConnectionInspectorGUI(c.sourceNode.outConnections.IndexOf(c));

            UndoManager.CheckDirty(c.graph);
        }

        //The information to show in the middle area of the connection
        virtual protected string GetConnectionInfo() { return null; }
        //Editor.Override to show controls in the editor panel when connection is selected
        virtual protected void OnConnectionInspectorGUI() { }


        ///Returns the mid position rect of the connection
        public Rect GetMidRect() {
            return centerRect;
        }


        ///the connection context menu
        GenericMenu GetConnectionMenu() {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent(infoExpanded ? "Collapse Info" : "Expand Info"), false, () => { infoExpanded = !infoExpanded; });
            menu.AddItem(new GUIContent(isActive ? "Disable" : "Enable"), false, () => { isActive = !isActive; });

            var assignable = this as ITaskAssignable;
            if ( assignable != null ) {

                if ( assignable.task != null ) {
                    menu.AddItem(new GUIContent("Copy Assigned Condition"), false, () => { CopyBuffer.Set<Task>(assignable.task); });
                } else {
                    menu.AddDisabledItem(new GUIContent("Copy Assigned Condition"));
                }

                if ( CopyBuffer.Has<Task>() ) {
                    menu.AddItem(new GUIContent(string.Format("Paste Assigned Condition ({0})", CopyBuffer.Peek<Task>().name)), false, () =>
                    {
                        if ( assignable.task == CopyBuffer.Peek<Task>() ) {
                            return;
                        }

                        if ( assignable.task != null ) {
                            if ( !EditorUtility.DisplayDialog("Paste Condition", string.Format("Connection already has a Condition assigned '{0}'. Replace assigned condition with pasted condition '{1}'?", assignable.task.name, CopyBuffer.Peek<Task>().name), "YES", "NO") ) {
                                return;
                            }
                        }

                        try { assignable.task = CopyBuffer.Get<Task>().Duplicate(graph); }
                        catch { Logger.LogWarning("Can't paste Condition here. Incombatible Types.", "Editor", this); }
                    });

                } else {
                    menu.AddDisabledItem(new GUIContent("Paste Assigned Condition"));
                }

            }

            menu.AddSeparator("/");
            menu.AddItem(new GUIContent("Delete"), false, () => { graph.RemoveConnection(this); });
            return menu;
        }

    }
}

#endif