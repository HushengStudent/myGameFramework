/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/08/18 15:35:10
** desc:  ÕººØπ‹¿Ì;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class AtlasMgr : MonoSingleton<AtlasMgr>
    {
        private Dictionary<string, Atlas> _atlasDict;
        private readonly string _atlasRootName = "@AtlasRoot";

        public GameObject AtlasRoot
        {
            get
            {
                var go = GameObject.Find(_atlasRootName);
                if (go)
                {
                    return go;
                }
                go = new GameObject(_atlasRootName);
                DontDestroyOnLoad(go);
                return go;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _atlasDict = new Dictionary<string, Atlas>();
            EventMgr.singleton.AddGlobalEvent(EventType.ON_SCENE_UNLOAD, OnSceneUnload);
        }

        public void SetSprite(Object Object, string atlasPath, string spriteName)
        {
            if (Object == null || string.IsNullOrEmpty(atlasPath)
                || string.IsNullOrEmpty(spriteName))
            {
                return;
            }
            Atlas atlas;
            if (!_atlasDict.TryGetValue(atlasPath, out atlas) || atlas.Deprecated)
            {
                atlas = PoolMgr.Singleton.GetCsharpObject<Atlas>();
                atlas.OnInitialize(atlasPath);
            }
            if (atlas == null)
            {
                return;
            }
            atlas.SetSprite(Object, spriteName);
        }

        public void ReleaseSprite(Object Object, string atlasPath)
        {
            if (string.IsNullOrEmpty(atlasPath))
            {
                return;
            }
            Atlas atlas;
            if (_atlasDict.TryGetValue(atlasPath, out atlas) && !atlas.Deprecated)
            {
                atlas.ReleaseSprite(Object);
            }
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            var list = PoolMgr.Singleton.GetCsharpList<string>();
            foreach (var temp in _atlasDict)
            {
                if (temp.Value.Deprecated)
                {
                    list.Add(temp.Key);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                var name = list[i];
                var atlas = _atlasDict[name];
                _atlasDict.Remove(name);
                atlas.OnUninitialize();
            }
            list.Clear();
            PoolMgr.Singleton.ReleaseCsharpList(list);
        }

        private void OnSceneUnload(IEventArgs eventArgs)
        {
            var list = PoolMgr.Singleton.GetCsharpList<string>();
            foreach (var temp in _atlasDict)
            {
                if (temp.Value.Deprecated || temp.Value.TryReleaseAtlas())
                {
                    list.Add(temp.Key);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                var name = list[i];
                var atlas = _atlasDict[name];
                _atlasDict.Remove(name);
                atlas.OnUninitialize();
            }
            list.Clear();
            PoolMgr.Singleton.ReleaseCsharpList(list);
        }

        protected override void OnUninitialize()
        {
            base.OnUninitialize();
            foreach (var temp in _atlasDict)
            {
                temp.Value.OnUninitialize();
            }
            _atlasDict.Clear();
        }
    }
}