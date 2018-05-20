/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/08 00:05:29
** desc:  网络协议包;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public class Packet
    {
        private int _id;
        public int Id { get { return _id; } set { _id = value; } }

        public virtual void Process() { }
    }
}
