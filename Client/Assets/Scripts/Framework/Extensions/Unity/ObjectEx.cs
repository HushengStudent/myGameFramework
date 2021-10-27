/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/20 01:44:34
** desc:  Object��չ;
*********************************************************************************/

using System.Collections.Generic;

namespace Framework
{
    public class ObjectEx : IPool
    {
        protected ObjectEx()
        {
            ID = IdGenerateHelper.GenerateId();
            Enable = false;
        }

        private List<AbsComponent> _componentList = new List<AbsComponent>();

        public long ID { get; private set; }
        public bool Enable { get; set; }

        protected void InternalInitialize()
        {
            _componentList.Clear();
            RegisterComponent();
            EventSubscribe();
            InitializeEx();
        }

        protected void InternalUnInitialize()
        {
            EventUnsubscribe();
            EventMgr.singleton.RemoveEvent(this);
            UnRegisterComponent();
            for (var i = 0; i < _componentList.Count; i++)
            {
                DestroyComponent(_componentList[i]);
            }
            _componentList.Clear();
            UnInitializeEx();
        }

        /// <summary>
        /// ��ʼ��;
        /// </summary>
        protected virtual void InitializeEx() { }
        /// <summary>
        /// ����;
        /// </summary>
        protected virtual void UnInitializeEx() { }
        /// <summary>
        /// ע�����;
        /// </summary>
        protected virtual void RegisterComponent() { }
        /// <summary>
        /// ɾ�����;
        /// </summary>
        protected virtual void UnRegisterComponent() { }
        /// <summary>
        /// ע���¼�;
        /// </summary>
        protected virtual void EventSubscribe() { }
        /// <summary>
        /// ע���¼�;
        /// </summary>
        protected virtual void EventUnsubscribe() { }
        /// <summary>
        /// ���볡��;
        /// </summary>
        /// <param name="sceneId"></param>
        protected virtual void OnEnterScene(int sceneId) { }
        /// <summary>
        /// �뿪����;
        /// </summary>
        /// <param name="sceneId"></param>
        protected virtual void OnExitScene(int sceneId) { }

        /// <summary>
        /// ��ȡ���;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : AbsComponent, new()
        {
            for (var i = 0; i < _componentList.Count; i++)
            {
                if (_componentList[i] is T target)
                {
                    return target;
                }
            }
            /// LogHelper.PrintError($"[ObjectEx]GetComponent error,not find component:{typeof(T).ToString()}.");
            return null;
        }

        /// <summary>
        /// ������;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddComponent<T>() where T : AbsComponent, new()
        {
            for (var i = 0; i < _componentList.Count; i++)
            {
                if (_componentList[i] is T)
                {
                    LogHelper.PrintError($"[ObjectEx]AddComponent,{typeof(T).ToString()} repeat.");
                    return null;
                }
            }
            var component = PoolMgr.singleton.GetCsharpObject<T>();
            component.Initialize(this);
            _componentList.Add(component);
            return component;
        }

        /// <summary>
        /// �Ƴ����;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ReleaseComponent<T>() where T : AbsComponent, new()
        {
            for (var i = 0; i < _componentList.Count; i++)
            {
                if (_componentList[i] is T target)
                {
                    target.UnInitialize();
                    PoolMgr.singleton.ReleaseCsharpObject(target);
                    _componentList.Remove(target);
                    return true;
                }
            }
            LogHelper.PrintError($"[ComponentMgr]ReleaseComponent {typeof(T).ToString()} error,can not find the component.");
            return false;
        }

        /// <summary>
        /// �Ƴ����;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comp"></param>
        /// <returns></returns>
        public bool ReleaseComponent<T>(AbsComponent comp) where T : AbsComponent, new()
        {
            if (comp as T == null)
            {
                LogHelper.PrintError($"[ComponentMgr]ReleaseComponent error:comp as {typeof(T).ToString()} is null.");
                return false;
            }
            for (var i = 0; i < _componentList.Count; i++)
            {
                var target = _componentList[i];
                if (target == comp)
                {
                    comp.UnInitialize();
                    PoolMgr.singleton.ReleaseCsharpObject(comp as T);
                    _componentList.Remove(target);
                    return true;
                }
            }
            LogHelper.PrintError($"[ComponentMgr]ReleaseComponent {typeof(T).ToString()} error,can not find the component.");
            return false;
        }

        /// <summary>
        /// �������;
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public bool DestroyComponent(AbsComponent comp)
        {
            for (var i = 0; i < _componentList.Count; i++)
            {
                var target = _componentList[i];
                if (target == comp)
                {
                    comp.UnInitialize();
                    _componentList.Remove(comp);
                    return true;
                }
            }
            LogHelper.PrintError($"[ComponentMgr]DestroyComponent {comp.GetType().ToString()} error,can not find the component!");
            return false;
        }

        /// <summary>
        /// �����Get;
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnGet(params object[] args) { }

        /// <summary>
        /// �����Release;
        /// </summary>
        public virtual void OnRelease() { }

        /// <summary>
        /// ����¼�;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handler"></param>
        protected void AddEvent(EventType type, EventHandler handler)
        {
            EventMgr.singleton.AddEvent(this, type, handler);
        }

        /// <summary>
        /// �Ƴ��¼�;
        /// </summary>
        /// <param name="type"></param>
        protected void RemoveEvent(EventType type)
        {
            EventMgr.singleton.RemoveEvent(this, type);
        }

        /// <summary>
        /// �����¼�;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventArgs"></param>
        protected void FireEvent(EventType type, IEventArgs eventArgs)
        {
            EventMgr.singleton.FireEvent(this, type, eventArgs);
        }
    }
}