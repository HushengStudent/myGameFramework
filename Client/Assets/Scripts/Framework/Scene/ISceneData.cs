/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/16 00:50:48
** desc:  场景数据接口;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.SceneModule
{
    public interface ISceneData
    {
        void Serializ(object[] args);
    }
}
