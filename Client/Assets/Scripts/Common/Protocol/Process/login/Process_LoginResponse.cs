using System.Collections;
using System.Collections.Generic;

public class Process_LoginResponse
{
    public static void Process(Packet_LoginResponse packet)
    {
        LogHelper.Print("+++++>>>>>C# process protocol!", LogColor.Green);
        LogHelper.Print("+++++>>>>>id:" + packet.Data.id, LogColor.Green);
    }
}
