/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/16 00:56:52
** desc:  场景点数据;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ScenePointData : ISceneData
    {
        private float _x;
        private float _y;
        private float _z;
        
        public float X { get { return _x; } }
        public float Y { get { return _y; } }
        public float Z { get { return _z; } }

        public void Serializ(object[] args)
        {
            _x = (float)args[0];
            _y = (float)args[1];
            _z = (float)args[2];
        }
    }
}