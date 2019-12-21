/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/6/28 2:39:37
** desc:  单例基类;
*********************************************************************************/


namespace Framework
{
    public delegate void OnInitializeEventHandler();
    public delegate void OnUninitializeEventHandler();

    public class Singletoninterface
    {
        public virtual void Launch() { }
        public virtual void SingletoninterfaceOnInitialize() { }
        public virtual void SingletoninterfaceOnUninitialize() { }
    }

    public class SingletonBase : Singletoninterface
    {
        public event OnInitializeEventHandler OnInitializeHandler;
        public event OnUninitializeEventHandler OnUninitializeHandler;

        private bool isInit = false;
        private bool isUninit = false;

        public sealed override void Launch() { }

        public sealed override void SingletoninterfaceOnInitialize()
        {
            if (!isInit)
            {
                isInit = true;
                OnInitialize();
                OnInitializeHandler?.Invoke();
            }
        }

        public sealed override void SingletoninterfaceOnUninitialize()
        {
            if (!isUninit)
            {
                isUninit = true;
                OnUninitializeHandler?.Invoke();
                OnUninitialize();
            }
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnUninitialize() { }
    }
}
