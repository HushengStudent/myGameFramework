/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/11 22:22:29
** desc:  合并模型组件;
*********************************************************************************/

using Framework.ResourceModule;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

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
        private Dictionary<ModelPart, AbsAssetProxy> _partProxyDict;
        private Dictionary<ModelPart, AbsAssetProxy> _tempPartProxyDict;
        private readonly string _skeletonPath = "Assets/Bundles/Prefab/Models/Avatar/ch_pc_hou.prefab";
        private GameObject _skeleton;
        private AbsAssetProxy _skeletonProxy;

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
            _partProxyDict = new Dictionary<ModelPart, AbsAssetProxy>();
            _tempPartProxyDict = new Dictionary<ModelPart, AbsAssetProxy>();

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
            _tempPartProxyDict.TryGetValue(part, out var runningProxy);
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
            var skeleton = _skeleton.GetComponentsInChildren<Transform>();
            var combineInstances = new List<CombineInstance>();
            var materials = new List<Material>();
            var bones = new List<Transform>();

            foreach (var temp in _partProxyDict)
            {
                if (temp.Key == ModelPart.ModelWeapon)
                {
                    continue;
                }
                var proxy = temp.Value;
                var go = proxy.GetInstantiateObject<GameObject>();
                var smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
                materials.AddRange(smr.materials);
                for (var sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
                {
                    var combineInstance = new CombineInstance
                    {
                        mesh = smr.sharedMesh,
                        subMeshIndex = sub
                    };
                    combineInstances.Add(combineInstance);
                }
                foreach (var bone in smr.bones)
                {
                    var bonename = bone.name;
                    foreach (var transform in skeleton)
                    {
                        if (transform.name != bonename)
                        {
                            continue;
                        }
                        bones.Add(transform);
                        break;
                    }
                }
                proxy.ReleaseInstantiateObject(go);
            }

            var UVList = new List<Vector2[]>();
            var newMat = new Material(Shader.Find("Mobile/Diffuse"));

            var Textures = new List<Texture2D>();
            for (var i = 0; i < materials.Count; i++)
            {
                Textures.Add(materials[i].GetTexture("_MainTex") as Texture2D);
            }

            var newTex = new Texture2D(512, 512, TextureFormat.ETC2_RGBA8, true);
            var texUV = newTex.PackTextures(Textures.ToArray(), 0);
            newMat.mainTexture = newTex;

            Vector2[] oldUV, newUV;
            for (var i = 0; i < combineInstances.Count; i++)
            {
                oldUV = combineInstances[i].mesh.uv;
                newUV = new Vector2[oldUV.Length];
                for (var j = 0; j < oldUV.Length; j++)
                {
                    newUV[j] = new Vector2((oldUV[j].x * texUV[i].width) + texUV[i].x, (oldUV[j].y * texUV[i].height) + texUV[i].y);
                }
                UVList.Add(combineInstances[i].mesh.uv);
                combineInstances[i].mesh.uv = newUV;
            }

            var skinnedMeshRenderer = _skeleton.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                UnityObject.DestroyImmediate(skinnedMeshRenderer);
            }

            skinnedMeshRenderer = _skeleton.AddComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.sharedMesh = new Mesh();
            skinnedMeshRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, false);
            skinnedMeshRenderer.bones = bones.ToArray();
            //skinnedMeshRenderer.materials = materials.ToArray();
            skinnedMeshRenderer.material = newMat;
            for (var i = 0; i < combineInstances.Count; i++)
            {
                combineInstances[i].mesh.uv = UVList[i];
            }
            if (_skeleton.transform.childCount > 0)
            {
                _skeleton.transform.GetChild(0).hideFlags = HideFlags.HideInHierarchy;
            }
        }
    }
}