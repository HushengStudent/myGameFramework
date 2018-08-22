/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/31 23:57:59
** desc:  GameObject扩展;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class GameObjectEx
    {
        private int _parentInstanceId;

        public GameObject Go { get; set; }
        public int ParentInstanceId { get { return _parentInstanceId; } }
        private Transform Trans { get; set; }

        public GameObjectEx(GameObject go, int parentInstanceId = -1)
        {
            Go = go;
            _parentInstanceId = parentInstanceId;
            Trans = go.transform;
        }

        public void SetLocalPosition(float x, float y, float z)
        {
            if (Go)
            {
                Go.transform.localPosition = new Vector3(x, y, z);
            }
        }

        public void SetLocalScale(float x, float y, float z)
        {
            if (Go)
            {
                Go.transform.localScale = new Vector3(x, y, z);
            }
        }

        public void SetLocalRotation(float x, float y, float z, float w)
        {
            if (Go)
            {
                Go.transform.localRotation = new Quaternion(x, y, z, w);
            }
        }
    }
}