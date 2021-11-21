/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/20 20:50:46
** desc:  网络消息接收者;
*********************************************************************************/

using System;

namespace Framework.NetModule
{
    public class SessionReceiver
    {
        private readonly byte[] _buffer;

        public int Length;
        public int ReceivedLength;

        public int BufferSize { get { return _buffer.Length; } }

        public SessionReceiver(int bufferSize)
        {
            if (bufferSize <= 0)
            {
                throw new Exception("buffer size is invalid.");
            }
            _buffer = new byte[bufferSize];
        }

        public void Reset(int length)
        {
            Length = length;
            ReceivedLength = 0;
        }

        public byte[] GetBuffer()
        {
            return _buffer;
        }
    }
}
