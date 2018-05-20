/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/20 22:13:33
** desc:  会话工具;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public static class SessionUtil
    {
        public static void Serialize<T>(Session networkChannel, Stream destination, T packet) where T : Packet
        {

        }

        public static Packet Deserialize(Session networkChannel, Stream source, out object customErrorData)
        {
            Packet packet = new Packet();
            customErrorData = null;
            return packet;
        }
    }
}