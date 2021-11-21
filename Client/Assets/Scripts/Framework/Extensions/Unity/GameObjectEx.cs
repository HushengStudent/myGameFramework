/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/31 23:57:59
** desc:  GameObject扩展;
*********************************************************************************/

using Framework.ECSModule;
using System;
using UnityEngine;

namespace Framework
{
    public class GameObjectEx
    {
        private Action<GameObjectEx> _loadFinishHandler;
        private Action<GameObjectEx> _destroyHandler;

        private bool _isCombineModel;
        public ModelComponent ModelComponent { get; private set; }

        private Vector3 _position = Vector3.zero;
        private Vector3 _scale = Vector3.one;
        private Vector3 _rotation = Vector3.zero;

        public bool IsLoadFinish { get; private set; }
        public GameObject gameObject { get; private set; }
        public Transform Trans { get; private set; }
        public AbsEntity Entity { get; private set; }
        public string ResPath { get; private set; }

        public void Initialize(AbsEntity entity, bool isAsync = true)
        {
            Entity = entity;
            IsLoadFinish = false;
            _isCombineModel = true;

            if (_isCombineModel)
            {
                ModelComponent = Entity.AddComponent<CombineModelComponent>();
            }
            else
            {
                ModelComponent = Entity.AddComponent<CommonModelComponent>();
            }
            ModelComponent.OnLoadFinishHandler = () =>
            {
                gameObject = ModelComponent.GameObject;
                gameObject.name = Entity.UID.ToString();
                gameObject.transform.localPosition = _position;
                gameObject.transform.localScale = _scale;
                gameObject.transform.localRotation = Quaternion.Euler(_rotation);
                IsLoadFinish = true;
                Trans = gameObject.transform;
                _loadFinishHandler?.Invoke(this);
            };
        }

        public void UnInitialize()
        {
            _destroyHandler?.Invoke(this);
            Trans = null;
            Entity = null;
            ResPath = string.Empty;
            _loadFinishHandler = null;
            _destroyHandler = null;
        }

        public void AddLoadFinishHandler(Action<GameObjectEx> handler)
        {
            _loadFinishHandler += handler;
        }

        public void RemoveLoadFinishHandler(Action<GameObjectEx> handler)
        {
            _loadFinishHandler -= handler;
        }

        public void AddDestroyHandler(Action<GameObjectEx> handler)
        {
            _destroyHandler += handler;
        }

        public void SetLocalPosition(float x, float y, float z)
        {
            _position = new Vector3(x, y, z);
            if (gameObject)
            {
                gameObject.transform.localPosition = _position;
            }
        }

        public void SetLocalScale(float x, float y, float z)
        {
            _scale = new Vector3(x, y, z);
            if (gameObject)
            {
                gameObject.transform.localScale = _scale;
            }
        }

        public void SetLocalRotation(float x, float y, float z)
        {
            _rotation = new Vector3(x, y, z);
            if (gameObject)
            {
                gameObject.transform.localRotation = Quaternion.Euler(_rotation);
            }
        }
    }
}