/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/01 00:04:40
** desc:  lua协议工具;
*********************************************************************************/

using LuaInterface;
using System;

namespace Framework
{
    public static class LuaNetHelper
    {
        public static void SendLuaReq(int id, LuaBuffer buffer)
        {
            NetMgr.singleton.Send(id, buffer);
        }

        public static void Send2Lua(int id, byte[] bytes)
        {
            try
            {
                var byteBuffer = new LuaByteBuffer(bytes);
                LuaUtility.CallLuaModuleMethod("Protol.ProtoProcess", "Process", id, byteBuffer);
            }
            catch (Exception e)
            {
                LogHelper.PrintError($"[LuaNetUtility]Send2Lua error,id:{id},info:{e}.");
            }
        }
    }
}
