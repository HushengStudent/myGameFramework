/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 14:16:25
** desc:  ECS组件管理;
*********************************************************************************/

namespace Framework
{
    public class ComponentMgr : MonoSingleton<ComponentMgr>
    {
        #region Function

        /// <summary>
        /// 创建Component;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <returns></returns>
        public T CreateComponent<T>(ObjectEx owner) where T : AbsComponent, new()
        {
            if (null != owner)
            {
                return owner.AddComponent<T>();
            }
            else
            {
                LogHelper.PrintError($"[ComponentMgr]CreateComponent {typeof(T).ToString()} error!");
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
