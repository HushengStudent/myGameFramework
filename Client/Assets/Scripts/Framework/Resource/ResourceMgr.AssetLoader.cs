/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/23 17:34:50
** desc:  资源加载器,不参与资源管理;
*********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MEC;
using Object = UnityEngine.Object;

namespace Framework
{
    public partial class ResourceMgr
    {
        public GameObject LoadGameObject(Object obj)
        {
            if (obj != null)
            {
                return GameObject.Instantiate(obj) as GameObject;
            }
            else
            {
                return null;
            }
        }

        public void DestroyUnityGameObject(GameObject go)
        {
            if (go != null)
            {
                GameObject.Destroy(go);
            }
        }
    }
}
