/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/06 00:48:37
** desc:  HUD
*********************************************************************************/

using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDComponent : AbsComponent
{
    public override void OnInitComponent(AbsEntity entity, GameObject go)
    {
        base.OnInitComponent(entity, go);
    }

    protected override void DeAttachComponentGo()
    {
        throw new System.NotImplementedException();
    }

    protected override void DeAttachEntity()
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
