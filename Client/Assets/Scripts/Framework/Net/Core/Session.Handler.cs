/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/07 23:57:14
** desc:  网络事件;
*********************************************************************************/

namespace Framework
{
    public partial class Session
    {
        /// <summary> 网络连接; </summary>
        public delegate void SessionConnectedEventHandler(Session session, SessionParam args);
        /// <summary> 网络关闭; </summary>
        public delegate void SessionClosedEventHandler(Session session);
        /// <summary> session错误; </summary>
        public delegate void SessionErrorEventHandler(Session session, SessionErrorCode state, string error);
        /// <summary> 通信错误; </summary>
        public delegate void SessionCustomErrorEventHandler(Session session, object args);
        /// <summary> 接收; </summary>
        public delegate void SessionReceiveEventHandler(Session session, Packet packet);
        /// <summary> 发送; </summary>
        public delegate void SessionSendEventHandler(Session session, int count, SessionParam args);
    }
}