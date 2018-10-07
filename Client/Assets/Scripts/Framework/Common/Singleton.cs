/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 23:37:56
** desc:  单例模板;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance = null;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (_lock)
                    {
                        _instance = new T(); //调用构造函数;
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 构造函数;
        /// </summary>
        protected Singleton()
        {
            if (null != _instance)
                LogUtil.LogUtility.Print("This " + (typeof(T)).ToString() + " Singleton Instance is not null!");
        }

        public virtual void Init() { }
    }
}
