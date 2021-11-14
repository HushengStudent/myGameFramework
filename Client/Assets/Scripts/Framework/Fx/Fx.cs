/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2021/11/14 20:45:53
** desc:  ÌØÐ§;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public partial class Fx : IPool
    {
        public int ID { get; private set; }
        public FxData Data { get; private set; }

        public void OnInitialize()
        {

        }

        public void OnUninitialize()
        {

        }

        public void OnGet(params object[] args)
        {
            var fxData = args[0] as FxData;

            ID = FxMgr.singleton.FxID;
        }

        public void OnRelease()
        {
            ID = 0;
            PoolMgr.singleton.ReleaseCsharpObject(Data);
        }
    }
}