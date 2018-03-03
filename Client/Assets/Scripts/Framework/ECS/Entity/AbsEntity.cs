/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:32:30
** desc:  实体抽象基类
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AbsEntity
    {
        private long _id;
        private bool _enable = true;
        private bool _isLoaded = false;
        private GameObject _entityGo = null;
        private Action<AbsEntity> _initCallBack;

        public long ID { get { return _id; } }
        public bool Enable { get { return _enable; } set { _enable = value; } }
        public bool IsLoaded { get { return _isLoaded; } set { _isLoaded = value; } }
        public GameObject EntityGO { get { return _entityGo; } set { _entityGo = value; } }
        public Action<AbsEntity> InitCallBack { get { return _initCallBack; } set { _initCallBack = value; } }

        public virtual void AwakeEx() { }
        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }
        public virtual void OnDestroyEx() { }

        public virtual void OnLoad() { }

        public virtual void OnInit()
        {
            //TODO:use pool and async;
            _id = IdGenerater.GenerateId();
            if (InitCallBack != null)
            {
                InitCallBack(this);
            }
        }

        protected virtual void ResetEntity()
        {
            _id = 0;
            _enable = true;
            _entityGo = null;
            _initCallBack = null;
        }
    }
}
