/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/21 01:01:09
** desc:  AssetBundle���ؽڵ�;
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Framework
{
    public class AssetBundleLoadNode : IPool
    {
        public enum AssetBundleNodeState : byte
        {
            Non = 1,
            Waitting,
            Loading,
            Error,
            Finish,
        }

        private Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private AssetBundleNodeState _nodeState = AssetBundleNodeState.Non;
        private Queue<AssetBundleLoadNode> _dependNodeQueue = null;
        private string _assetBundleName;
        private int _allCount = 0;
        private int _loadCount = 0;
        //���ؽ��Ȼص�;
        private Action<float> _onLoadProgress;

        public AssetBundleNodeState NodeState { get { return _nodeState; } }
        public AssetBundle assetBundle { get; private set; }
        public string AssetBundlePath { get; private set; }
        public float Progress { get; private set; }

        public void Init(string path, Queue<AssetBundleLoadNode> nodeQueue = null)
        {
            AssetBundlePath = path;
            Progress = 0f;
            _loadCount = 0;
            _nodeState = AssetBundleNodeState.Waitting;
            _dependNodeQueue = nodeQueue;
            _allCount = null == _dependNodeQueue ? 0 : _dependNodeQueue.Count + 1;
        }

        public void Reset()
        {
            AssetBundlePath = null;
            Progress = 0f;
            _allCount = 0;
            _loadCount = 0;
            _nodeState = AssetBundleNodeState.Non;
            _dependNodeQueue = null;
            _assetBundleName = null;
            assetBundle = null;
            _onLoadProgress = null;
            _stopwatch.Stop();
        }

        /// <summary>
        /// ��Ӽ��ؽ��Ȼص�;
        /// </summary>
        /// <param name="action"></param>
        public void AddLoadProgressCallBack(Action<float> action)
        {
            if (action != null)
            {
                _onLoadProgress += action;
            }
        }

        public void Update()
        {
            switch (_nodeState)
            {
                case AssetBundleNodeState.Non:
                    LogHelper.PrintError("[AssetBundleLoadNode]AssetBundleLoadNode is not init!");
                    _nodeState = AssetBundleNodeState.Finish;
                    break;
                case AssetBundleNodeState.Waitting:
                    _nodeState = AssetBundleNodeState.Loading;
                    Update();
                    break;
                case AssetBundleNodeState.Loading:
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    AssetBundleLoadNode _curNode = null;
                    while (true && _dependNodeQueue != null)
                    {
                        if (_dependNodeQueue.Count < 1 && null == _curNode)
                        {
                            _stopwatch.Stop();
                            break;
                        }
                        if (_dependNodeQueue.Count > 0)
                        {
                            if (null == _curNode)
                                _curNode = _dependNodeQueue.Dequeue();
                        }
                        if (_curNode.NodeState == AssetBundleNodeState.Finish)
                        {
                            PoolMgr.Instance.ReleaseCsharpObject<AssetBundleLoadNode>(_curNode);
                            _curNode = null;
                            _loadCount++;
                            UpdateProgress();
                        }
                        else
                        {
                            _curNode.Update();
                        }
                        if (_stopwatch.Elapsed.Milliseconds >= ResourceMgr.Instance.MAX_LOAD_TIME)
                        {
                            _stopwatch.Stop();
                            return;
                        }
                    }
                    _stopwatch.Stop();
                    assetBundle = AssetBundleMgr.Instance.LoadAssetBundleSync(AssetBundlePath);
                    _nodeState = AssetBundleNodeState.Finish;
                    _loadCount++;
                    UpdateProgress();
                    break;
                case AssetBundleNodeState.Error:
                    LogHelper.PrintError(string.Format("[AssetBundleLoadNode]AssetBundleLoadNode load asset:{0} error!"
                        , AssetBundlePath));
                    _nodeState = AssetBundleNodeState.Finish;
                    break;
                case AssetBundleNodeState.Finish:
                    break;
                default:
                    LogHelper.PrintError("[AssetBundleLoadNode]AssetBundleLoadNode error state!");
                    _nodeState = AssetBundleNodeState.Finish;
                    break;
            }
            return;
        }

        public void OnGet(params object[] args)
        {
        }

        public void OnRelease()
        {
            Reset();
        }

        private void UpdateProgress()
        {
            Progress = _allCount == 0 ? 1 : _loadCount / _allCount;
            if (_onLoadProgress != null)
            {
                _onLoadProgress(Progress);
            }
        }
    }
}
