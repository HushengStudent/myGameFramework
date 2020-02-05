/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/07 23:57:14
** desc:  网络事件;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public partial class Session
    {
        //网络连接;
        public delegate void SessionConnectedEventHandler(Session session, SessionParam args);
        //网络关闭;
        public delegate void SessionClosedEventHandler(Session session);
        //session错误;
        public delegate void SessionErrorEventHandler(Session session, SessionErrorCode state, string error);
        //通信错误;
        public delegate void SessionCustomErrorEventHandler(Session session, object args);
        //接收;
        public delegate void SessionReceiveEventHandler(Session session, Packet packet);
        //发送;
        public delegate void SessionSendEventHandler(Session session, int count, SessionParam args);
    }
}