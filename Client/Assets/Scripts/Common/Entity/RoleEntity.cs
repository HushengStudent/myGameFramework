/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:29:15
** desc:  角色实体;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class RoleEntity : AbsEntity
{
    private BuffComponent _buffComp = null;

    public BuffComponent BuffComp { get { return _buffComp; } }

    protected override void OnInitEx()
    {
        base.OnInitEx();
        _buffComp = ComponentMgr.Instance.CreateComponent<BuffComponent>(this);
    }

    protected override void OnResetEx()
    {
        base.OnResetEx();
        ComponentMgr.Instance.ReleaseComponent<BuffComponent>(_buffComp);
        _buffComp = null;
    }
}

