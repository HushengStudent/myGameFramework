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
        public virtual void MonoSingletoninterfaceOnInitialize() { }
        public virtual void MonoSingletoninterfaceOnUninitialize() { }
    }

    public class MonoSingletonBase : MonoSingletoninterface
    {
        public event OnInitializeFinishedHandler OnInitializeHandler;
        public event OnUninitializeStartHandler OnUninitializeHandler;

        private bool isInit = false;
        private bool isUninit = false;

        public sealed override void Launch() { }

        public sealed override void MonoSingletoninterfaceOnInitialize()
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

        public sealed override void MonoSingletoninterfaceOnUninitialize()
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
