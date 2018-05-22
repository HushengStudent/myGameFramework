/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/23 00:17:00
** desc:  协议工厂;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class PacketFactory
    {
        private Type _type;
        //TODO:packet回收时数据重置;
        private Queue<Packet> _queue;

        public PacketFactory(Type packet)
        {
            _type = packet;
            _queue = new Queue<Packet>();
        }

        public Packet CreatePacket()
        {
            if (null != _type)
            {
                System.Object target = Activator.CreateInstance(_type);
                if (target is Packet)
                {
                    return target as Packet;
                }
            }
            return null;
        }

        public Packet GetPacket()
        {
            if (_queue.Count > 0)
            {
                return _queue.Dequeue();
            }
            return CreatePacket();
        }

        public void ReturnPacket(Packet packet)
        {
            _queue.Enqueue(packet);
        }
    }
}