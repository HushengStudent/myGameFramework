/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/20 01:44:34
** desc:  Object��չ;
*********************************************************************************/

using Framework.EventModule;
using Framework.ObjectPoolModule;
using System.Collections.Generic;

namespace Framework.ECSModule
{
    public class ObjectEx : IPool
    {
        protected ObjectEx()
        {
            ID = IdGenerateHelper.GenerateId();
            Enable = false;
        }

        private List<AbsComponent> _componentList = new List<AbsComponent>();

        /// Call Update ComponentList;
        private List<AbsComponent> _updateComponentList = new List<AbsComponent>();
        /// Release at this frame;
        private List<string> _releaseComponentUIDList = new List<string>();

        public long ID { get; private set; }
        public bool Enable { get; set; }

        protected virtual void FixedUpdateEx(float interval) { }
        protected virtual void UpdateEx(float interval) { }
        protected virtual void LateUpdateEx(float interval) { }

        public void FixedUpdate(float interval)
        {
            FixedUpdateEx(interval);

            _updateComponentList.Clear();
            _updateComponentList.AddRange(_componentList);
            _releaseComponentUIDList.Clear();

            for (var i = 0; i < _updateComponentList.Count; i++)
            {
                var target = _updateComponentList[i];
                if (target.Enable && !_releaseComponentUIDList.Contains(target.UID))
                {
                    target.FixedUpdateEx(interval);
                }
            }
        }

        public void Update(float interval)
        {
            UpdateEx(interval);
            for (var i = 0; i < _updateComponentList.Count; i++)
            {
                var target = _updateComponentList[i];
                if (target.Enable && !_releaseComponentUIDList.Contains(target.UID))
                {
                    target.UpdateEx(interval);
                }
            }
        }

        public void LateUpdate(float interval)
        {
            LateUpdateEx(interval);
            for (var i = 0; i < _updateComponentList.Count; i++)
            {
                var target = _updateComponentList[i];
                if (target.Enable && !_releaseComponentUIDList.Contains(target.UID))
                {
                    target.LateUpdateEx(interval);
                }
            }
        }

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
                    _releaseComponentUIDList.Add(target.UID);
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
        /// <param name="component"></param>
        /// <returns></returns>
        public bool ReleaseComponent<T>(AbsComponent component) where T : AbsComponent, new()
        {
            if (component as T == null)
            {
                LogHelper.PrintError($"[ComponentMgr]ReleaseComponent error:comp as {typeof(T).ToString()} is null.");
                return false;
            }
            for (var i = 0; i < _componentList.Count; i++)
            {
                var target = _componentList[i];
                if (target == component)
                {
                    component.UnInitialize();
                    PoolMgr.singleton.ReleaseCsharpObject(component as T);
                    _componentList.Remove(target);
                    _releaseComponentUIDList.Add(target.UID);
                    return true;
                }
            }
            LogHelper.PrintError($"[ComponentMgr]ReleaseComponent {typeof(T).ToString()} error,can not find the component.");
            return false;
        }

        /// <summary>
        /// �������;
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool DestroyComponent(AbsComponent component)
        {
            for (var i = 0; i < _componentList.Count; i++)
            {
                var target = _componentList[i];
                if (target == component)
                {
                    component.UnInitialize();
                    _componentList.Remove(component);
                    _releaseComponentUIDList.Add(target.UID);
                    return true;
                }
            }
            LogHelper.PrintError($"[ComponentMgr]DestroyComponent {component.GetType().ToString()} error,can not find the component!");
            return false;
        }

        void IPool.OnGet(params object[] args)
        {
            OnGetEx(args);
        }

        void IPool.OnRelease()
        {
            OnReleaseEx();
        }

        /// <summary>
        /// �����Get;
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnGetEx(params object[] args) { }

        /// <summary>
        /// �����Release;
        /// </summary>
        protected virtual void OnReleaseEx() { }

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