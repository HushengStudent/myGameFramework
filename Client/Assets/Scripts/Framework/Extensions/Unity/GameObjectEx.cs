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
    public delegate void GameObjectExLoadFinishHandler(GameObjectEx go);
    public delegate void GameObjectExDestroyHandler(GameObjectEx go);

    public class GameObjectEx
    {
        private int _parentInstanceId = -1;
        private AbsEntity _entity = null;
        private string _resPath = string.Empty;
        private GameObjectExLoadFinishHandler _loadFinishHandler = null;
        private GameObjectExDestroyHandler _destroyHandler = null;

        public bool IsLoadFinish;
        public GameObject gameObject;
        public Transform Trans;
        public AbsEntity Entity { get { return _entity; } }
        public string ResPath { get { return _resPath; } }
        public int ParentInstanceId { get { return _parentInstanceId; } }

        public void Init(AbsEntity entity, string path, bool isAsync = true)
        {
            _entity = entity;
            _resPath = path;
            IsLoadFinish = false;
            //加载;
            GameObject go = null;
            gameObject = go;
            IsLoadFinish = true;
            Trans = go.transform;
            if (_loadFinishHandler != null)
            {
                _loadFinishHandler(this);
            }
        }

        public void Uninit()
        {
            if (_destroyHandler != null)
            {
                _destroyHandler(this);
            }
            if (IsLoadFinish)
            {
                //销毁;
            }
            gameObject = null;
            Trans = null;
            _entity = null;
            _resPath = string.Empty;
            _parentInstanceId = -1;
            _loadFinishHandler = null;
            _destroyHandler = null;
        }

        public void AddLoadFinishHandler(GameObjectExLoadFinishHandler handler)
        {
            _loadFinishHandler += handler;
        }

        public void RemoveLoadFinishHandler(GameObjectExLoadFinishHandler handler)
        {
            _loadFinishHandler -= handler;
        }

        public void AddDestroyHandler(GameObjectExDestroyHandler handler)
        {
            _destroyHandler += handler;
        }

        public void SetLocalPosition(float x, float y, float z)
        {
            if (gameObject)
            {
                gameObject.transform.localPosition = new Vector3(x, y, z);
            }
        }

        public void SetLocalScale(float x, float y, float z)
        {
            if (gameObject)
            {
                gameObject.transform.localScale = new Vector3(x, y, z);
            }
        }

        public void SetLocalRotation(float x, float y, float z, float w)
        {
            if (gameObject)
            {
                gameObject.transform.localRotation = new Quaternion(x, y, z, w);
            }
        }
    }
}