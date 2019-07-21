#if UNITY_EDITOR

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Editor
{

    [CustomEditor(typeof(GlobalBlackboard))]
    public class GlobalBlackboardInspector : UnityEditor.Editor
    {

        private GlobalBlackboard bb {
            get { return (GlobalBlackboard)target; }
        }

        public override void OnInspectorGUI() {

            if ( Event.current.isMouse ) {
                Repaint();
            }

            UndoManager.CheckUndo(bb, "Global Blackboard");
            GUI.backgroundColor = GlobalBlackboard.allGlobals.Find(b => b.name == bb.name && b != bb) ? Color.red : Color.white;
            bb.name = EditorGUILayout.TextField("Unique Name", bb.name);
            GUI.backgroundColor = Color.white;

            bb.dontDestroy = EditorGUILayout.Toggle("DontDestroyOnLoad", bb.dontDestroy);
            if ( GUI.changed ) {
                UndoManager.CheckDirty(bb);
            }


            BlackboardEditor.ShowVariables(bb, bb);
            EditorUtils.EndOfInspector();
            if ( Application.isPlaying ) {
                Repaint();
            }
        }
    }
}

#endif