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

        }

        private void OnReceive(Session session, Packet packet)
        {
            packet.Process();
            ProtoRegister.ReturnPacket(packet);
        }

        private void OnCustomError(Session session, object args)
        {

        }

        private void OnError(Session session, SessionErrorCode state, string error)
        {
            LogUtil.LogUtility.PrintError(string.Format("[NetMgr]Session error,Session Error Code: {0},info: {1}", state.ToString(), error.ToString()));
        }

        private void OnClosed(Session session)
        {

        }

        private void OnConnected(Session session, SessionParam args)
        {
            LogUtil.LogUtility.Print(string.Format("[NetMgr]Session error,Session Connected!"));
            ProtoRegister.Register();
        }

        public void Send<T>(T packet) where T : Packet
        {
            try
            {
                session.Send<T>(packet);
            }
            catch (Exception e)
            {
                LogUtil.LogUtility.PrintError(string.Format("[NetMgr]Send {0} error!", e.ToString()));
                session.Dispose();
            }
        }
    }
}
