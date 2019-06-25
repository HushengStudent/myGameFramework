/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/07 16:39:13
** desc:  单例模板;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour, ISingleton
    {
        protected static T _instance = null;

        private bool _applicationIsPlaying = true;
        public bool ApplicationIsPlaying { get { return _applicationIsPlaying; } }

        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    GameObject go = GameObject.Find("~!@#$%^&*()_+_monoSingleton_");
                    if (null == go)
                    {
                        go = new GameObject("~!@#$%^&*()_+_monoSingleton_");
                        DontDestroyOnLoad(go);
                    }
                    _instance = go.AddComponent<T>();
                    LogHelper.Print((typeof(T)).ToString() + " singleton instance Initialize.");
                    _instance.OnInitialize();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 构造函数;
        /// </summary>
        protected MonoSingleton()
        {
            if (null == _instance)
            {
                LogHelper.Print((typeof(T)).ToString() + " singleton instance created.");
                CreateInstance();
            }
        }

        protected virtual void CreateInstance() { }

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
            _applicationIsPlaying = false;
            _instance = null;
            OnDestroyEx();
        }
    }
}
