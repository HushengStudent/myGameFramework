/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/15 17:04:52
** desc:  特效管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class FxMgr : Singleton<FxMgr>
    {
        private Dictionary<string, GameObject> _fxDict = new Dictionary<string, GameObject>();

        protected override void OnInitialize()
        {
            Clear();
        }

        public void PlayFx(string fxName, bool usePool = false)
        {

        }

        private void Clear()
        {
            foreach (var temp in _fxDict)
            {
                var target = temp.Value;
                if (target)
                {
                    Object.DestroyImmediate(target);
                }
            }
            _fxDict.Clear();
        }
    }
}