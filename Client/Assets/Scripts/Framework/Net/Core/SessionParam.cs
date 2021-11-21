/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/20 21:30:51
** desc:  会话参数;
*********************************************************************************/

using System.Net;
using System.Net.Sockets;

namespace Framework.NetModule
{
    public class SessionParam
    {
        public Socket socket { get; private set; }
        public IPEndPoint EndPoint { get; private set; }

        public SessionParam(Socket socket, IPAddress addr, int port)
        {
            this.socket = socket;
            EndPoint = new IPEndPoint(addr, port);
        }
    }
}