/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/07 23:57:14
** desc:  网络事件
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public delegate void NetConnectEventHandler(Session session, object args);
    public delegate void NetCloseEventHandler(Session session);
    public delegate void NetSendEventHandler(Session session, int id, object args);
    public delegate void NetReceiveEventHandler(Session session, NetPacket args);
    public delegate void NetMissBeatEventHandler(Session session, int id);
    public delegate void NetErrorEventHandler(Session session, NetCode code, string str);
    public delegate void NetCustomErrorEventHandler(Session session, object args);
}