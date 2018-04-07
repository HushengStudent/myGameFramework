/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:29:15
** desc:  角色实体
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class RoleEntity : AbsEntity
{
    private BuffComponent _buffComp = null;

    public BuffComponent BuffComp { get { return _buffComp; } }

    public override void OnInitEntity(GameObject go)
    {
        base.OnInitEntity(go);
        ComponentMgr.Instance.CreateComponent<BuffComponent>(this, EntityGO, null);
    }

    public override void OnResetEntity()
    {
        base.OnResetEntity();
        ComponentMgr.Instance.ReleaseComponent<BuffComponent>(_buffComp);
        _buffComp = null;
    }
}

