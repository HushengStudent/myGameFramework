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
        private static readonly string _monoSingletonRoot = "@GameRoot";

        private static T _singleton = null;
        public static T singleton
        {
            get
            {
                if (null == _singleton)
                {
                    var go = GameObject.Find(_monoSingletonRoot);
                    if (go)
                    {
                        _singleton = go.AddComponent<T>();
                        var singleton = _singleton as MonoSingletoninterface;
                        if (singleton != null)
                        {
                            //OnInitialize晚于AwakeEx执行;
                            singleton.MonoSingletoninterfaceOnInitialize();
                        }
                    }
                }
                return _singleton;
            }
        }

        public static bool ApplicationIsPlaying
        {
            get
            {
                return _singleton != null;
            }
        }

        /// 构造函数;
        protected MonoSingleton()
        {
            if (null == _singleton)
            {
                LogHelper.PrintGreen($"[MonoSingleton]{typeof(T).ToString()} singleton instance created.");
            }
        }

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

        //MonoSingleton只有在ApplicationQuit时才会Destroy;
        void OnApplicationQuit()
        {
            var singleton = _singleton as MonoSingletoninterface;
            if (singleton != null)
            {
                singleton.MonoSingletoninterfaceOnUninitialize();
            }
            OnDestroyEx();
            _singleton = null;
        }

        protected virtual void AwakeEx() { }
        protected virtual void StartEx() { }
        protected virtual void OnEnableEx() { }
        protected virtual void FixedUpdateEx(float interval) { }
        protected virtual void UpdateEx(float interval) { }
        protected virtual void LateUpdateEx(float interval) { }
        protected virtual void OnDisableEx() { }
        protected virtual void OnDestroyEx() { }
    }
}
