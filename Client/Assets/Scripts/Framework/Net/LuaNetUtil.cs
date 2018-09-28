/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/01 00:04:40
** desc:  lua协议发送工具;
*********************************************************************************/

using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class LuaNetUtil
    {
        public static void SendLuaReq(int id, LuaBuffer buffer)
        {
            NetMgr.Instance.Send(id, buffer);
        }

        public static void Send2Lua(byte[] bytes)
        {
            LuaBuffer buffer = new LuaBuffer();
            buffer.WriteBytes(bytes);
            LuaUtility.CallLuaTableMethod("Protol.ProtoProcess", "Process", buffer);
        }
    }
}
