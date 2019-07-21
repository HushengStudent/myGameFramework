using System.Collections.Generic;
using System;
using UnityEngine;


namespace ParadoxNotion.Services
{

    ///Singleton. Automatically added when needed, collectively calls methods that needs updating amongst other things relevant to MonoBehaviours
    public class MonoManager : MonoBehaviour
    {

        public event Action onUpdate;
        public event Action onLateUpdate;
        public event Action onFixedUpdate;
        public event Action onGUI;
        public event Action onApplicationQuit;
        public event Action<bool> onApplicationPause;

        public static bool isQuiting { get; private set; }

        private static MonoManager _current;
        public static MonoManager current {
            get
            {
                if ( _current == null && !isQuiting ) {
                    _current = FindObjectOfType<MonoManager>();
                    if ( _current == null ) {
                        _current = new GameObject("_MonoManager").AddComponent<MonoManager>();
                    }
                }
                return _current;
            }
        }


        ///Creates the MonoManager singleton
        public static void Create() { _current = current; }

        protected void OnApplicationQuit() {
            isQuiting = true;
            if ( onApplicationQuit != null ) {
                onApplicationQuit();
            }
        }

        protected void OnApplicationPause(bool isPause) {
            if ( onApplicationPause != null ) {
                onApplicationPause(isPause);
            }
        }

        protected void Awake() {
            if ( _current != null && _current != this ) {
                DestroyImmediate(this.gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            _current = this;
        }

        protected void Update() {
            if ( onUpdate != null ) { onUpdate(); }
        }

        protected void LateUpdate() {
            if ( onLateUpdate != null ) { onLateUpdate(); }
        }

        protected void FixedUpdate() {
            if ( onFixedUpdate != null ) { onFixedUpdate(); }
        }

#if UNITY_EDITOR
        protected void OnGUI() {
            if ( onGUI != null ) { onGUI(); }
        }
#endif

    }
}