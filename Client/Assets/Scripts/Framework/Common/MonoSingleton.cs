/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/07 16:39:13
** desc:  单例模板;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance = null;

        private float _fixedUpdate;
        private float _lastUpdate;
        private float _lateUpdate;

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
                }
                return _instance;
            }
        }

        /// <summary>
        /// 构造函数;
        /// </summary>
        protected MonoSingleton()
        {
            if (null != _instance)
                LogUtil.LogUtility.Print("This " + (typeof(T)).ToString() + " Singleton Instance is not null!");
            InitEx();
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
            _fixedUpdate = Time.realtimeSinceStartup;
            _lastUpdate = Time.realtimeSinceStartup;
            _lateUpdate = Time.realtimeSinceStartup;
            AwakeEx();
        }
        void Start() { StartEx(); }
        void OnEnable() { OnEnableEx(); }
        void FixedUpdate()
        {
            FixedUpdateEx(Time.realtimeSinceStartup - _fixedUpdate);
            _fixedUpdate = Time.realtimeSinceStartup;
        }
        void Update()
        {
            UpdateEx(Time.realtimeSinceStartup - _lastUpdate);
            _lastUpdate = Time.realtimeSinceStartup;
        }
        void LateUpdate()
        {
            LateUpdateEx(Time.realtimeSinceStartup - _lateUpdate);
            _lateUpdate = Time.realtimeSinceStartup;
        }
        void OnDisable() { OnDisableEx(); }
        void OnDestroy()
        {
            _instance = null;
            OnDestroyEx();
        }

        protected virtual void InitEx() { }
        public void Init() { }
    }
}
