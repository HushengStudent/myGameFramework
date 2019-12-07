/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/08/18 15:39:05
** desc:  图集;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class Atlas : IPool
    {
        private Dictionary<string, Sprite> _spriteDict = new Dictionary<string, Sprite>();
        private Dictionary<Object, string> _objectRefDict = new Dictionary<Object, string>();
        private GameObject _atlasPrefab;
        private AssetBundleAssetProxy _proxy;

        public string AtlasPath { get; private set; }
        public int RefCount { get; private set; }
        public bool Deprecated { get; private set; }

        public void OnInitialize(string atlasPath)
        {
            _objectRefDict.Clear();
            _spriteDict.Clear();
            _atlasPrefab = null;
            _proxy = null;
            AtlasPath = atlasPath;
            RefCount = 0;
            Deprecated = false;

            LoadAtlas();
        }

        private void LoadAtlas()
        {
            _proxy = ResourceMgr.singleton.LoadAssetAsync(AtlasPath);
            _proxy.AddLoadFinishCallBack(OnLoadFinish);
        }

        private void OnLoadFinish()
        {
            if (_proxy == null)
            {
                _atlasPrefab = _proxy.GetInstantiateObject<GameObject>();
                if (_atlasPrefab == null)
                {
                    Deprecated = true;
                }
                else
                {
                    _atlasPrefab.transform.SetParent(AtlasMgr.singleton.AtlasRoot.transform);
                }
            }
            if (!Deprecated)
            {
                var behaviour = _atlasPrefab.GetComponent<AtlasBehaviour>();
                if (behaviour == null)
                {
                    Deprecated = true;
                }
                for (int i = 0; i < behaviour._spriteList.Count; i++)
                {
                    var sprite = behaviour._spriteList[i];
                    if (sprite)
                    {
                        _spriteDict[sprite.name] = sprite;
                    }
                }
                foreach (var temp in _objectRefDict)
                {
                    TrySetSprite(temp.Key, temp.Value);
                }
            }
        }

        public void OnUninitialize()
        {
            if (_atlasPrefab != null)
            {
                ResourceMgr.singleton.DestroyInstantiateObject(_atlasPrefab);
            }

            _objectRefDict.Clear();
            _spriteDict.Clear();
            _atlasPrefab = null;
            _proxy.UnloadProxy();
            _proxy = null;
            AtlasPath = null;
            RefCount = 0;
            Deprecated = false;
        }

        public void SetSprite(Object target, string spriteName)
        {
            _objectRefDict[target] = spriteName;
            if (_atlasPrefab == null)
            {
                return;
            }

            TrySetSprite(target, spriteName);
        }

        private void TrySetSprite(Object target, string spriteName)
        {
            Sprite sprite;
            if (!_spriteDict.TryGetValue(spriteName, out sprite))
            {
                LogHelper.PrintError($"[Atlas]not find sprite:{AtlasPath} {spriteName}");
                return;
            }

            if (target is Image)
            {
                (target as Image).sprite = sprite;
            }
        }

        public void ReleaseSprite(Object target)
        {
            string name;
            if (_objectRefDict.TryGetValue(target, out name))
            {
                _objectRefDict.Remove(target);
                TryReleaseAtlas();
            }
        }

        /// 切场景时卸载一次;
        public bool TryReleaseAtlas()
        {
            var list = PoolMgr.singleton.GetCsharpList<Object>();
            foreach (var temp in _objectRefDict)
            {
                if (temp.Key == null)
                {
                    list.Add(temp.Key);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                _objectRefDict.Remove(list[i]);
            }
            if (_objectRefDict.Count == 0)
            {
                Deprecated = true;
            }
            return Deprecated;
        }

        public void OnGet(params object[] args)
        {
        }

        public void OnRelease()
        {
        }
    }
}