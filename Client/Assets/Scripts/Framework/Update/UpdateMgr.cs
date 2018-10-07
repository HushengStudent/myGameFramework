/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/23 00:53:17
** desc:  热更管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public class UpdateMgr : Singleton<UpdateMgr>
    {
        private VersionInfo _localVersionInfo;
        private VersionInfo _netVersionInfo;

        public VersionInfo LoaclVersion { get { return _localVersionInfo; } }
        public VersionInfo NetVersion { get { return _netVersionInfo; } }

        public UpdateStartEventHandler StartHandler;
        public UpdateErrorEventHandler ErrorHandler;
        public UpdateSuccessEventHandler SuccessHandler;

        public override void Init()
        {
            base.Init();
            SuccessHandler = () => { GameMgr.Instance.EnterGame(); };
            if (!Directory.Exists(GameConfig.VersionFilePath))
            {
                VersionInfo info = new VersionInfo();
                SerializeUtility.SerializeXml<VersionInfo>(GameConfig.VersionFilePath, info);
            }
            _localVersionInfo = SerializeUtility.DeserializeXml<VersionInfo>(GameConfig.VersionFilePath);
        }

        public void CheckVersion()
        {
            WWWDownLoadHelper www = new WWWDownLoadHelper();
            www.SuccessHandler = () =>
            {
                _netVersionInfo = SerializeUtility.DeserializeXml<VersionInfo>(GameConfig.NetVersionFilePath);
                CheckUpdate();
            };
            CoroutineMgr.Instance.StartCoroutine(www.StartDownLoad(_localVersionInfo._updateUrl, GameConfig.NetVersionFilePath));
        }

        public void CheckUpdate()
        {

        }
    }
}