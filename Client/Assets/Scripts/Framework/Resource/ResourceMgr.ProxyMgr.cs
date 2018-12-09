/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 22:54:09
** desc:  Resource资源加载代理;
*********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using MEC;
using System.IO;

namespace Framework
{
    public partial class ResourceMgr
    {
        public ManagerInitEventHandler ResourceMgrInitHandler { get; set; }

        private List<ResourceLoadProxy> _proxyList = new List<ResourceLoadProxy>();

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            for (int i = 0; i < _proxyList.Count; i++)
            {
                var target = _proxyList[i];
                if (target.UnloadProxy())
                {
                    _proxyList.Remove(target);
                }
            }
        }

        public void AddProxy(ResourceLoadProxy proxy)
        {
            _proxyList.Add(proxy);
        }

        public IEnumerator<float> CancleAllProxy()
        {
            yield return Timing.WaitForOneFrame;
            while (true)
            {
                if (_proxyList.Count > 0)
                {
                    yield return Timing.WaitForOneFrame;
                }
                else
                {
                    if (ResourceMgrInitHandler != null)
                    {
                        ResourceMgrInitHandler();
                    }
                    break;
                }
            }
        }
    }
}
