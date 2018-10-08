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
            {
                LogUtil.LogUtility.PrintWarning((typeof(T)).ToString() + " singleton Instance is not null.");
            }
            else
            {
                LogUtil.LogUtility.Print((typeof(T)).ToString() + " singleton Instance created.");
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
        void Start() { StartEx(); }
        void OnEnable() { OnEnableEx(); }
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
        void OnDisable() { OnDisableEx(); }
        void OnDestroy()
        {
            _instance = null;
            OnDestroyEx();
        }

        public virtual void Init()
        {
            LogUtil.LogUtility.Print((typeof(T)).ToString() + " singleton Instance Init.");
        }
    }
}
