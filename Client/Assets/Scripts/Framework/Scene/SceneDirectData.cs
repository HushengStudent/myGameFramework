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
        private ScenePointData _from;
        private ScenePointData _to;

        public ScenePointData From { get { return _from; } }
        public ScenePointData To { get { return _to; } }

        public void Serializ(object[] args)
        {
            _from = args[0] as ScenePointData;
            _to = args[1] as ScenePointData;
        }
    }
}
