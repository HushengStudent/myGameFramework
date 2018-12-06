/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/16 00:57:26
** desc:  场景向量数据;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class SceneDirectData : ISceneData
    {
        public ScenePointData From { get; private set; }
        public ScenePointData To { get; private set; }

        public void Serializ(object[] args)
        {
            From = args[0] as ScenePointData;
            To = args[1] as ScenePointData;
        }
    }
}
