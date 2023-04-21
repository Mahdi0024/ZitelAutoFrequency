using System.Net.NetworkInformation;

namespace ZitelAutoFreqTool;

internal static class Helpers
{
    public static async Task WaitForInternetConnection()
    {
        using var p = new Ping();
        while (true)
        {
            try
            {
                var pingReply = await p.SendPingAsync("4.2.2.4");
                if (pingReply.Status is IPStatus.Success)
                    return;
            }
            catch
            {
                continue;
            }
        }
    }
}