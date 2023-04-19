using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZitelUtilLib.Enums;

namespace ZitelUtilLib.Requests
{
    internal class ZitelRequest
    {
        public int Cmd { get; init; }
        public string Method { get; init; }
        public string SessionId { get; init; }
        public string Language { get; } = "EN";
        public ZitelRequest(CommandCodes commandCode, string method, string sessionId = "")
        {
            Cmd = (int)commandCode;
            Method = method;
            SessionId = sessionId;
        }
    }
}
