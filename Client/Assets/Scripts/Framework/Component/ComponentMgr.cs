/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 14:16:25
** desc:  ECS组件管理;
*********************************************************************************/

using System.Collections.Generic;

namespace Framework
{
    public class ComponentMgr : MonoSingleton<ComponentMgr>
    {
        #region Field

        private List<AbsComponent> _componentList = new List<AbsComponent>();

        #endregion

        #region Unity api

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _componentList.Clear();
        }

        protected override void FixedUpdateEx(float interval)
        {
            base.FixedUpdateEx(interval);
            for (int i = 0; i < _componentList.Count; i++)
            {
                if (_componentList[i].Enable)
                {
                    _componentList[i].FixedUpdateEx(interval);
                }
            }
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            for (int i = 0; i < _componentList.Count; i++)
            {
                if (_componentList[i].Enable)
                {
                    _componentList[i].UpdateEx(interval);
                }
            }
        }

        protected override void LateUpdateEx(float interval)
        {
            base.LateUpdateEx(interval);
            for (int i = 0; i < _componentList.Count; i++)
            {
                if (_componentList[i].Enable)
                {
                    _componentList[i].LateUpdateEx(interval);
                }
            }
        }

        #endregion

        #region Function

        /// <summary>
        /// 创建Component;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <returns></returns>
        public T CreateComponent<T>(ObjectEx owner) where T : AbsComponent, new()
        {
            if (null != owner && owner.AddComponent<T>())
            {
                return owner.GetComponent<T>();
            }
            else
            {
                LogHelper.PrintError("[ComponentMgr]CreateComponent " + typeof(T).ToString() + " error!");
                return null;
            }
        }

        /// <summary>
        /// 移除Component;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        public void ReleaseComponent<T>(ObjectEx owner) where T : AbsComponent, new()
        {
            if (null == owner)
            {
                return;
            }
            owner.ReleaseComponent<T>();
        }

        /// <summary>
        /// 移除Component;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comp"></param>
        public void ReleaseComponent<T>(AbsComponent comp) where T : AbsComponent, new()
        {
            if (null == comp || null == comp.Owner)
            {
                return;
            }
            if (comp.Owner.ReleaseComponent<T>(comp))
            {
                comp = null;
            }
        }

        /// <summary>
        /// 移除Component;
        /// </summary>
        /// <param name="comp"></param>
        public void DestroyComponent(AbsComponent comp)
        {
            if (null == comp.Owner || null == comp)
            {
                return;
            }
            comp.Owner.DestroyComponent(comp);
        }

        #endregion
    }
}
