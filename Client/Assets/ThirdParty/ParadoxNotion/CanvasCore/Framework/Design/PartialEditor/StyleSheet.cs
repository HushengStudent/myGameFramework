#if UNITY_EDITOR

using UnityEngine;
using NodeCanvas.Framework;

namespace NodeCanvas.Editor
{

    ///Styling database (styles, icons, colors)
    //[CreateAssetMenu]
    public partial class StyleSheet : ScriptableObject
    {

        private static StyleSheet _styleSheet;
        private static StyleSheet styleSheet {
            get { return _styleSheet ?? ( _styleSheet = Resources.Load<StyleSheet>(UnityEditor.EditorGUIUtility.isProSkin ? "StyleSheet/StyleSheetDark" : "StyleSheet/StyleSheetLight") ); }
        }

        [ContextMenu("Lock")] void Lock() { hideFlags = HideFlags.NotEditable; }
        [ContextMenu("UnLock")] void UnLock() { hideFlags = HideFlags.None; }

        [UnityEditor.InitializeOnLoadMethod]
        static void Load() {
            _styleSheet = styleSheet;
        }

        //NotEditable flags (lock)
        void OnValidate() {
            hideFlags = HideFlags.NotEditable;
        }

        ///----------------------------------------------------------------------------------------------

        [System.Serializable]
        public class Styles
        {
            public GUIStyle window;
            public GUIStyle windowShadow;
            public GUIStyle windowHighlight;
            public GUIStyle windowHeader;
            public GUIStyle windowTitle;

            public GUIStyle button;
            public GUIStyle box;
            public GUIStyle labelOnCanvas;
            public GUIStyle commentsBox;

            public GUIStyle nodePortContainer;
            public GUIStyle nodePortConnected;
            public GUIStyle nodePortEmpty;

            public GUIStyle scaleArrowBR;
            public GUIStyle scaleArrowTL;

            public GUIStyle canvasBG;
            public GUIStyle canvasBorders;
            public GUIStyle editorPanel;
        }

        [System.Serializable]
        public class Icons
        {
            [Header("Colorized")]
            public Texture2D statusSuccess;
            public Texture2D statusFailure;
            public Texture2D statusRunning;

            public Texture2D circle;
            public Texture2D arrowLeft;
            public Texture2D arrowRight;
            public Texture2D arrowTop;
            public Texture2D arrowBottom;

            [Header("Fixed")]
            public Texture2D canvasIcon;
            public Texture2D log;
            public Texture2D lens;
            public Texture2D verboseLevel1;
            public Texture2D verboseLevel2;
            public Texture2D verboseLevel3;
        }

        ///----------------------------------------------------------------------------------------------

        public Styles styles;
        public Icons icons;

        ///----------------------------------------------------------------------------------------------

        public static GUIStyle window {
            get { return styleSheet.styles.window; }
        }

        public static GUIStyle windowShadow {
            get { return styleSheet.styles.windowShadow; }
        }

        public static GUIStyle windowHighlight {
            get { return styleSheet.styles.windowHighlight; }
        }

        public static GUIStyle windowHeader {
            get { return styleSheet.styles.windowHeader; }
        }

        public static GUIStyle windowTitle {
            get { return styleSheet.styles.windowTitle; }
        }

        public static GUIStyle button {
            get { return styleSheet.styles.button; }
        }

        public static GUIStyle box {
            get { return styleSheet.styles.box; }
        }

        public static GUIStyle labelOnCanvas {
            get { return styleSheet.styles.labelOnCanvas; }
        }

        public static GUIStyle commentsBox {
            get { return styleSheet.styles.commentsBox; }
        }

        public static GUIStyle nodePortContainer {
            get { return styleSheet.styles.nodePortContainer; }
        }

        public static GUIStyle nodePortConnected {
            get { return styleSheet.styles.nodePortConnected; }
        }

        public static GUIStyle nodePortEmpty {
            get { return styleSheet.styles.nodePortEmpty; }
        }

        public static GUIStyle scaleArrowBR {
            get { return styleSheet.styles.scaleArrowBR; }
        }

        public static GUIStyle scaleArrowTL {
            get { return styleSheet.styles.scaleArrowTL; }
        }

        public static GUIStyle canvasBG {
            get { return styleSheet.styles.canvasBG; }
        }

        public static GUIStyle canvasBorders {
            get { return styleSheet.styles.canvasBorders; }
        }

        public static GUIStyle editorPanel {
            get { return styleSheet.styles.editorPanel; }
        }

        ///----------------------------------------------------------------------------------------------

        public static Texture2D canvasIcon {
            get { return styleSheet.icons.canvasIcon; }
        }

        public static Texture2D statusSuccess {
            get { return styleSheet.icons.statusSuccess; }
        }

        public static Texture2D statusFailure {
            get { return styleSheet.icons.statusFailure; }
        }

        public static Texture2D statusRunning {
            get { return styleSheet.icons.statusRunning; }
        }

        public static Texture2D circle {
            get { return styleSheet.icons.circle; }
        }

        public static Texture2D arrowLeft {
            get { return styleSheet.icons.arrowLeft; }
        }

        public static Texture2D arrowRight {
            get { return styleSheet.icons.arrowRight; }
        }

        public static Texture2D arrowTop {
            get { return styleSheet.icons.arrowTop; }
        }

        public static Texture2D arrowBottom {
            get { return styleSheet.icons.arrowBottom; }
        }


        public static Texture2D log {
            get { return styleSheet.icons.log; }
        }
        public static Texture2D lens {
            get { return styleSheet.icons.lens; }
        }


        public static Texture2D verboseLevel1 {
            get { return styleSheet.icons.verboseLevel1; }
        }
        public static Texture2D verboseLevel2 {
            get { return styleSheet.icons.verboseLevel2; }
        }
        public static Texture2D verboseLevel3 {
            get { return styleSheet.icons.verboseLevel3; }
        }

        ///----------------------------------------------------------------------------------------------

        ///Return an arrow based on direction vector
        public static Texture2D GetDirectionArrow(Vector2 dir) {
            if ( dir.normalized == Vector2.left ) { return arrowLeft; }
            if ( dir.normalized == Vector2.right ) { return arrowRight; }
            if ( dir.normalized == Vector2.up ) { return arrowTop; }
            if ( dir.normalized == Vector2.down ) { return arrowBottom; }
            return circle;
        }

        ///Return icon based on status
        public static Texture2D GetStatusIcon(Status status) {
            if ( status == Status.Success ) { return statusSuccess; }
            if ( status == Status.Failure ) { return statusFailure; }
            if ( status == Status.Running ) { return statusRunning; }
            return null;
        }

        ///Return color based on status
        public static Color GetStatusColor(Status status) {
            switch ( status ) {
                case ( Status.Failure ): return new Color(1.0f, 0.3f, 0.3f);
                case ( Status.Success ): return new Color(0.4f, 0.7f, 0.2f);
                case ( Status.Running ): return Color.yellow;
                case ( Status.Resting ): return new Color(0.7f, 0.7f, 1f, 0.8f);
                case ( Status.Error ): return Color.red;
                case ( Status.Optional ): return Color.grey;
            }
            return Color.white;
        }
    }
}

#endif
