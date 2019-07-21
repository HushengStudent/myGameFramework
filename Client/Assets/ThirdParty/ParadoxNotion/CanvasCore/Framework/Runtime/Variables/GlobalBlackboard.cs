using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NodeCanvas.Framework
{

    /// Global Blackboards are accessible from any BBParameter. Their name must be unique
    [ExecuteInEditMode]
    public class GlobalBlackboard : Blackboard
    {

        ///A list of all the current active global blackboards in the scene
        public static List<GlobalBlackboard> allGlobals = new List<GlobalBlackboard>();
        ///Flag to not destroy on load.
        public bool dontDestroy = true;

        ///Name of the global blackboard
        new public string name {
            get { return base.name; }
            set
            {
                if ( base.name != value ) {
                    base.name = value;
                    if ( !IsUnique() ) {
                        Debug.LogError("Another Blackboard has the same name. Please rename either.", gameObject);
                    }
                }
            }
        }


        public static GlobalBlackboard Create() {
            var bb = new GameObject("@GlobalBlackboard").AddComponent<GlobalBlackboard>();
            bb.name = "Global";
            return bb;
        }

        ///A convenient way to find and get a global blackboard by it's name
        public static GlobalBlackboard Find(string name) {
            if ( !Application.isPlaying ) {
                return FindObjectsOfType<GlobalBlackboard>().Where(b => b.name == name).FirstOrDefault();
            }
            var result = allGlobals.Find(b => b.name == name);
            //since bbs are registered in list in awake, if a script request a bb in it's own awake it might not found correctly.
            if ( result == null ) {
                result = FindObjectsOfType<GlobalBlackboard>().Where(b => b.name == name).FirstOrDefault();
                if ( result != null ) {
                    allGlobals.Add(result);
                }
            }
            return result;
        }

        protected override void Awake() {

            base.Awake();

#if UNITY_EDITOR
            if ( UnityEditor.EditorUtility.IsPersistent(this) ) {
                return;
            }
#endif

            if ( !allGlobals.Contains(this) ) {
                allGlobals.Add(this);
            }

            if ( Application.isPlaying ) {
                if ( IsUnique() ) {
                    if ( dontDestroy ) {
                        DontDestroyOnLoad(this.gameObject);
                    }
                } else {
                    Debug.Log(string.Format("There exist more than one Global Blackboards with same name '{0}'. The old one will be destroyed and replaced with the new one.", name));
                    Destroy(this.gameObject);
                }
            }

            if ( !Application.isPlaying && !IsUnique() ) {
                Debug.LogError(string.Format("There is a duplicate <b>GlobalBlackboard</b> named '{0}' in the scene. Please rename it.", name), this);
            }
        }

        void OnDestroy() {
            allGlobals.Remove(this);
        }

        bool IsUnique() {
            return allGlobals.Find(b => b.name == this.name && b != this) == null;
        }


        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        void OnValidate() {

            if ( UnityEditor.EditorUtility.IsPersistent(this) ) {
                return;
            }

            if ( !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode ) {
                if ( !allGlobals.Contains(this) ) {
                    allGlobals.Add(this);
                }
            }
        }

#endif

    }
}