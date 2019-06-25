/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 23:37:56
** desc:  单例模板;
*********************************************************************************/

namespace Framework
{
    public class Singleton<T> where T : class, ISingleton, new()
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
                    LogHelper.Print((typeof(T)).ToString() + " singleton instance Initialize.");
                    _instance.OnInitialize();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 构造函数;
        /// </summary>
        protected Singleton()
        {
            if (null == _instance)
            {
                LogHelper.Print((typeof(T)).ToString() + " singleton instance created.");
                CreateInstance();
            }
        }

        protected virtual void CreateInstance() { }
    }
}
