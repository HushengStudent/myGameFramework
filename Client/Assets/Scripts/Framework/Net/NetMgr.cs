/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:34:59
** desc:  网络会话管理;
*********************************************************************************/

using System;

namespace Framework.NetModule
{
    //TODO:协议池+buff池自己实现,以解耦,或许更好;
    public class NetMgr : MonoSingleton<NetMgr>
    {
        private Session _session;
        private bool _checkUpdateState = false;

        protected override void AwakeEx()
        {
            base.AwakeEx();
            _session = new Session("session");
            _checkUpdateState = false;
            InitHandler();
            _session.Connect(GameConfig.IPAddress, GameConfig.Port);
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            if (_session != null && _session.Active)
            {
                _session.Update();
            }
            CheckUpdate();
        }

        public void CheckUpdate()
        {
            if (_checkUpdateState)
            {
                GameMgr.singleton.CheckUpdate();
                _checkUpdateState = false;
            }
        }

        private void InitHandler()
        {
            _session.ConnectedHandler = OnConnected;
            _session.ClosedHandler = OnClosed;
            _session.ErrorHandler = OnError;
            _session.CustomErrorHandler = OnCustomError;
            _session.ReceiveHandler = OnReceive;
            _session.SendHandler = OnSend;
        }

        private void OnSend(Session session, int count, SessionParam args)
        {

        }

        private void OnReceive(Session session, Packet packet)
        {
            packet.Process();
            ProtoHelper.ReturnPacket(packet);
        }

        private void OnCustomError(Session session, object args)
        {

        }

        private void OnError(Session session, SessionErrorCode state, string error)
        {
            LogHelper.PrintError($"[NetMgr]Session error,Session Error Code: {state},info: {error}.");
        }

        private void OnClosed(Session session)
        {

        }

        private void OnConnected(Session session, SessionParam args)
        {
            LogHelper.Print("[NetMgr]Session Connected.");
            ProtoHelper.Register();
            if (!GameMgr.singleton.CheckUpdateState)
            {
                GameMgr.singleton.CheckUpdateState = true;
                _checkUpdateState = true;
            }
        }

        public void Send<T>(T packet) where T : Packet
        {
            try
            {
                _session.Send(packet);
            }
            catch (Exception e)
            {
                LogHelper.PrintError($"[NetMgr]Send {e} error.");
                _session.Dispose();
            }
        }

        public void Send(int id, LuaBuffer buffer)
        {
            try
            {
                _session.Send(id, buffer);
            }
            catch (Exception e)
            {
                LogHelper.PrintError($"[NetMgr]Send {e} error.");
                _session.Dispose();
            }
        }
    }
}
