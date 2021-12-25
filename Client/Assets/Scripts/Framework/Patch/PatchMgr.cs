/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/23 00:53:17
** desc:  热更管理;
*********************************************************************************/

using System.IO;

namespace Framework.PatchModule
{
    public class PatchMgr : Singleton<PatchMgr>
    {
        public AppVersion LoaclAppVersion { get; private set; }
        public AppVersion NetAppVersion { get; private set; }

        public PatchStartEventHandler StartHandler = null;
        public PatchErrorEventHandler ErrorHandler = null;
        public PatchSuccessEventHandler SuccessHandler = null;

        protected override void OnInitialize()
        {
            SuccessHandler = () =>
            {
                GameMgr.singleton.EnterGame();
            };
            if (!Directory.Exists(GameConfig.LoaclAppVersionFilePath))
            {
                //拷贝版本里面的版本信息;
            }
            LoaclAppVersion = SerializeHelper.DeserializeXml<AppVersion>(GameConfig.LoaclAppVersionFilePath);
            CheckNetVersion();
        }

        /// <summary>
        /// 拉取服务器上的版本信息;
        /// </summary>
        private void CheckNetVersion()
        {
            var unityWebRequest = new UnityWebRequestHelper
            {
                SuccessHandler = () =>
                {
                    NetAppVersion = SerializeHelper.DeserializeXml<AppVersion>(GameConfig.NetAppVersionFilePath);
                    CheckUpdate();
                },
                ErrorHandler = (string error) =>
                {
                    LogHelper.PrintError(error);

                    //重试;
                    CheckNetVersion();
                },
            };
            CoroutineMgr.singleton.StartCoroutine(unityWebRequest.StartDownLoad(GameConfig.NetAppVersionUrl, GameConfig.NetAppVersionFilePath));
        }

        private void CheckUpdate()
        {
            var result = LoaclAppVersion.AppVersionCompare(NetAppVersion);
            if (result == AppVersionCheckResult.Non)
            {
                //没有更新;
                SuccessHandler?.Invoke();
                return;
            }
            if (result == AppVersionCheckResult.ChannelError)
            {
                return;
            }
            if (result == (AppVersionCheckResult.ResourceCanUpdate
                | AppVersionCheckResult.LuaCanUpdate))
            {
                return;
            }
            if (result == AppVersionCheckResult.ResourceCanUpdate)
            {
                return;
            }
            if (result == AppVersionCheckResult.LuaCanUpdate)
            {
                return;
            }
        }

        private void UpdateLua()
        {
            //下载;

            //解压;
        }

        private void UpdateResource()
        {
            //下载;

            //解压;
        }

        private bool CheckChangeList()
        {
            //根据服务器对比ChangeList;

            return true;
        }
    }
}