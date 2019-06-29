/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/6/29 17:26:38
** desc:  单例基类;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class MonoSingletoninterface : MonoBehaviour
    {
        public virtual void Launch() { }
        public virtual void OnInitialize() { }
        public virtual void OnUninitialize() { }
    }

    public class MonoSingletonBase : MonoSingletoninterface
    {
        private onInitializeFinishedHandler _onInitializeFinishedHandler = delegate { };
        private onUninitializeStartHandler _onUninitializeStartHandler = delegate { };

        private bool isInit = false;
        private bool isUninit = false;

        public sealed override void Launch() { }

        public sealed override void OnInitialize()
        {
            if (!isInit)
            {
                isInit = true;
                OnInitializeEx();
                if (_onInitializeFinishedHandler != null)
                {
                    _onInitializeFinishedHandler();
                }
            }

        }

        public sealed override void OnUninitialize()
        {
            if (!isUninit)
            {
                isUninit = true;
                if (_onUninitializeStartHandler != null)
                {
                    _onUninitializeStartHandler();
                }
                OnUninitializeEx();
            }
        }

        protected virtual void OnInitializeEx() { }
        protected virtual void OnUninitializeEx() { }

        public void RegisterOnInitialize(onInitializeFinishedHandler handler)
        {
            _onInitializeFinishedHandler += handler;
        }
        public void UnRegisterOnInitialize(onInitializeFinishedHandler handler)
        {
            _onInitializeFinishedHandler -= handler;
        }
        public void RegisterOnUninitialize(onUninitializeStartHandler handler)
        {
            _onUninitializeStartHandler += handler;
        }
        public void UnRegisterOnUninitialize(onUninitializeStartHandler handler)
        {
            _onUninitializeStartHandler -= handler;
        }
    }
}
