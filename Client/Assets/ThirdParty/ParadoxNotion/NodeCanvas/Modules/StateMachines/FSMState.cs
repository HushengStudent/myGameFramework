#if UNITY_EDITOR
using UnityEditor;
using NodeCanvas.Editor;
#endif

using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;

namespace NodeCanvas.StateMachines
{

    ///----------------------------------------------------------------------------------------------

    public interface IState
    {

        ///The name of the state
        string name { get; }

        ///The tag of the state
        string tag { get; }

        ///The elapsed time of the state
        float elapsedTime { get; }

        ///The FSM this state belongs to
        FSM FSM { get; }

        ///An array of the state's transition connections
        FSMConnection[] GetTransitions();

        ///Evaluates the state's transitions and returns true if a transition has been performed
        bool CheckTransitions();

        ///Marks the state as Finished
        void Finish(bool success);
    }

    ///----------------------------------------------------------------------------------------------

    /// Super base class for FSM state nodes that live within an FSM Graph.
    abstract public class FSMState : Node, IState
    {

        public enum TransitionEvaluationMode
        {
            CheckContinuously,
            CheckAfterStateFinished,
            CheckManually
        }

        [SerializeField]
        private TransitionEvaluationMode _transitionEvaluation;

        private float _elapsedTime;
        private bool _hasInit;

        public override bool allowAsPrime { get { return true; } }
        public override int maxInConnections { get { return -1; } }
        public override int maxOutConnections { get { return -1; } }
        sealed public override System.Type outConnectionType { get { return typeof(FSMConnection); } }
        sealed public override Alignment2x2 commentsAlignment { get { return Alignment2x2.Bottom; } }
        sealed public override Alignment2x2 iconAlignment { get { return Alignment2x2.Default; } }

        public TransitionEvaluationMode transitionEvaluation {
            get { return _transitionEvaluation; }
            set { _transitionEvaluation = value; }
        }

        public float elapsedTime {
            get { return _elapsedTime; }
            private set { _elapsedTime = value; }
        }

        ///The FSM this state belongs to
        public FSM FSM {
            get { return (FSM)graph; }
        }

        ///Returns all transitions of the state
        public FSMConnection[] GetTransitions() {
            return outConnections.Cast<FSMConnection>().ToArray();
        }

        ///Declares that the state has finished
        public void Finish() { Finish(true); }
        public void Finish(bool inSuccess) { status = inSuccess ? Status.Success : Status.Failure; }

        ///----------------------------------------------------------------------------------------------

        public override void OnGraphStarted() { }
        public override void OnGraphStoped() { }
        public override void OnGraphPaused() { if ( status == Status.Running ) { OnPause(); } }

        ///----------------------------------------------------------------------------------------------

        //avoid connecting from same source
        protected override bool CanConnectFromSource(Node sourceNode) {
            if ( this.IsChildOf(sourceNode) ) {
                Logger.LogWarning("States are already connected together. Consider using multiple conditions on an existing transition instead", "Editor", this);
                return false;
            }
            return true;
        }

        //avoid connecting to same target
        protected override bool CanConnectToTarget(Node targetNode) {
            if ( this.IsParentOf(targetNode) ) {
                Logger.LogWarning("States are already connected together. Consider using multiple conditions on an existing transition instead", "Editor", this);
                return false;
            }
            return true;
        }

        //OnEnter...
        sealed protected override Status OnExecute(Component agent, IBlackboard bb) {

            if ( !_hasInit ) {
                _hasInit = true;
                OnInit();
            }

            if ( status == Status.Resting || status == Status.Running ) {
                status = Status.Running;

                for ( int i = 0; i < outConnections.Count; i++ ) {
                    if ( ( (FSMConnection)outConnections[i] ).condition != null ) {
                        ( (FSMConnection)outConnections[i] ).condition.Enable(agent, bb);
                    }
                }

                OnEnter();
            }

            return status;
        }

        //OnUpdate...
        public void Update() {

            elapsedTime += Time.deltaTime;

            if ( transitionEvaluation == TransitionEvaluationMode.CheckContinuously ) {
                CheckTransitions();
            } else if ( transitionEvaluation == TransitionEvaluationMode.CheckAfterStateFinished && status != Status.Running ) {
                CheckTransitions();
            }

            if ( status == Status.Running ) {
                OnUpdate();
            }
        }

        ///Returns true if a transitions was valid and thus made
        public bool CheckTransitions() {

            for ( var i = 0; i < outConnections.Count; i++ ) {

                var connection = (FSMConnection)outConnections[i];
                var condition = connection.condition;

                if ( !connection.isActive ) {
                    continue;
                }

                if ( ( condition != null && condition.CheckCondition(graphAgent, graphBlackboard) ) || ( condition == null && status != Status.Running ) ) {
                    FSM.EnterState((FSMState)connection.targetNode);
                    connection.status = Status.Success; //editor vis
                    return true;
                }

                connection.status = Status.Failure; //editor vis
            }

            return false;
        }

