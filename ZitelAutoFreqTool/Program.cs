using Humanizer;
using ZitelAutoFreqTool;
using ZitelUtilApp.Speedtest;
using ZitelUtilLib;

var zitel = new ZitelUtils("http://192.168.0.254/cgi-bin/http.cgi");
var sessionId = zitel.Login("farabord", "farabord");
Console.WriteLine(">>Getting current cell info...");
var cellInfo = zitel.GetCurrentCellInfo(sessionId);


Console.WriteLine($"""
->Current radio status:
-->Cell id: {cellInfo.CellId}
-->Frequency: {cellInfo.Frequency}
-->Locked: {cellInfo.Locked}
------------------------------
""");
var validFrequencies = new[] { 42490, 42690, 42890 };
var speedResults = new List<(int freq, long speed)>();
var speedTest = new SpeedtestWrapper("speedtestOokla.exe");

speedResults.Add((cellInfo.Frequency, TestDownloadSpeed(speedTest)));


foreach (var freq in validFrequencies.Where(freq => freq != cellInfo.Frequency))
{
    Console.WriteLine($"->Setting freq to {freq}, CellId: {cellInfo.CellId}");
    var success = zitel.SetFrequency(freq, cellInfo.CellId, sessionId);
    if (!success)
    {
        Console.WriteLine(">>>>Failed to set frequency!");
        goto end;
    }
    Console.WriteLine("->Frequency set successfully, waiting for internet connection...");
    await Helpers.WaitForInternetConnection();
    speedResults.Add((freq, TestDownloadSpeed(speedTest)));
}

var fastestFreq = speedResults.OrderByDescending(r => r.speed).First().freq;
Console.WriteLine($"->Fastest frequency was {fastestFreq}, configuring radio...");
var freqSetSuccess = zitel.SetFrequency(fastestFreq, cellInfo.CellId, sessionId);
if (!freqSetSuccess)
{
    Console.WriteLine(">>>>Failed to set frequency!");
}
Console.WriteLine("-->Frequency set successfuly. Enjoy!");

end:
Console.WriteLine("\n-->Press any key to exit<--");
Console.ReadKey();



long TestDownloadSpeed(SpeedtestWrapper speedtest)
{
    Console.WriteLine("->Testing downlod speed...");
    while (true)
    {
        try
        {
            var speed = speedTest.GetDownloadSpeedOnly();
            Console.WriteLine($"-->Speed was: {(speed * 8).Bytes().Per(1.Seconds()).Humanize("Mb")}");
            Console.WriteLine("------------------------------");
            return speed;
        }
        catch
        {
            Console.WriteLine("->Speedtest failed! trying again...");
        }
    }

}