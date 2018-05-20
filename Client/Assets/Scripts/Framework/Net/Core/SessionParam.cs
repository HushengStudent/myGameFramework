/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/20 21:30:51
** desc:  会话参数;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Framework
{
    public class SessionParam
    {
        private Socket _socket;
        private IPEndPoint _endPoint;

        public Socket socket { get { return _socket; } }
        public IPEndPoint EndPoint { get { return _endPoint; } }

        public SessionParam(Socket socket, IPAddress addr, int port)
        {
            _socket = socket;
            _endPoint = new IPEndPoint(addr, port);
        }
    }
}