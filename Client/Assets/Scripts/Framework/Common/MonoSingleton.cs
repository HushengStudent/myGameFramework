/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/07 16:39:13
** desc:  单例模板;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class MonoSingleton<T> : MonoSingletonBase where T : MonoBehaviour
    {
        private static readonly string _monoSingletonRoot = "@MonoSingletonRoot";

        protected static T _instance = null;

        private static bool _applicationIsPlaying = true;
        public static bool ApplicationIsPlaying { get { return _applicationIsPlaying; } }

        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    GameObject go = GameObject.Find(_monoSingletonRoot);
                    if (null == go)
                    {
                        go = new GameObject(_monoSingletonRoot);
                        DontDestroyOnLoad(go);
                    }
                    _instance = go.AddComponent<T>();
                    MonoSingletoninterface singleton = _instance as MonoSingletoninterface;
                    if (singleton != null)
                    {
                        singleton.MonoSingletoninterfaceOnInitialize();
                    }
                }
                return _instance;
            }
        }

        /// 构造函数;
        protected MonoSingleton()
        {
            if (null == _instance)
            {
                LogHelper.PrintError("[MonoSingleton]" + (typeof(T)).ToString() + " singleton instance created.");
            }
        }

        protected virtual void StartEx() { }
        protected virtual void AwakeEx() { }
        protected virtual void OnEnableEx() { }
        protected virtual void FixedUpdateEx(float interval) { }
        protected virtual void UpdateEx(float interval) { }
        protected virtual void LateUpdateEx(float interval) { }
        protected virtual void OnDisableEx() { }
        protected virtual void OnDestroyEx() { }

        void Awake()
        {
            AwakeEx();
        }
        void Start()
        {
            StartEx();
        }
        void OnEnable()
        {
            OnEnableEx();
        }
        void FixedUpdate()
        {
            FixedUpdateEx(Time.deltaTime);
        }
        void Update()
        {
            UpdateEx(Time.deltaTime);
        }
        void LateUpdate()
        {
            LateUpdateEx(Time.deltaTime);
        }
        void OnDisable()
        {
            OnDisableEx();
        }
        void OnDestroy()
        {
            MonoSingletoninterface singleton = _instance as MonoSingletoninterface;
            if (singleton != null)
            {
                singleton.MonoSingletoninterfaceOnUninitialize();
            }
            OnDestroyEx();
            _applicationIsPlaying = false;
            _instance = null;
        }
    }
}
