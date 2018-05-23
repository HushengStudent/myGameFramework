using Framework;
using System;
using System.Collections;
using System.Collections.Generic;

public class ProtoRegister
{
    private static Dictionary<int, PacketFactory> _factory = new Dictionary<int, PacketFactory>();

    private static object thisLock = new object();

    private static void RegisterProto(Type type)
    {
        System.Object target = Activator.CreateInstance(type);
        if (target is Packet)
        {
            Packet packet = target as Packet;
            _factory[packet.GetPacketId()] = new PacketFactory(type);
            ReturnPacket(packet);
        }
        else
        {
            LogUtil.LogUtility.PrintError(string.Format("[ProtoRegister]Register {0} error!", type.ToString()));
        }
    }

    public static Packet GetPacket(int type)
    {
        lock (thisLock)
        {
            if (_factory.ContainsKey(type))
            {
                return _factory[type].GetPacket();
            }
            else
            {
                LogUtil.LogUtility.PrintError(string.Format("[ProtoRegister]UnRegister {0}!", type.ToString()));
                return null;
            }
        }
    }

    public static void ReturnPacket(Packet packet)
    {
        lock (thisLock)
        {
            int type = packet.GetPacketId();
            if (_factory.ContainsKey(type))
            {
                _factory[type].ReturnPacket(packet);
            }
            else
            {
                LogUtil.LogUtility.PrintError(string.Format("[ProtoRegister]UnRegister {0}!", type.ToString()));
            }
        }
    }

    public static void Register()
    {
        _factory.Clear();
        RegisterProto(typeof(Packet_LoginResponse));
    }
}
