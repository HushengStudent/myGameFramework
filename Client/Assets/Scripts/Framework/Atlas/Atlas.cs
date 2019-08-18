/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/08/18 15:39:05
** desc:  Í¼¼¯;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class Atlas : IPool
    {
        private Dictionary<string, Sprite> _spriteDict = new Dictionary<string, Sprite>();
        private Dictionary<Object, string> _objectRefDict = new Dictionary<Object, string>();
        private Texture _texture;
        private AssetBundleAssetProxy _proxy;


        public string AtlasPath { get; private set; }
        public int RefCount { get; private set; }
        public bool Deprecated { get; private set; }

        public void OnInitialize(string atlasPath)
        {
            _objectRefDict.Clear();
            _spriteDict.Clear();
            _texture = null;
            _proxy = null;
            AtlasPath = atlasPath;
            RefCount = 0;
            Deprecated = false;

            LoadAtlas();
        }

        private void LoadAtlas()
        {
            _proxy = ResourceMgr.Instance.LoadAssetAsync(AtlasPath);
            _proxy.AddLoadFinishCallBack(OnLoadFinish);
        }

        private void OnLoadFinish()
        {
            if (_proxy == null)
            {
                _texture = _proxy.GetUnityAsset<Texture>();
                if (_texture == null)
                {
                    _proxy.UnloadProxy();
                    Deprecated = true;
                }
            }
            if (!Deprecated)
            {

            }
        }

        public void OnUninitialize()
        {
            _objectRefDict.Clear();
            _spriteDict.Clear();
            _texture = null;
            _proxy.UnloadProxy();
            _proxy = null;
            AtlasPath = null;
            RefCount = 0;
            Deprecated = false;
        }

        public void SetSprite(Object target, string spriteName)
        {
            if (_texture == null)
            {
                string name;
                if (!_objectRefDict.TryGetValue(target, out name))
                {
                    _objectRefDict[target] = spriteName;
                }
                return;
            }

            TrySetSprite(target, spriteName);
        }

        private void TrySetSprite(Object target, string spriteName)
        {
            Sprite sprite;
            if (!_spriteDict.TryGetValue(spriteName, out sprite))
            {
                LogHelper.PrintError("[Atlas]not find sprite:" + AtlasPath + " " + spriteName);
                return;
            }

            if(target is Image)
            {
                (target as Image).sprite = sprite;
            }
        }

        public void OnGet(params object[] args)
        {
        }

        public void OnRelease()
        {
        }
    }
}