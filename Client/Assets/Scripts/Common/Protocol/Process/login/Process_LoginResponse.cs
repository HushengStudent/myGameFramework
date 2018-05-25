using System.Collections;
using System.Collections.Generic;

public class Process_LoginResponse
{
    public static void Process(Packet_LoginResponse packet)
    {
        LogUtil.LogUtility.PrintError("=====>>>>>Recive from server!");
        LogUtil.LogUtility.PrintError(string.Format("get id: {0} !", packet.Data.id));
    }
}
