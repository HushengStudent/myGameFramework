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
    public override void OnInitEntity(GameObject go)
    {
        base.OnInitEntity(go);
    }

    protected override void OnAttachEntityGo(GameObject go)
    {
        base.OnAttachEntityGo(go);
    }

    protected override void DeAttachEntityGo()
    {
        throw new System.NotImplementedException();
    }

    protected override void EventSubscribe()
    {
        throw new System.NotImplementedException();
    }

    protected override void EventUnsubscribe()
    {
        throw new System.NotImplementedException();
    }
}

