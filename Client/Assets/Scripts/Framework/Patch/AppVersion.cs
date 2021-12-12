/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/07 15:50:22
** desc:  �汾������Ϣ;
*********************************************************************************/

namespace Framework.PatchModule
{
    /*
     * ���²���:
     * Lua:ÿ���漰�ű�����,����һ��ȫ���ű����²���,����ȫ���ű��ļ�;
     * Resource:ÿ������Դ����,������һ���ڵ�汾(lastStepVersion),�����°汾(newestVersion)֮���
     * (newestVersion-lastStepVersion)�����°�;
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

        public AppVersionCompareResult AppVersionCompare(AppVersion nextAppVersion)
        {
            if (null == nextAppVersion)
            {
                LogHelper.PrintError("appVersion compare error.");
                return AppVersionCompareResult.Non;
            }
            if (Channel != nextAppVersion.Channel)
            {
                return AppVersionCompareResult.ChannelError;
            }
            var resourceCanUpdate = Resource != nextAppVersion.Resource;
            var luaCanUpdate = Lua != nextAppVersion.Lua;
            if (resourceCanUpdate && luaCanUpdate)
            {
                return AppVersionCompareResult.ResourceCanUpdate | AppVersionCompareResult.LuaCanUpdate;
            }
            if (resourceCanUpdate)
            {
                return AppVersionCompareResult.ResourceCanUpdate;
            }
            if (luaCanUpdate)
            {
                return AppVersionCompareResult.LuaCanUpdate;
            }
            return AppVersionCompareResult.Non;
        }
    }
}