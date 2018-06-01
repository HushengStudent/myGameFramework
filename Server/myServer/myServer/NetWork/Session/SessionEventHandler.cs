using System.Collections;
using System.Collections.Generic;

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
