using Humanizer;
using ZitelUtilApp.Speedtest;
using ZitelUtilLib;

var zitel = new ZitelUtils("http://192.168.0.254/cgi-bin/http.cgi");

Console.WriteLine(">>Logging in...");
var sessionId = zitel.Login("farabord", "farabord");
Console.WriteLine($">>Logged in successfully!");

Console.WriteLine(">>Getting current cell info...");
var cellInfo = zitel.GetCurrentCellInfo(sessionId);
Console.WriteLine($"""
>Cell id: {cellInfo.CellId}
>Frequency: {cellInfo.Frequency}
>Locked: {cellInfo.Locked}
""");
var validFrequencies =new[] { 42490, 42690, 42890 };
var speedResults = new List<(int freq, long speed)>();
var st = new SpeedtestWrapper("speedtestOokla.exe");
foreach (var freq in validFrequencies)
{
    Console.WriteLine($">>Setting freq to {freq}, CellId: {cellInfo.CellId}");
    var success = zitel.SetFrequency(freq, cellInfo.CellId, sessionId);
    if (!success)
    {
        Console.WriteLine(">>>>Failed to set frequency! Exitting...");
        goto end;
    }
    Console.WriteLine(">>Frequency set successfully, waiting for modem...");
    Thread.Sleep(10.Seconds());
    Console.WriteLine(">>Testing downlod speed...");
    var speed = st.GetDownloadSpeedOnly();
    Console.WriteLine($">>Speed was: {(speed * 8).Bytes().Per(1.Seconds()).Humanize("Mb")}");
    speedResults.Add((freq, speed));
}

var fastestFreq = speedResults.OrderByDescending(r => r.speed).First().freq;
Console.WriteLine($">>Fastest freq was {fastestFreq}, setting frequency to {fastestFreq}...");
var freqSetSuccess = zitel.SetFrequency(fastestFreq, cellInfo.CellId, sessionId);
if (freqSetSuccess)
{
    Console.WriteLine(">>Frequency set successfuly. Enjoy!");
}

end:
Console.WriteLine("\n\nPress any key to exit...");
Console.ReadKey();
