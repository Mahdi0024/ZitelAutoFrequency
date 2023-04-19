namespace ZitelUtilApp.Speedtest;

internal class SpeedtestOutput
{
    public string Type { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public SpeedtestPing Ping { get; set; } = default(SpeedtestPing)!;
    public SpeedtestData Download { get; set; } = default(SpeedtestData)!;
    public SpeedtestData Upload { get; set; } = default(SpeedtestData)!;
    public string Error { get; set; } = "";

}

public class SpeedtestData
{
    public long Bandwidth { get; set; }
    public long Bytes { get; set; }
    public int Elapsed { get; set; }
    public SpeedtestLatency Latency { get; set; } = default(SpeedtestLatency)!;
    public double Progress { get; set; }

}

public class SpeedtestPing
{
    public double Jitter { get; set; }
    public double Latency { get; set; }
    public double Progress { get; set; }
}
public class SpeedtestLatency
{
    public double Iqm { get; set; }
}