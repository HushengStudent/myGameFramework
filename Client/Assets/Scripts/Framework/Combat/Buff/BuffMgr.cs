/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:27:56
** desc:  Buff管理;
*********************************************************************************/

using Framework.ECSModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.BuffModule
{
    public class BuffMgr : Singleton<BuffMgr>
    {
        public void AddBuff(ulong uid, int buffId)
        {
            var entity = EntityMgr.singleton.GetEntity<RoleEntity>(uid);
            if (entity == null)
            {
                LogHelper.PrintError($"[BuffMgr]entity is null,uid is {uid}.");
                return;
            }
            var comp = entity.BuffComp;
            if (comp == null)
            {
                LogHelper.PrintError($"[BuffMgr]buffComponent is null,uid is {uid}.");
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
