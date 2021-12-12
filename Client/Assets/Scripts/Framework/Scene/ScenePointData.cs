/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/16 00:56:52
** desc:  场景点数据;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.SceneModule
{
    public class ScenePointData : ISceneData
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public void Serializ(object[] args)
        {
            X = (float)args[0];
            Y = (float)args[1];
            Z = (float)args[2];
        }
    }
}