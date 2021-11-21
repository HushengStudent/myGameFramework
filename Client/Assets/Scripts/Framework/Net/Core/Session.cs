/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/07 23:46:47
** desc:  网络会话;
*********************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Framework.NetModule
{
    public enum ProtoType : int
    {
        Csharp = 0,
        Lua = 1,
    }

    public partial class Session
    {
        private int _defaultPacketLength = 4;//协议格式:4位长度+4位id+内容;此处指长度,4位;
        private int _defaultMaxPacketLength = 1024 * 64;
        private Socket _socket;
        private SessionReceiver _receiver;
        private bool _disposed;
        private IPAddress _ipAddress;
        private int _port;
        private object thisLock = new object();

        public SessionConnectedEventHandler ConnectedHandler = null;
        public SessionClosedEventHandler ClosedHandler = null;
        public SessionErrorEventHandler ErrorHandler = null;
        public SessionCustomErrorEventHandler CustomErrorHandler = null;
        public SessionReceiveEventHandler ReceiveHandler = null;
        public SessionSendEventHandler SendHandler = null;

        public string Name { get; }
        public SessionType Type { get; private set; }
        public bool Active { get; private set; }

        public Session(string name)
        {
            Name = name ?? string.Empty;
            Type = SessionType.Unknown;
            _socket = null;
            _receiver = null;
            Active = false;
            _disposed = false;
            ConnectedHandler = null;
            ClosedHandler = null;
            ErrorHandler = null;
            CustomErrorHandler = null;
            ReceiveHandler = null;
            SendHandler = null;
        }

        public void Update()
        {

        }

        #region Session Info

        public bool Connected
        {
            get
            {
                if (_socket != null)
                {
                    return _socket.Connected;
                }
                return false;
            }
        }

        public IPAddress LocalIPAddress
        {
            get
            {
                if (_socket == null)
                {
                    throw new Exception("not initialize network.");
                }
                var ipEndPoint = (IPEndPoint)_socket.LocalEndPoint;
                if (ipEndPoint == null)
                {
                    throw new Exception("localIPAddress is invalid.");
                }
                return ipEndPoint.Address;
            }
        }

        public int LocalPort
        {
            get
            {
                if (_socket == null)
                {
                    throw new Exception("not initialize network.");
                }
                var ipEndPoint = (IPEndPoint)_socket.LocalEndPoint;
                if (ipEndPoint == null)
                {
                    throw new Exception("LocalPort is invalid.");
                }
                return ipEndPoint.Port;
            }
        }

        public IPAddress RemoteIPAddress
        {
            get
            {
                if (_socket == null)
                {
                    throw new Exception("not initialize network.");
                }
                var ipEndPoint = (IPEndPoint)_socket.RemoteEndPoint;
                if (ipEndPoint == null)
                {
                    throw new Exception("RemoteIPAddress is invalid.");
                }
                return ipEndPoint.Address;
            }
        }

        public int RemotePort
        {
            get
            {
                if (_socket == null)
                {
                    throw new Exception("not initialize network.");
                }
                var ipEndPoint = (IPEndPoint)_socket.RemoteEndPoint;
                if (ipEndPoint == null)
                {
                    throw new Exception("RemotePort is invalid.");
                }
                return ipEndPoint.Port;
            }
        }

        public int ReceiveBufferSize
        {
            get
            {
                if (_socket == null)
                {
                    throw new Exception("not initialize network.");
                }
                return _socket.ReceiveBufferSize;
            }
            set
            {
                if (_socket == null)
                {
                    throw new Exception("not initialize network.");
                }
                _socket.ReceiveBufferSize = value;
            }
        }

        public int SendBufferSize
        {
            get
            {
                if (_socket == null)
                {
                    throw new Exception("not initialize network.");
                }
                return _socket.SendBufferSize;
            }
            set
            {
                if (_socket == null)
                {
                    throw new Exception("not initialize network.");
                }
                _socket.SendBufferSize = value;
            }
        }

        #endregion

        #region Session Connect

        public void Connect(IPAddress ipAddress, int port)
        {
            Initialize(ipAddress.AddressFamily);
            _ipAddress = ipAddress;
            _port = port;
            if (_socket == null)
            {
                var errorMessage = "initialize failure.";
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.StateError, errorMessage);
                    return;
                }
                throw new Exception(errorMessage);
            }
            try
            {
                var param = new SessionParam(_socket, _ipAddress, _port);
                _socket.BeginConnect(_ipAddress, _port, ConnectCallback, param);
            }
            catch (Exception exception)
            {
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.StateError, exception.Message);
                    return;
                }
                throw;
            }
        }

        private void Initialize(AddressFamily addressFamily)
        {
            if (_socket != null)
            {
                Close();
                _socket = null;
            }
            switch (addressFamily)
            {
                case AddressFamily.InterNetwork:
                    Type = SessionType.IPv4;
                    break;
                case AddressFamily.InterNetworkV6:
                    Type = SessionType.IPv6;
                    break;
                default:
                    throw new Exception($"Not supported address family '{addressFamily.ToString()}'.");
            }
            _socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
            _receiver = new SessionReceiver(_defaultMaxPacketLength);
            _receiver.Reset(_defaultPacketLength);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            var param = (SessionParam)ar.AsyncState;
            try
            {
                param.socket.EndConnect(ar);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception exception)
            {
                Active = false;
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.ConnectError, exception.Message);
                    return;
                }
                throw;
            }
            Active = true;
            ConnectedHandler?.Invoke(this, param);
            Receive();
        }

        #endregion

        #region Session Send

        public void Send(byte[] buffer, int offset, int size)
        {
            if (_socket == null)
            {
                var errorMessage = "session not initialize.";
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.StateError, errorMessage);
                    return;
                }
                throw new Exception(errorMessage);
            }
            try
            {
                var param = new SessionParam(_socket, _ipAddress, _port);
                _socket.BeginSend(buffer, offset, size, SocketFlags.None, SendCallback, param);
            }
            catch (Exception exception)
            {
                Active = false;
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.StateError, exception.Message);
                    return;
                }
                throw;
            }
        }


        public void Send(int id, LuaBuffer buffer)
        {
            if (_socket == null)
            {
                var errorMessage = "session not initialize.";
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.StateError, errorMessage);
                    return;
                }
                throw new Exception(errorMessage);
            }
            var bufferBytes = buffer.ToBytes();
            if (bufferBytes == null)
            {
                var errorMessage = "luaBuff is null.";
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.StateError, errorMessage);
                    return;
                }
                throw new Exception(errorMessage);
            }
            try
            {
                var length = 0;
                var packetLength = 0;
                var packetBuffer = new byte[_defaultMaxPacketLength];//TODO:内存池;
                using (var memoryStream = new MemoryStream(packetBuffer, true))
                {
                    memoryStream.Seek(_defaultPacketLength, SeekOrigin.Begin);
                    var idBytes = ConvertHelper.GetBytes(id);
                    memoryStream.Write(idBytes, 0, idBytes.Length);
                    memoryStream.Write(bufferBytes, 0, bufferBytes.Length);
                    length = (int)memoryStream.Position;
                }
                packetLength = length - _defaultPacketLength;
                ConvertHelper.GetBytes(packetLength).CopyTo(packetBuffer, 0);
                Send(packetBuffer, 0, length);
            }
            catch (Exception exception)
            {
                Active = false;
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.SerializeError, exception.ToString());
                    return;
                }
                throw;
            }
        }

        public void Send<T>(T packet) where T : Packet
        {
            if (_socket == null)
            {
                var errorMessage = "session not initialize.";
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.StateError, errorMessage);
                    return;
                }
                throw new Exception(errorMessage);
            }
            if (packet == null)
            {
                var errorMessage = "Packet is invalid.";
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.StateError, errorMessage);
                    return;
                }
                throw new Exception(errorMessage);
            }
            try
            {
                var length = 0;
                var packetLength = 0;
                var packetBuffer = new byte[_defaultMaxPacketLength];//TODO:内存池;
                using (var memoryStream = new MemoryStream(packetBuffer, true))
                {
                    memoryStream.Seek(_defaultPacketLength, SeekOrigin.Begin);
                    SessionUtil.Serialize(this, memoryStream, packet);
                    length = (int)memoryStream.Position;
                }
                packetLength = length - _defaultPacketLength;
                ConvertHelper.GetBytes(packetLength).CopyTo(packetBuffer, 0);
                Send(packetBuffer, 0, length);
            }
            catch (Exception exception)
            {
                Active = false;
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.SerializeError, exception.ToString());
                    return;
                }
                throw;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            var param = (SessionParam)ar.AsyncState;

            var bytesSent = 0;
            try
            {
                bytesSent = param.socket.EndSend(ar);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception exception)
            {
                Active = false;
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.SendError, exception.Message);
                    return;
                }

                throw;
            }
            SendHandler?.Invoke(this, bytesSent, param);
        }

        #endregion

        #region Session Receive

        private void Receive()
        {
            try
            {
                _socket.BeginReceive(_receiver.GetBuffer(), _receiver.ReceivedLength, _receiver.Length - _receiver.ReceivedLength, SocketFlags.None, ReceiveCallback, _socket);
            }
            catch (Exception exception)
            {
                Active = false;
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.ReceiveError, exception.Message);
                    return;
                }
                throw;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var socket = (Socket)ar.AsyncState;
            var bytesReceived = 0;
            try
            {
                bytesReceived = socket.EndReceive(ar);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception exception)
            {
                Active = false;
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.ReceiveError, exception.Message);
                    return;
                }

                throw;
            }
            if (bytesReceived <= 0)
            {
                Close();
                return;
            }
            _receiver.ReceivedLength += bytesReceived;
            if (_receiver.ReceivedLength < _receiver.Length)
            {
                Receive();
                return;
            }
            var processSuccess = false;
            try
            {
                processSuccess = Process();//返回false,继续接收;
            }
            catch (Exception exception)
            {
                Active = false;
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.StreamError, exception.Message);
                    return;
                }

                throw;
            }
            if (processSuccess)
            {
                Receive();
                return;
            }
        }

        private bool Process()
        {
            if (_receiver.ReceivedLength != _receiver.Length)
            {
                throw new Exception($"Receive length '{_receiver.ReceivedLength}' is not equal to length '{_receiver.Length}'.");
            }
            if (_receiver.Length < _defaultPacketLength)
            {
                throw new Exception($"Length '{_receiver.Length}' is smaller than length header.");
            }
            if (_receiver.Length == _defaultPacketLength)
            {
                var packetLength = ConvertHelper.GetInt32(_receiver.GetBuffer());
                if (packetLength <= 0)
                {
                    var errorMessage = $"Packet length '{packetLength}' is invalid.";
                    if (ErrorHandler != null)
                    {
                        ErrorHandler(this, SessionErrorCode.HeaderError, errorMessage);
                        return false;
                    }
                    throw new Exception(errorMessage);
                }
                _receiver.Length += packetLength;
                if (_receiver.Length > _receiver.BufferSize)
                {
                    var errorMessage = $"Length '{_receiver.Length}' is larger than buffer size '{_receiver.BufferSize}'.";
                    if (ErrorHandler != null)
                    {
                        ErrorHandler(this, SessionErrorCode.OutOfRangeError, errorMessage);//未接收完成,继续;
                        return false;
                    }
                    throw new Exception(errorMessage);
                }
                return true;
            }
            Packet packet = null;
            try
            {
                var packetLength = _receiver.Length - _defaultPacketLength;
                object customErrorData = null;
                using (var memoryStream = new MemoryStream(_receiver.GetBuffer(), _defaultPacketLength, packetLength, false))
                {
                    lock (thisLock)
                    {
                        packet = SessionUtil.Deserialize(this, memoryStream, out customErrorData);
                    }
                }
                _receiver.Reset(_defaultPacketLength);
                if (packet == null)
                {
                    CustomErrorHandler?.Invoke(this, customErrorData);
                }
                else
                {
                    var id = packet.GetPacketId();
                    if (LuaProtoRegister.LuaProtoDict.TryGetValue(id, out var luaOverride))
                    {
                        if (!luaOverride)
                        {
                            ReceiveHandler?.Invoke(this, packet);
                        }
                        using (var memoryStream = new MemoryStream())
                        {
                            packet.Serialize(memoryStream);
                            LuaNetHelper.Send2Lua(id, memoryStream.ToArray());
                        }
                    }
                    else
                    {
                        ReceiveHandler?.Invoke(this, packet);
                    }
                }
            }
            catch (Exception exception)
            {
                Active = false;
                if (ErrorHandler != null)
                {
                    ErrorHandler(this, SessionErrorCode.DeserializeError, exception.ToString());
                    return false;
                }
                throw;
            }
            return true;
        }

        #endregion

        #region Session Close

        public void Close()
        {
            if (_socket == null)
            {
                return;
            }
            Active = false;
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }
            finally
            {
                _socket.Close();
                _socket = null;
                _receiver = null;
                ClosedHandler?.Invoke(this);
            }
        }

        #endregion

        #region Session Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                Close();
            }
            _disposed = true;
        }

        #endregion

    }
}