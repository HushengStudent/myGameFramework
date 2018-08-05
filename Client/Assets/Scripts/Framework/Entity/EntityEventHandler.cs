/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/06 00:33:27
** desc:  ÊµÌåÎ¯ÍÐ;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public delegate void EntityInitEventHandler(BaseEntity comp);
    public delegate void EntityLoadFinishEventHandler(BaseEntity comp, GameObject go);
}
