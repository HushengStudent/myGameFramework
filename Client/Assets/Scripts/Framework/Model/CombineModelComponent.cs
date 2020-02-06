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
        private Dictionary<ModelPart, string> _partDataDict;
        private Dictionary<ModelPart, AssetBundleAssetProxy> _partProxyDict;
        private Dictionary<ModelPart, AssetBundleAssetProxy> _tempPartProxyDict;
        private readonly string _skeletonPath = "Prefab/Models/Avatar/ch_pc_hou.prefab";
        private GameObject _skeleton;
        private AssetBundleAssetProxy _skeletonProxy;

        protected override void InitializeEx()
        {
            base.InitializeEx();
            _partDataDict = new Dictionary<ModelPart, string>
            {
                [ModelPart.ModelHead] = ModelConfig.HeadArray[0],
                [ModelPart.ModelBody] = ModelConfig.BodyArray[0],
                [ModelPart.ModelHand] = ModelConfig.HandArray[0],
                [ModelPart.ModelFeet] = ModelConfig.FeetArray[0],
                [ModelPart.ModelWeapon] = ModelConfig.WeaponArray[0]
            };

            _skeletonProxy = ResourceMgr.singleton.LoadAssetAsync(_skeletonPath);
            _skeletonProxy.AddLoadFinishCallBack(() =>
            {
                _skeleton = _skeletonProxy.GetInstantiateObject<GameObject>();
                OnLoaded();
            });
            _partProxyDict = new Dictionary<ModelPart, AssetBundleAssetProxy>();
            _tempPartProxyDict = new Dictionary<ModelPart, AssetBundleAssetProxy>();

            foreach (var temp in _partDataDict)
            {
                var part = temp.Key;
                var name = temp.Value;
                var proxy = ResourceMgr.singleton.LoadAssetAsync(name);
                _partProxyDict[part] = proxy;
                proxy.AddLoadFinishCallBack(OnLoaded);
            }
        }

        private void OnLoaded()
        {
            if (IsInit)
            {
                return;
            }
            if (!_skeletonProxy.IsFinish)
            {
                return;
            }
            foreach (var temp in _partProxyDict)
            {
                if (!temp.Value.IsFinish)
                {
                    return;
                }
            }

            CombineModel();
            OnLoadFinish();
        }

        protected override void OnLoadFinishEx()
        {
            base.OnLoadFinishEx();
            GameObject = _skeleton;
        }

        protected override void UnInitializeEx()
        {
            base.UnInitializeEx();
            _partDataDict.Clear();
            if (_skeleton)
            {
                _skeletonProxy.ReleaseInstantiateObject(_skeleton);
            }
            _skeleton = null;
            _skeletonProxy.UnloadProxy();

            foreach (var temp in _partProxyDict)
            {
                var proxy = temp.Value;
                proxy.UnloadProxy();
            }
            _partProxyDict.Clear();

            foreach (var temp in _tempPartProxyDict)
            {
                var proxy = temp.Value;
                proxy.UnloadProxy();
            }
            _tempPartProxyDict.Clear();
        }

        public void SetHead(string head)
        {
            var curHead = _partDataDict[ModelPart.ModelHead];
            if (curHead != head)
            {
                _partDataDict[ModelPart.ModelHead] = head;
                LoadModelPart(ModelPart.ModelHead, head);
            }
        }

        public void SetBody(string body)
        {
            var curBody = _partDataDict[ModelPart.ModelBody];
            if (curBody != body)
            {
                _partDataDict[ModelPart.ModelBody] = body;
                LoadModelPart(ModelPart.ModelBody, body);
            }
        }

        public void SetHand(string hand)
        {
            var curHand = _partDataDict[ModelPart.ModelHand];
            if (curHand != hand)
            {
                _partDataDict[ModelPart.ModelHand] = hand;
                LoadModelPart(ModelPart.ModelHand, hand);
            }
        }

        public void SetFeet(string feet)
        {
            var curFeet = _partDataDict[ModelPart.ModelFeet];
            if (curFeet != feet)
            {
                _partDataDict[ModelPart.ModelFeet] = feet;
                LoadModelPart(ModelPart.ModelFeet, feet);
            }
        }

        public void SetWeapon(string weapon)
        {
            var curWeapon = _partDataDict[ModelPart.ModelWeapon];
            if (curWeapon != weapon)
            {
                _partDataDict[ModelPart.ModelWeapon] = weapon;
                LoadModelPart(ModelPart.ModelWeapon, weapon);
            }
        }

        private void LoadModelPart(ModelPart part, string resName)
        {
            AssetBundleAssetProxy runningProxy;
            _tempPartProxyDict.TryGetValue(part, out runningProxy);
            if (runningProxy != null)
            {
                runningProxy.UnloadProxy();
                _tempPartProxyDict[part] = null;
            }
            var proxy = ResourceMgr.singleton.LoadAssetAsync(resName);
            _tempPartProxyDict[part] = proxy;
            proxy.AddLoadFinishCallBack(() =>
            {
                var oldProxy = _partProxyDict[part];
                oldProxy.UnloadProxy();
                _partProxyDict[part] = proxy;
                _tempPartProxyDict[part] = null;
                CombineModel(part);
            });
        }

        private void CombineModel(ModelPart part)
        {
            if (!IsInit)
            {
                return;
            }
            if (part == ModelPart.ModelWeapon)
            {

            }
            else
            {
                CombineModel();
            }
        }

        private void CombineModel()
        {
            Transform[] skeleton = _skeleton.GetComponentsInChildren<Transform>();
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            List<Material> materials = new List<Material>();
            List<Transform> bones = new List<Transform>();

            foreach (var temp in _partProxyDict)
            {
                if (temp.Key == ModelPart.ModelWeapon)
                {
                    continue;
                }
                var proxy = temp.Value;
                var go = proxy.GetInstantiateObject<GameObject>();
                SkinnedMeshRenderer smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
                materials.AddRange(smr.materials);
                for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
                {
                    CombineInstance combineInstance = new CombineInstance
                    {
                        mesh = smr.sharedMesh,
                        subMeshIndex = sub
                    };
                    combineInstances.Add(combineInstance);
                }
                foreach (Transform bone in smr.bones)
                {
                    string bonename = bone.name;
                    foreach (Transform transform in skeleton)
                    {
                        if (transform.name != bonename)
                            continue;
                        bones.Add(transform);
                        break;
                    }
                }
                proxy.ReleaseInstantiateObject(go);
            }

            SkinnedMeshRenderer skinnedMeshRenderer = _skeleton.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                UnityEngine.Object.DestroyImmediate(skinnedMeshRenderer);
            }

            skinnedMeshRenderer = _skeleton.AddComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.sharedMesh = new Mesh();
            skinnedMeshRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);
            skinnedMeshRenderer.bones = bones.ToArray();
            skinnedMeshRenderer.materials = materials.ToArray();
        }
    }
}