using System.Collections;
using System.Collections.Generic;

public class Process_LoginResponse
{
    public static void Process(Packet_LoginResponse packet)
    {
        LogUtil.LogUtility.Print("+++++>>>>>C# process protocol!", LogUtil.LogColor.Green);
        LogUtil.LogUtility.Print("+++++>>>>>id:" + packet.Data.id, LogUtil.LogColor.Green);
    }
}
