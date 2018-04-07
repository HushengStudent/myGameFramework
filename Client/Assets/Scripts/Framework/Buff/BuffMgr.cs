/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:27:56
** desc:  Buff管理
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BuffMgr : Singleton<BuffMgr>
    {
        public void AddBuff(ulong uid, int buffId)
        {
            RoleEntity entity = EntityMgr.Instance.GetEntity<RoleEntity>(uid);
            if (entity == null)
            {
                LogUtil.LogUtility.PrintError(string.Format("[BuffMgr]entity is null,uid is {0}.", uid));
                return;
            }
            BuffComponent comp = entity.BuffComp;
            if (comp == null)
            {
                LogUtil.LogUtility.PrintError(string.Format("[BuffMgr]buffComponent is null,uid is {0}.", uid));
                return;
            }
            Buff buff = null;// 
            comp.AddBuff(buff);
        }

        public void UpdateBuff(ulong uid, int buffId, ulong leftTime)
        {

        }

        public void RemoveBuff(ulong uid, int buffId)
        {

        }
    }
}
