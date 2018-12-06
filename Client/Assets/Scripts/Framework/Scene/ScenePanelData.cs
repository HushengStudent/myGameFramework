/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/16 00:57:45
** desc:  场景面数据;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ScenePanelData : ISceneData
    {
        public ScenePointData X { get; private set; }
        public ScenePointData Y { get; private set; }
        public ScenePointData Z { get; private set; }
        public ScenePointData W { get; private set; }

        public void Serializ(object[] args)
        {
            X = args[0] as ScenePointData;
            Y = args[1] as ScenePointData;
            Z = args[2] as ScenePointData;
            W = args[3] as ScenePointData;
        }
    }
}
