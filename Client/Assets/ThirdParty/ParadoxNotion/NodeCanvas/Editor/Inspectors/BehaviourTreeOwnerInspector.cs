#if UNITY_EDITOR

using NodeCanvas.Framework;
using NodeCanvas.BehaviourTrees;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace NodeCanvas.Editor
{

    [CustomEditor(typeof(BehaviourTreeOwner))]
    public class BehaviourTreeOwnerInspector : GraphOwnerInspector
    {

        private BehaviourTreeOwner owner {
            get { return target as BehaviourTreeOwner; }
        }

        protected override void OnExtraOptions() {
            owner.repeat = EditorGUILayout.Toggle("Repeat", owner.repeat);
            if ( owner.repeat ) {
                GUI.color = owner.updateInterval > 0 ? Color.white : new Color(1, 1, 1, 0.5f);
                owner.updateInterval = EditorGUILayout.FloatField("Update Interval", owner.updateInterval);
                GUI.color = Color.white;
            }
        }

        protected override void OnGrapOwnerControls() {
            if ( GUILayout.Button(Icons.stepIcon) ) {
                owner.PauseBehaviour();
                owner.UpdateBehaviour();
            }
        }
    }
}

#endif