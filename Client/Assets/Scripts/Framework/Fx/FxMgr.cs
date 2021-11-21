/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/15 17:04:52
** desc:  特效管理;
*********************************************************************************/

using Framework.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class FxMgr : MonoSingleton<FxMgr>
    {
        private int _fxID = 0;
        public int FxID
        {
            get
            {
                if (_fxID == int.MaxValue)
                {
                    _fxID = 0;
                }
                return _fxID++;
            }
        }

        private Dictionary<int, Fx> _fxDict = new Dictionary<int, Fx>();

        protected override void OnInitialize()
        {
            Clear();
        }

        public Fx.FxData GetFxData()
        {
            return PoolMgr.singleton.GetCsharpObject<Fx.FxData>();
        }

        public int PlayFx(Fx.FxData fxData)
        {
            var fx = PoolMgr.singleton.GetCsharpObject<Fx>(fxData);

            return fx.ID;
        }

        private void Clear()
        {
            foreach (var temp in _fxDict)
            {
                var fx = temp.Value;
                if (fx != null)
                {
                    fx.OnUninitialize();
                }
            }
            _fxDict.Clear();
        }
    }
}