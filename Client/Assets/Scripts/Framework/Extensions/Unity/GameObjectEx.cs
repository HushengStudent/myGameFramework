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
        private GameObject _go;
        private int _parentInstanceId;
        private Transform _trans;

        public GameObject gameObject { get { return _go; } set { _go = value; } }
        public int parentInstanceId { get { return _parentInstanceId; } set { _parentInstanceId = value; } }
        public Transform Trans { get { return _trans; } }

        public void SetLocalPosition(float x,float y,float z)
        {
            if (_go)
            {
                gameObject.transform.localPosition = new Vector3(x, y, z);
            }
        }

        public void SetLocalScale(float x,float y,float z)
        {
            if (_go)
            {
                gameObject.transform.localScale = new Vector3(x, y, z);
            }
        }
    }
}