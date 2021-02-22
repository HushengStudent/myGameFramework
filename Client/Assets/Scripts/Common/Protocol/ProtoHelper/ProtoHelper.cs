using Framework;
using System;
using System.Collections.Generic;

public partial class ProtoHelper
{
    private static Dictionary<int, PacketFactory> _factoryDict
        = new Dictionary<int, PacketFactory>();

    private static readonly object _lock = new object();

    private static void RegisterProto(Type type)
    {
        var instance = Activator.CreateInstance(type);
        if (instance is Packet)
        {
            var packet = instance as Packet;
            var packetId = packet.GetPacketId();
            if (!_factoryDict.ContainsKey(packetId))
            {
                _factoryDict[packetId] = new PacketFactory(type);
            }
            ReturnPacket(packet);
        }
        else
        {
            LogHelper.PrintError($"[ProtoHelper]RegisterProto:{type.ToString()} error.");
        }
    }

    public static Packet GetPacket(int type)
    {
        lock (_lock)
        {
            if (_factoryDict.ContainsKey(type))
            {
                return _factoryDict[type].GetPacket();
            }
            else
            {
                LogHelper.PrintError($"[ProtoHelper]GetPacket:{type.ToString()} error.");
                return null;
            }
        }
    }

    public static void ReturnPacket(Packet packet)
    {
        lock (_lock)
        {
            var packetId = packet.GetPacketId();
            if (_factoryDict.ContainsKey(packetId))
            {
                _factoryDict[packetId].ReturnPacket(packet);
            }
            else
            {
                LogHelper.PrintError($"[ProtoHelper]ReturnPacket:{packetId.ToString()} error.");
            }
        }
    }

    public static void Register()
    {
        _factoryDict.Clear();
        RegisterGeneratedProto();
    }
}
