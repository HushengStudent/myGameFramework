public class Process_LoginResponse
{
    public static void Process(Packet_LoginResponse packet)
    {
        LogHelper.PrintGreen($"--->>>LoginResponse ID£º{packet.Data.id}");
    }
}
