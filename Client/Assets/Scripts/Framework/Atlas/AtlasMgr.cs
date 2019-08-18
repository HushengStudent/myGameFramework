/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/08/18 15:35:10
** desc:  ÕººØπ‹¿Ì;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class AtlasMgr : MonoSingleton<AtlasMgr>
    {
        private Dictionary<string, Atlas> _atlasDict;
        private Dictionary<Atlas, Object> _atlasRefDict;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _atlasDict = new Dictionary<string, Atlas>();
            _atlasRefDict = new Dictionary<Atlas, Object>();
        }

        public void SetSprite(Image image,string atlasPath,string spriteName)
        {
            if(image == null || string.IsNullOrEmpty(atlasPath) 
                || string.IsNullOrEmpty(spriteName))
            {
                return;
            }
            Atlas atlas;
            if (!_atlasDict.TryGetValue(atlasPath,out atlas))
            {
                atlas = PoolMgr.Instance.GetCsharpObject<Atlas>();
                atlas.OnInitialize(atlasPath);
            }
            if(atlas == null)
            {
                return;
            }

        }

        public void ReleaseSprite(string atlasPath, string spriteName)
        {

        }

        protected override void OnUninitialize()
        {
            base.OnUninitialize();
        }
    }
}