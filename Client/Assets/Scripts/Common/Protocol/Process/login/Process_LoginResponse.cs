public class Process_LoginResponse
{
    public static void Process(Packet_LoginResponse packet)
    {
        LogHelper.Print("--->>>Process_LoginResponse", LogColor.Green);
        LogHelper.Print("--->>>ID:" + packet.Data.id, LogColor.Green);
    }
}