        //OnExit...
        sealed protected override void OnReset() {
            for ( int i = 0; i < outConnections.Count; i++ ) {
                if ( ( (FSMConnection)outConnections[i] ).condition != null ) {
                    ( (FSMConnection)outConnections[i] ).condition.Disable();
                }
            }

            //call OnExit before reseting elapsedTime so that it's available in the OnExit override if needed.
            OnExit();
            elapsedTime = 0;
        }


        //Converted
        virtual protected void OnInit() { }
        virtual protected void OnEnter() { }
        virtual protected void OnUpdate() { }
        virtual protected void OnExit() { }
        virtual protected void OnPause() { }
        //




        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        private static GUIPort clickedPort { get; set; }
        private static int dragDropMisses { get; set; }

        class GUIPort
        {
            public FSMState parent { get; private set; }
            public Vector2 pos { get; private set; }
            public GUIPort(FSMState parent, Vector2 pos) {
                this.parent = parent;
                this.pos = pos;
            }
        }

        //Draw the ports and connections
        sealed protected override void DrawNodeConnections(Rect drawCanvas, bool fullDrawPass, Vector2 canvasMousePos, float zoomFactor) {

            var e = Event.current;

            //Receive connections first
            if ( clickedPort != null && e.type == EventType.MouseUp && e.button == 0 ) {

                if ( rect.Contains(e.mousePosition) ) {
                    graph.ConnectNodes(clickedPort.parent, this);
                    clickedPort = null;
                    e.Use();

                } else {

                    dragDropMisses++;

                    if ( dragDropMisses == graph.allNodes.Count && clickedPort != null ) {
                        var source = clickedPort.parent;
                        var pos = Event.current.mousePosition;
                        var menu = new GenericMenu();
                        clickedPort = null;

                        menu.AddItem(new GUIContent("Add Action State"), false, () =>
                        {
                            var newState = graph.AddNode<ActionState>(pos);
                            graph.ConnectNodes(source, newState);
                        });

                        //PostGUI cause of zoom factors
                        GraphEditorUtility.PostGUI += () => { menu.ShowAsContext(); };
                        Event.current.Use();
                        e.Use();
                    }
                }
            }

            var portRectLeft = new Rect(0, 0, 20, 20);
            var portRectRight = new Rect(0, 0, 20, 20);
            var portRectBottom = new Rect(0, 0, 20, 20);

            portRectLeft.center = new Vector2(rect.x - 11, rect.center.y);
            portRectRight.center = new Vector2(rect.xMax + 11, rect.center.y);
            portRectBottom.center = new Vector2(rect.center.x, rect.yMax + 11);

            if ( maxOutConnections != 0 ) {
                if ( fullDrawPass || drawCanvas.Overlaps(rect) ) {
                    EditorGUIUtility.AddCursorRect(portRectLeft, MouseCursor.ArrowPlus);
                    EditorGUIUtility.AddCursorRect(portRectRight, MouseCursor.ArrowPlus);
                    EditorGUIUtility.AddCursorRect(portRectBottom, MouseCursor.ArrowPlus);

                    GUI.color = new Color(1, 1, 1, 0.3f);
                    GUI.DrawTexture(portRectLeft, StyleSheet.arrowLeft);
                    GUI.DrawTexture(portRectRight, StyleSheet.arrowRight);
                    if ( maxInConnections == 0 ) {
                        GUI.DrawTexture(portRectBottom, StyleSheet.arrowBottom);
                    }
                    GUI.color = Color.white;

                    if ( GraphEditorUtility.allowClick && e.type == EventType.MouseDown && e.button == 0 ) {

                        if ( portRectLeft.Contains(e.mousePosition) ) {
                            clickedPort = new GUIPort(this, portRectLeft.center);
                            dragDropMisses = 0;
                            e.Use();
                        }

                        if ( portRectRight.Contains(e.mousePosition) ) {
                            clickedPort = new GUIPort(this, portRectRight.center);
                            dragDropMisses = 0;
                            e.Use();
                        }

                        if ( maxInConnections == 0 && portRectBottom.Contains(e.mousePosition) ) {
                            clickedPort = new GUIPort(this, portRectBottom.center);
                            dragDropMisses = 0;
                            e.Use();
                        }
                    }
                }
            }

            //draw new linking
            if ( clickedPort != null && clickedPort.parent == this ) {
                Handles.DrawBezier(clickedPort.pos, e.mousePosition, clickedPort.pos, e.mousePosition, new Color(0.5f, 0.5f, 0.8f, 0.8f), null, 2);
            }

            //draw out connections
            for ( var i = 0; i < outConnections.Count; i++ ) {

                var connection = outConnections[i] as FSMConnection;
                var targetState = connection.targetNode as FSMState;
                if ( targetState == null ) { //In case of MissingNode type
                    continue;
                }
                var targetPos = targetState.GetConnectedInPortPosition(connection);
                var sourcePos = Vector2.zero;

                if ( rect.center.x <= targetPos.x ) {
                    sourcePos = portRectRight.center;
                }

                if ( rect.center.x > targetPos.x ) {
                    sourcePos = portRectLeft.center;
                }

                if ( maxInConnections == 0 && rect.center.y < targetPos.y - 50 && Mathf.Abs(rect.center.x - targetPos.x) < 200 ) {
                    sourcePos = portRectBottom.center;
                }

                var boundRect = RectUtils.GetBoundRect(sourcePos, targetPos);
                if ( fullDrawPass || drawCanvas.Overlaps(boundRect) ) {
                    connection.DrawConnectionGUI(sourcePos, targetPos);
                }
            }
        }


