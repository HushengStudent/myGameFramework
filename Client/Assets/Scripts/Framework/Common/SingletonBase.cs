/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/6/28 2:39:37
** desc:  单例基类;
*********************************************************************************/


namespace Framework
{
    public delegate void OnInitializeFinishedHandler();
    public delegate void OnUninitializeStartHandler();

    public class Singletoninterface
    {
        public virtual void Launch() { }
        public virtual void SingletoninterfaceOnInitialize() { }
        public virtual void SingletoninterfaceOnUninitialize() { }
    }

    public class SingletonBase : Singletoninterface
    {
        public OnInitializeFinishedHandler OnInitializeHandler;
        public OnUninitializeStartHandler OnUninitializeHandler;

        private bool isInit = false;
        private bool isUninit = false;

        public sealed override void Launch() { }

        public sealed override void SingletoninterfaceOnInitialize()
        {
            if (!isInit)
            {
                isInit = true;
                OnInitialize();
                if (OnInitializeHandler != null)
                {
                    OnInitializeHandler();
                }
            }
        }

        public sealed override void SingletoninterfaceOnUninitialize()
        {
            if (!isUninit)
            {
                isUninit = true;
                if (OnUninitializeHandler != null)
                {
                    OnUninitializeHandler();
                }
                OnUninitialize();
            }
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnUninitialize() { }
    }
}
