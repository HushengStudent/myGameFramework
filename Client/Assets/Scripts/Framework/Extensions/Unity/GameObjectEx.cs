/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/31 23:57:59
** desc:  GameObject扩展;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class GameObjectEx
    {
        private int _parentInstanceId = -1;

        public GameObject Go { get; set; }
        public int ParentInstanceId { get { return _parentInstanceId; } }
        private Transform Trans { get; set; }

        public void Init(AbsEntity entity, string path, Action<GameObjectEx> action, bool isAsync = true)
        {
            //加载;
            GameObject go = null;
            Go = go;
            Trans = go.transform;
            if (action != null)
            {
                action(this);
            }
        }

        public void Uninit()
        {
            //销毁;
            _parentInstanceId = -1;
            Go = null;
            Trans = null;
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