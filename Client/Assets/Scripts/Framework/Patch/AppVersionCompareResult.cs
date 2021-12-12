/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2021/12/12 22:30:03
** desc:  版本比较结果;
*********************************************************************************/

namespace Framework.PatchModule
{
    public enum AppVersionCompareResult : byte
    {
        Non = 1 << 0,
        //渠道错误;
        ChannelError = 1 << 1,
        //有新的App版本,前往应用市场下载新版本;
        AppCanUpdate = 1 << 2,
        //资源可更新;
        ResourceCanUpdate = 1 << 3,
        //脚本可更新;
        LuaCanUpdate = 1 << 4,
    }
}