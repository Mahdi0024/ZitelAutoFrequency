using System.Diagnostics;
using System.Text.Json;

namespace ZitelUtilApp.Speedtest;

internal class SpeedtestWrapper
{
    private string _speedtestAddress;

    public SpeedtestWrapper(string speedtestAddress)
    {
        _speedtestAddress = speedtestAddress;
    }
    public long GetDownloadSpeedOnly()
    {
        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                Arguments = "--format jsonl --accept-license",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = _speedtestAddress
            }
        };
        process.Start();

        while (true)
        {
            var data = process.StandardOutput.ReadLine();
            if (data == null) continue;
            var dataObj = JsonSerializer.Deserialize<SpeedtestOutput>(data!, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            if (dataObj!.Type != "testStart" && dataObj!.Type != "ping" && dataObj!.Type != "download")
            {
                process.Kill();
                throw new Exception("Speedtest failed!");
            }

            if (dataObj!.Type == "download")
            {
                if (dataObj.Download.Progress >= 1)
                {
                    process.Kill();
                    return dataObj.Download.Bandwidth;
                }
            }
        }

    }
}