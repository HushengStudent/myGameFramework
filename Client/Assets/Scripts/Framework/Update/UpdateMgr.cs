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
        public VersionInfo LoaclVersion { get; private set; }
        public VersionInfo NetVersion { get; private set; }

        public UpdateStartEventHandler StartHandler = null;
        public UpdateErrorEventHandler ErrorHandler = null;
        public UpdateSuccessEventHandler SuccessHandler = null;

        public override void Init()
        {
            base.Init();
            SuccessHandler = () => { GameMgr.Instance.EnterGame(); };
            if (!Directory.Exists(GameConfig.VersionFilePath))
            {
                VersionInfo info = new VersionInfo();
                SerializeHelper.SerializeXml<VersionInfo>(GameConfig.VersionFilePath, info);
            }
            LoaclVersion = SerializeHelper.DeserializeXml<VersionInfo>(GameConfig.VersionFilePath);

            //CheckVersion();
        }

        public void CheckVersion()
        {
            WWWDownLoadHelper www = new WWWDownLoadHelper();
            www.SuccessHandler = () =>
            {
                NetVersion = SerializeHelper.DeserializeXml<VersionInfo>(GameConfig.NetVersionFilePath);
                CheckUpdate();
            };
            CoroutineMgr.Instance.StartCoroutine(www.StartDownLoad(LoaclVersion._updateUrl, GameConfig.NetVersionFilePath));
        }

        public void CheckUpdate()
        {

        }
    }
}