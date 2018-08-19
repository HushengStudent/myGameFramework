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
        private GameObject _gameObject;
        private int _parentInstanceId;

        public GameObject gameObject { get { return _gameObject; } set { _gameObject = value; } }
        public int parentInstanceId { get { return _parentInstanceId; } set { _parentInstanceId = value; } }
    }
}