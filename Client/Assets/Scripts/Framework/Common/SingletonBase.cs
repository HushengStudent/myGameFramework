/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/6/28 2:39:37
** desc:  单例基类;
*********************************************************************************/


namespace Framework
{
    public delegate void onInitializeFinishedHandler();
    public delegate void onUninitializeStartHandler();

    public class Singletoninterface
    {
        public virtual void Launch() { }
        public virtual void SingletoninterfaceOnInitialize() { }
        public virtual void SingletoninterfaceOnUninitialize() { }
    }

    public class SingletonBase : Singletoninterface
    {
        public onInitializeFinishedHandler _onInitializeFinishedHandler = delegate { };
        public onUninitializeStartHandler _onUninitializeStartHandler = delegate { };

        private bool isInit = false;
        private bool isUninit = false;

        public sealed override void Launch() { }

        public sealed override void SingletoninterfaceOnInitialize()
        {
            if (!isInit)
            {
                isInit = true;
                OnInitialize();
                if (_onInitializeFinishedHandler != null)
                {
                    _onInitializeFinishedHandler();
                }
            }
        }

        public sealed override void SingletoninterfaceOnUninitialize()
        {
            if (!isUninit)
            {
                isUninit = true;
                if (_onUninitializeStartHandler != null)
                {
                    _onUninitializeStartHandler();
                }
                OnUninitialize();
            }
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnUninitialize() { }
    }
}
