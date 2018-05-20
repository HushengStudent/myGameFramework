/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:34:59
** desc:  网络会话管理;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class NetMgr : MonoSingleton<NetMgr>
    {
        private Session session;

        protected override void Init()
        {
            base.Init();
            session = new Session("session");
            InitHandler();
            session.Connect(GameConfig.ipAddress, GameConfig.port);
        }

        public override void UpdateEx()
        {
            base.UpdateEx();
            if (session.Active)
                session.Update();
        }

        private void InitHandler()
        {
            session.ConnectedHandler += OnConnected;
            session.ClosedHandler += OnClosed;
            session.ErrorHandler += OnError;
            session.CustomErrorHandler += OnCustomError;
            session.ReceiveHandler += OnReceive;
            session.SendHandler += OnSend;
        }

        private void OnSend(Session session, int count, SessionParam args)
        {
            throw new NotImplementedException();
        }

        private void OnReceive(Session session, Packet packet)
        {
            throw new NotImplementedException();
        }

        private void OnCustomError(Session session, object args)
        {
            throw new NotImplementedException();
        }

        private void OnError(Session session, SessionState state, string error)
        {
            throw new NotImplementedException();
        }

        private void OnClosed(Session session)
        {
            throw new NotImplementedException();
        }

        private void OnConnected(Session session, SessionParam args)
        {
            throw new NotImplementedException();
        }
    }
}
