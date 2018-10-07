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
        private string _versionFilePath = Application.dataPath.ToLower() + "/../Config/Version.xml";
        private string _netVersionFilePath = Application.dataPath.ToLower() + "/../Config/temp/Version.xml";

        private VersionInfo _localVersionInfo;
        private VersionInfo _netVersionInfo;

        public VersionInfo LoaclVersion { get { return _localVersionInfo; } }
        public VersionInfo NetVersion { get { return _netVersionInfo; } }

        public UpdateStartEventHandler StartHandler;
        public UpdateErrorEventHandler ErrorHandler;
        public UpdateSuccessEventHandler SuccessHandler;

        protected override void InitEx()
        {
            base.InitEx();
            SuccessHandler = () => { GameMgr.Instance.EnterGame(); };
            if (!Directory.Exists(_versionFilePath))
            {
                VersionInfo info = new VersionInfo();
                SerializeUtility.SerializeXml<VersionInfo>(_versionFilePath, info);
            }
            _localVersionInfo = SerializeUtility.DeserializeBinary<VersionInfo>(_versionFilePath);
        }

        public void CheckVersion()
        {
            WWWDownLoadHelper www = new WWWDownLoadHelper();
            www.SuccessHandler = () =>
            {
                _netVersionInfo = SerializeUtility.DeserializeBinary<VersionInfo>(_netVersionFilePath);
                CheckUpdate();
            };
            CoroutineMgr.Instance.StartCoroutine(www.StartDownLoad(_localVersionInfo._updateUrl, _netVersionFilePath));
        }

        public void CheckUpdate()
        {

        }
    }
}