/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 23:37:56
** desc:  单例模板;
*********************************************************************************/

namespace Framework
{
    public class Singleton<T> : SingletonBase where T : class, new()
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

                        Singletoninterface singleton = _instance as Singletoninterface;
                        if (singleton != null)
                        {
                            singleton.SingletoninterfaceOnInitialize();
                        }
                    }
                }
                return _instance;
            }
        }

        /// 构造函数;
        protected Singleton()
        {
            if (null == _instance)
            {
                LogHelper.Print("[Singleton]" + (typeof(T)).ToString() + " singleton instance created.");
            }
        }
    }
}
