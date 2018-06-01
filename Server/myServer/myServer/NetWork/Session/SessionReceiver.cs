using System;
using System.Collections;
using System.Collections.Generic;

public class SessionReceiver
{
    private readonly byte[] _buffer;
    private int _length;
    private int _receivedLength;

    public int BufferSize { get { return _buffer.Length; } }
    public int Length { get { return _length; } set { _length = value; } }
    public int ReceivedLength { get { return _receivedLength; } set { _receivedLength = value; } }

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
