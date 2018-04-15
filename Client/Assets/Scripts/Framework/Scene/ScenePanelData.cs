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
        private ScenePointData _x;
        private ScenePointData _y;
        private ScenePointData _z;
        private ScenePointData _w;

        public ScenePointData X { get { return _x; } }
        public ScenePointData Y { get { return _y; } }
        public ScenePointData Z { get { return _z; } }
        public ScenePointData W { get { return _w; } }

        public void Serializ(object[] args)
        {
            _x = args[0] as ScenePointData;
            _y = args[1] as ScenePointData;
            _z = args[2] as ScenePointData;
            _w = args[3] as ScenePointData;
        }
    }
}
