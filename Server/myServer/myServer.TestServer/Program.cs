using myServerFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace myServer.TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServerAsync();
            Console.ReadKey();
        }

        static void StartServerAsync()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 10000);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallBack, serverSocket);
        }

        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket = serverSocket.EndAccept(ar);
            Console.WriteLine("connect sucess!");
            Session session = new Session("Test");
            session.socket = clientSocket;
            EndPoint endPoint = clientSocket.RemoteEndPoint;
            string[] clientInfo = (endPoint.ToString()).Split(":".ToArray());
            session.ipAddress = IPAddress.Parse(clientInfo[0]);
            session.Port = int.Parse(clientInfo[1]);
            Packet_LoginResponse rsp = new Packet_LoginResponse();
            rsp.Data.id = 123;
            session.Send<Packet_LoginResponse>(rsp);
            Console.WriteLine(string.Format("send to client: {0} !",rsp.Data.id));
        }
    }
}
