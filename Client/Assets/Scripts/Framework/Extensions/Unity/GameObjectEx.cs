/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/31 23:57:59
** desc:  GameObject扩展;
*********************************************************************************/

using System;
using UnityEngine;

namespace Framework
{
    public class GameObjectEx
    {
        private Action<GameObjectEx> _loadFinishHandler;
        private Action<GameObjectEx> _destroyHandler;

        private bool _isCombineModel;
        private ModelComponent _modelComponent;

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
                _modelComponent = Entity.AddComponent<CombineModelComponent>();
            }
            else
            {
                _modelComponent = Entity.AddComponent<CommonModelComponent>();
            }
            _modelComponent.OnLoadFinishHandler = () =>
            {
                gameObject = _modelComponent.GameObject;
                gameObject.name = Entity.UID.ToString();
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

        }

        public void SetLocalScale(float x, float y, float z)
        {

        }

        public void SetLocalRotation(float x, float y, float z)
        {
            //gameObject.transform.localRotation = Quaternion.Euler(_rotation);
        }
    }
}