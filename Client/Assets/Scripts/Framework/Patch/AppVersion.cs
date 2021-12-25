/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/07 15:50:22
** desc:  版本控制信息;
*********************************************************************************/

namespace Framework.PatchModule
{
    /*
     * 更新策略:
     * Lua:每次涉及脚本更新,都出一个全量脚本更新补丁,更新全部脚本文件;
     * Resource:每次有资源更新,都出上一个节点版本(lastStepVersion),到最新版本(newestVersion)之间的
     * (newestVersion-lastStepVersion)个更新包;
     */
    public class AppVersion
    {
        public AppVersion(int channel, int app, int resource, int lua)
        {
            Channel = channel;
            App = app;
            Resource = resource;
            Lua = lua;
        }

        public int Channel { get; private set; }
        public int App { get; private set; }
        public int Resource { get; private set; }
        public int Lua { get; private set; }

        public AppVersionCheckResult AppVersionCompare(AppVersion nextAppVersion)
        {
            if (null == nextAppVersion)
            {
                LogHelper.PrintError("appVersion compare error.");
                return AppVersionCheckResult.Non;
            }
            if (Channel != nextAppVersion.Channel)
            {
                return AppVersionCheckResult.ChannelError;
            }
            var resourceCanUpdate = Resource != nextAppVersion.Resource;
            var luaCanUpdate = Lua != nextAppVersion.Lua;
            if (resourceCanUpdate && luaCanUpdate)
            {
                return AppVersionCheckResult.ResourceCanUpdate | AppVersionCheckResult.LuaCanUpdate;
            }
            if (resourceCanUpdate)
            {
                return AppVersionCheckResult.ResourceCanUpdate;
            }
            if (luaCanUpdate)
            {
                return AppVersionCheckResult.LuaCanUpdate;
            }
            return AppVersionCheckResult.Non;
        }
    }
}