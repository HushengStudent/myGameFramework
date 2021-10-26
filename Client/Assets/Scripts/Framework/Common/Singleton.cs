/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 23:37:56
** desc:  单例模板;
*********************************************************************************/

namespace Framework
{
    public class Singleton<T> : SingletonBase where T : class, new()
    {
        private static T _singleton = null;
        private static readonly object _lock = new object();

        public static T singleton
        {
            get
            {
                if (null == _singleton)
                {
                    lock (_lock)
                    {
                        _singleton = new T(); //调用构造函数;
                        if (_singleton is SingletonInterface singleton)
                        {
                            singleton.SingletonInterfaceOnInitialize();
                        }
                    }
                }
                return _singleton;
            }
        }

        /// 构造函数;
        protected Singleton()
        {
            if (null == _singleton)
            {
                LogHelper.PrintGreen($"[Singleton]{typeof(T).ToString()} singleton instance created.");
            }
        }
    }
}
