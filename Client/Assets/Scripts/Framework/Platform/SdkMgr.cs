/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/04 23:56:58
** desc:  渠道管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{
    public class SdkMgr : Singleton<SdkMgr>
    {
        private Dictionary<string, Action<string>> _sdkCallbackDict = new Dictionary<string, Action<string>>();

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _sdkCallbackDict.Clear();
        }

        public void RegisterCallBack(Action<string> cb)
        {
            if (cb != null)
            {
                var cbName = cb.Method.Name;
                if (_sdkCallbackDict.ContainsKey(cbName))
                {
                    _sdkCallbackDict[cbName] = cb;
                }
                else
                {
                    _sdkCallbackDict.Add(cbName, cb);
                }
                LogHelper.Print($"[SdkMgr]Register Callback Success:{cbName}");
            }
        }

        public void ClearAllCallBack()
        {
            _sdkCallbackDict.Clear();
        }

        public void ExecuteCallBack(string str)
        {
            if (MiniJSON.jsonDecode(str) is Hashtable table)
            {
                var cbName = table["Method"] as string;
                var content = table["Content"] as string;
                if (_sdkCallbackDict.ContainsKey(cbName))
                {
                    _sdkCallbackDict[cbName]?.Invoke(content);
                }
            }
        }
    }
}
