/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/21 01:01:09
** desc:  #####
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class AssetBundleLoadNode
    {
        public enum AssetBundleNodeState : byte
        {
            Non = 1,
            Waitting,
            Loading,
            Error,
            Finish,
        }

        //一秒30帧,一帧最久0.33秒;
        private readonly float MAX_LOAD_TIME = 0.33f;

        private AssetBundleNodeState _nodeState = AssetBundleNodeState.Non;

        private static float _startTime;
        private List<AssetBundleLoadNode> _dependNode = null;

        public AssetBundleNodeState NodeState { get { return _nodeState; } }
        public string AssetBundlePath { get; private set; }

        public void Init(string path, List<AssetBundleLoadNode> nodeList = null)
        {
            AssetBundlePath = path;
            _nodeState = AssetBundleNodeState.Waitting;
            _dependNode = nodeList;
        }

        public void Reset()
        {
            AssetBundlePath = null;
            _nodeState = AssetBundleNodeState.Non;
            _dependNode = null;
        }

        public void Update()
        {
            _startTime = Time.unscaledTime;

        }
    }
}
