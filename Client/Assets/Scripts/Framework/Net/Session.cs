/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/07 23:46:47
** desc:  网络会话
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Framework
{
    public enum NetType:int
    {
        Non = 0,
        Ipv4,
        Ipv6
    }

    public class Session
    {
        private const int _defaultPacketHeaderLength = 4;
        private const int _defaultMaxPacketLength = 1024 * 32;
        private const float _defaultHeartBeatInterval = 30f;

        private Socket _socket;
        private bool _active;
        private bool _disposed;

        public Session()
        {

        }
    }
}
