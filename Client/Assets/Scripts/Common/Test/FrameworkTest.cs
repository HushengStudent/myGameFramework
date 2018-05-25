/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/23 23:40:04
** desc:  测试;
*********************************************************************************/

using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameworkTest : MonoBehaviour
{
    public void NetWorkTest()
    {
        Packet_LoginRequest req = new Packet_LoginRequest();
        req.Data.id = 1001;
        req.Data.name = "HushengStudent";
        NetMgr.Instance.Send<Packet_LoginRequest>(req);
    }
}
