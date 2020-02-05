/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/11 22:22:29
** desc:  合并模型组件;
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum ModelPart : int
    {
        ModelHead = 1,
        ModelBody,
        ModelHand,
        ModelFeet,
        ModelWeapon
    }

    public class CombineModelComponent : ModelComponent
    {
        private class PartLoader
        {
            public GameObject Object;
            public AssetBundleAssetProxy Proxy;
        }

        private Dictionary<ModelPart, string> _modelDataDict;
        private Dictionary<ModelPart, PartLoader> _modelLoaderDict;
        private readonly string _modelRootPath = "Prefab/Models/Avatar/ch_pc_hou.prefab";
        private GameObject _modelRootObject;
        private AssetBundleAssetProxy _modelRootProxy;

        protected override void InitializeEx()
        {
            base.InitializeEx();
            _modelDataDict = new Dictionary<ModelPart, string>
            {
                [ModelPart.ModelHead] = ModelMgr.singleton.HeadArray[0],
                [ModelPart.ModelBody] = ModelMgr.singleton.BodyArray[0],
                [ModelPart.ModelHand] = ModelMgr.singleton.HandArray[0],
                [ModelPart.ModelFeet] = ModelMgr.singleton.FeetArray[0],
                [ModelPart.ModelWeapon] = ModelMgr.singleton.WeaponArray[0]
            };

            _modelRootProxy = ResourceMgr.singleton.LoadAssetAsync(_modelRootPath);
            _modelRootProxy.AddLoadFinishCallBack(() =>
            {
                _modelRootObject = _modelRootProxy.GetInstantiateObject<GameObject>();
                OnLoaded();
            });
            _modelLoaderDict = new Dictionary<ModelPart, PartLoader>();

            foreach (var temp in _modelDataDict)
            {
                var part = temp.Key;
                var name = temp.Value;
                var proxy = ResourceMgr.singleton.LoadAssetAsync(name);
                var loader = PoolMgr.singleton.GetCsharpObject<PartLoader>();
                loader.Object = null;
                loader.Proxy = proxy;
                _modelLoaderDict[part] = loader;
                proxy.AddLoadFinishCallBack(() =>
                {
                    var gameObject = proxy.GetInstantiateObject<GameObject>();
                    _modelLoaderDict[part].Object = gameObject;
                    OnLoaded();
                });
            }
        }

        private void OnLoaded()
        {
            if (IsInit)
            {
                return;
            }
            if (!_modelRootProxy.IsFinish)
            {
                return;
            }
            foreach (var temp in _modelLoaderDict)
            {
                if (!temp.Value.Proxy.IsFinish)
                {
                    return;
                }
            }

            CombineModel();
            OnLoadFinish();
        }

        protected override void UnInitializeEx()
        {
            base.UnInitializeEx();
            _modelDataDict.Clear();
            if (_modelRootObject)
            {
                _modelRootProxy.ReleaseInstantiateObject(_modelRootObject);
            }
            _modelRootObject = null;
            _modelRootProxy.UnloadProxy();

            foreach (var temp in _modelLoaderDict)
            {
                var loader = temp.Value;
                var go = loader.Object;
                var proxy = loader.Proxy;
                if (go)
                {
                    proxy.ReleaseInstantiateObject(go);
                }
                loader.Object = null;
                proxy.UnloadProxy();
                PoolMgr.singleton.ReleaseCsharpObject(loader);
            }
            _modelLoaderDict.Clear();
        }

        public void SetHead(string head)
        {
            var curHead = _modelDataDict[ModelPart.ModelHead];
            if (curHead != head)
            {
                _modelDataDict[ModelPart.ModelHead] = head;
                LoadModelPart(ModelPart.ModelHead, head, AddModelPart);
            }
        }

        public void SetBody(string body)
        {
            var curBody = _modelDataDict[ModelPart.ModelBody];
            if (curBody != body)
            {
                _modelDataDict[ModelPart.ModelBody] = body;
                LoadModelPart(ModelPart.ModelBody, body, AddModelPart);
            }
        }

        public void SetHand(string hand)
        {
            var curHand = _modelDataDict[ModelPart.ModelHand];
            if (curHand != hand)
            {
                _modelDataDict[ModelPart.ModelHand] = hand;
                LoadModelPart(ModelPart.ModelHand, hand, AddModelPart);
            }
        }

        public void SetFeet(string feet)
        {
            var curFeet = _modelDataDict[ModelPart.ModelFeet];
            if (curFeet != feet)
            {
                _modelDataDict[ModelPart.ModelFeet] = feet;
                LoadModelPart(ModelPart.ModelFeet, feet, AddModelPart);
            }
        }

        public void SetWeapon(string weapon)
        {
            var curWeapon = _modelDataDict[ModelPart.ModelWeapon];
            if (curWeapon != weapon)
            {
                _modelDataDict[ModelPart.ModelWeapon] = weapon;
                LoadModelPart(ModelPart.ModelWeapon, weapon, AddModelPart);
            }
        }

        private void LoadModelPart(ModelPart part, string resName, Action<ModelPart, GameObject> callback)
        {

        }

        private void AddModelPart(ModelPart part, GameObject go)
        {
            CombineModel(part);
        }

        private void CombineModel()
        {

        }

        private void CombineModel(ModelPart part)
        {
            if (!IsInit)
                return;
            if (part == ModelPart.ModelWeapon)
            {

            }
            else
            {

            }
        }
    }
}