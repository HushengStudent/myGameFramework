/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/04 23:56:58
** desc:  渠道管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Framework
{
    public class SdkMgr : Singleton<SdkMgr>
    {
        private Dictionary<string, Action<string>> SdkCallBack = new Dictionary<string, Action<string>>();

        public override void Init()
        {
            base.Init();
            SdkCallBack.Clear();

        }

        public void RegisterCallBack(Action<string> cb)
        {
            if (cb != null)
            {
                string cbName = cb.Method.Name;
                if (SdkCallBack.ContainsKey(cbName))
                {
                    SdkCallBack[cbName] = cb;
                }
                else
                {
                    SdkCallBack.Add(cbName, cb);
                }
                LogUtil.LogUtility.Print("[SdkMgr]Register Callback Success:" + cbName);
            }
        }

        public void ClearAllCallBack()
        {
            SdkCallBack.Clear();
        }

        public void ExecuteCallBack(string str)
        {
            Hashtable table = MiniJSON.jsonDecode(str) as Hashtable;
            if (table != null)
            {
                string cbName = table["Method"] as string;
                string content = table["Content"] as string;
                if (SdkCallBack.ContainsKey(cbName))
                {
                    Action<string> cb = SdkCallBack[cbName];
                    if (cb != null)
                    {
                        cb(content);
                    }
                }
            }
        }
    }
}
