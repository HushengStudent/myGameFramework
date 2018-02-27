/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/28 00:30:26
** desc:  会话
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Framework
{
    public class ClientSession<T>
    {
        /// <summary>
        /// 上下文对象;
        /// </summary>
        internal SocketAsyncEventArgs SendEventArgs { get; private set; }
        internal SocketAsyncEventArgs ReceiveEventArgs { get; private set; }
        internal SocketAsyncEventArgs ConnectEventArgs { get; private set; }

        /// <summary>
        /// 和Session关联的对象;
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 客户端对应的Socket;
        /// </summary>
        internal Socket Socket { get; private set; }

        /// <summary>
        /// 构造函数;
        /// </summary>
        public ClientSession()
        {
            Init();
        }

        ~ClientSession()
        {
            LogUtil.LogUtility.Print("[ClientSession]Session release!");
        }

        void Init()
        {
            //SendEventArgs = new SocketAsyncEventArgs();
            //SendEventArgs.Completed += OnSendCompleted;

            //ReceiveEventArgs = new SocketAsyncEventArgs();
            //ReceiveEventArgs.Completed += OnRecvCompleted;

            //ConnectEventArgs = new SocketAsyncEventArgs();
            //ConnectEventArgs.Completed += OnConnectCompleted;
        }
    }
}
