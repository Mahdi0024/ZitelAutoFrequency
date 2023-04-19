using ZitelUtilLib.Enums;
using ZitelUtilLib.Requests;

namespace ZitelUtilLib.Requests
{
    internal class LoginRequest : ZitelRequest
    {
        public string Username { get; set; }
        public string Passwd { get; set; }
        public LoginRequest(string username,string password, string sessionId) : base(CommandCodes.LOGIN, "POST", sessionId)
        {
            Username = username;
            Passwd = password;
        }
    }
}