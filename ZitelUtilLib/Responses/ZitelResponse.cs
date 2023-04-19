using ZitelUtilLib.Enums;

namespace ZitelUtilLib.Responses;

internal class ZitelResponse
{
    public bool Success { get; set; }
    public CommandCodes Cmd { get; set; }
    public string Message { get; set; } = "";
    public string Other { get; set; } = "";
    public string SessionId { get; set; } = "";
}