        //...
        Vector2 GetConnectedInPortPosition(Connection connection) {

            var sourcePos = connection.sourceNode.rect.center;
            var thisPos = rect.center;

            var style = 0;

            if ( style == 0 ) {
                if ( sourcePos.x <= thisPos.x ) {
                    if ( sourcePos.y <= thisPos.y ) {
                        return new Vector2(rect.center.x - 15, rect.yMin - ( this == graph.primeNode ? 20 : 0 ));
                    } else {
                        return new Vector2(rect.center.x - 15, rect.yMax + 2);
                    }
                }

                if ( sourcePos.x > thisPos.x ) {
                    if ( sourcePos.y <= thisPos.y ) {
                        return new Vector2(rect.center.x + 15, rect.yMin - ( this == graph.primeNode ? 20 : 0 ));
                    } else {
                        return new Vector2(rect.center.x + 15, rect.yMax + 2);
                    }
                }
            }

            // //Another idea
            // if (style == 1){
            // 	if (sourcePos.x <= thisPos.x){
            // 		if (sourcePos.y >= thisPos.y){
            // 			return new Vector2(rect.xMin - 3, rect.yMax - 10);
            // 		} else {
            // 			return new Vector2(rect.xMin - 3, rect.yMin + 10);
            // 		}
            // 	}
            // 	if (sourcePos.x > thisPos.x){
            // 		if (sourcePos.y >= thisPos.y){
            // 			return new Vector2(rect.center.x, rect.yMax + 2);
            // 		} else {
            // 			return new Vector2(rect.center.x, rect.yMin - (this == graph.primeNode? 20 : 0 ));
            // 		}
            // 	}
            // }

            // //YET Another idea
            // if (style >= 2){
            // 	if (sourcePos.x <= thisPos.x){
            // 		if (sourcePos.y >= thisPos.y){
            // 			return new Vector2(rect.xMin - 3, rect.yMax - 10);
            // 		} else {
            // 			return new Vector2(rect.xMin - 3, rect.yMin + 10);
            // 		}
            // 	}
            // 	if (sourcePos.x > thisPos.x){
            // 		if (sourcePos.y >= thisPos.y){
            // 			return new Vector2(rect.xMax + 3, rect.yMax - 10);
            // 		} else {
            // 			return new Vector2(rect.xMax + 3, rect.yMin + 10);
            // 		}
            // 	}
            // }

            return thisPos;
        }

        //...
        protected override void OnNodeInspectorGUI() {
            ShowBaseFSMInspectorGUI();
            DrawDefaultInspector();
        }

        //...
        protected override GenericMenu OnContextMenu(GenericMenu menu) {
            if ( allowAsPrime ) {
                if ( Application.isPlaying ) {
                    menu.AddItem(new GUIContent("Enter State"), false, () => { FSM.EnterState(this); });
                } else {
                    menu.AddDisabledItem(new GUIContent("Enter State"));
                }
                menu.AddItem(new GUIContent("Breakpoint"), isBreakpoint, () => { isBreakpoint = !isBreakpoint; });
            }
            return menu;
        }

        //...
        protected void ShowBaseFSMInspectorGUI() {

            EditorUtils.CoolLabel("Transitions");

            if ( outConnections.Count == 0 ) {
                GUI.backgroundColor = new Color(1, 1, 1, 0.5f);
                GUILayout.BeginHorizontal("box");
                GUILayout.Label("No Transitions");
                GUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
            }

            var onFinishExists = false;
            EditorUtils.ReorderableList(outConnections, (i, picked) =>
            {
                var connection = (FSMConnection)outConnections[i];
                GUI.backgroundColor = new Color(1, 1, 1, 0.5f);
                GUILayout.BeginHorizontal("box");
                if ( connection.condition != null ) {
                    GUILayout.Label(connection.condition.summaryInfo, GUILayout.MinWidth(0), GUILayout.ExpandWidth(true));
                } else {
                    GUILayout.Label("OnFinish" + ( onFinishExists ? " (exists)" : string.Empty ), GUILayout.MinWidth(0), GUILayout.ExpandWidth(true));
                    onFinishExists = true;
                }

                GUILayout.FlexibleSpace();
                GUILayout.Label("--> '" + connection.targetNode.name + "'");
                if ( GUILayout.Button("►") ) {
                    GraphEditorUtility.activeElement = connection;
                }

                GUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
            });

            if ( allowAsPrime ) {
                transitionEvaluation = (TransitionEvaluationMode)EditorGUILayout.EnumPopup(transitionEvaluation);
            }

            EditorUtils.BoldSeparator();
        }

#endif
    }
}