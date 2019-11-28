public class Process_LoginResponse
{
    public static void Process(Packet_LoginResponse packet)
    {
        LogHelper.Print($"--->>>LoginResponse ID£º{packet.Data.id}", LogColor.Green);
    }
}
