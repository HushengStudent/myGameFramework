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
        protected static T instance = null;

        private float _fixedUpdate;
        private float _lastUpdate;
        private float _lateUpdate;

        public static T Instance
        {
            get
            {
                if (null == instance)
                {
                    GameObject go = GameObject.Find("~!@#$%^&*()_+_monoSingleton_");
                    if (null == go)
                    {
                        go = new GameObject("~!@#$%^&*()_+_monoSingleton_");
                        DontDestroyOnLoad(go);
                    }
                    instance = go.AddComponent<T>();
                }
                return instance;
            }
        }

        /// <summary>
        /// 构造函数;
        /// </summary>
        protected MonoSingleton()
        {
            if (null != instance)
                LogUtil.LogUtility.Print("This " + (typeof(T)).ToString() + " Singleton Instance is not null!");
        }

        public virtual void StartEx() { }
        public virtual void AwakeEx() { }
        public virtual void OnEnableEx() { }
        public virtual void FixedUpdateEx(float interval) { }
        public virtual void UpdateEx(float interval) { }
        public virtual void LateUpdateEx(float interval) { }
        public virtual void OnDisableEx() { }
        public virtual void OnDestroyEx() { }

        void Start() { StartEx(); }
        void Awake()
        {
            _fixedUpdate = Time.realtimeSinceStartup;
            _lastUpdate = Time.realtimeSinceStartup;
            _lateUpdate = Time.realtimeSinceStartup;
            AwakeEx();
        }
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
            instance = null;
            OnDestroyEx();
        }
    }
}
