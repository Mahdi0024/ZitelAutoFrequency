using ZitelUtilLib.Enums;

namespace ZitelUtilLib.Requests;

internal class SetFrequencyRequest : ZitelRequest
{
    public int FreqPoint { get; init; }
    public int PhyCellId { get; init; }
    public int LockedStatus { get; } = 1;

    public SetFrequencyRequest(int freq,int cellId,string sessionId):base(CommandCodes.LOCK_ONE_CELL,"POST",sessionId)
    {
        FreqPoint = freq;
        PhyCellId = cellId;
    }
}