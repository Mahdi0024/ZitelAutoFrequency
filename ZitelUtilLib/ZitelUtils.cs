using RestSharp;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ZitelUtilLib.Enums;
using ZitelUtilLib.Requests;
using ZitelUtilLib.Requestsک;
using ZitelUtilLib.Responses;

namespace ZitelUtilLib;

public class ZitelUtils
{
    private string _address;
    private readonly RestClient _client;
    private readonly JsonSerializerOptions _serializerOptions;

    public ZitelUtils(string address)
    {
        _client = new RestClient();
        _address = address;
        _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }
    private string GenerateSessionId()
    {
        var randomNumber = Random.Shared.Next();
        return GenerateSHA256(randomNumber.ToString());

    }
    private string GetSalt()
    {
        var request = new RestRequest(_address, Method.Post);
        request.AddBody(new ZitelRequest(CommandCodes.CREATE_RANDOM_SALT, "POST"));
        var result = _client.Execute(request);
        var response = JsonSerializer.Deserialize<ZitelResponse>(result.Content!, _serializerOptions);
        return response!.Message;
    }
    private string GenerateSHA256(string value)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash).ToLower();
    }
    public bool SetFrequency(int frequency, int cellId, string sessionId)
    {
        var request = new RestRequest(_address, Method.Post);
        request.AddBody(new SetFrequencyRequest(frequency, cellId, sessionId));
        var result = _client.Execute(request);
        var response = JsonSerializer.Deserialize<ZitelResponse>(result.Content!, _serializerOptions);
        return response!.Success;
    }
    public CellInfo GetCurrentCellInfo(string sessionId)
    {
        var request = new RestRequest(_address, Method.Post);
        request.AddBody(new ZitelRequest(CommandCodes.LOCK_ONE_CELL, "QUERY", sessionId));
        var result = _client.Execute(request);
        var response = JsonSerializer.Deserialize<QueryFrequencyResponse>(result.Content!, _serializerOptions);

        // in this case the LOCK_ONE_CELL command will not return the correct cell id and freuency so
        // i get the needed info with GET_LTE_STATUS command.
        if (response!.LockedStatus == "0") 
        {
            var lteStatus = GetLteStatus(sessionId);
            response!.FreqPoint = lteStatus["EARFCN/ARFCN"];
            response!.PhyCellId = lteStatus["Physical CellID"];
        }
        return new CellInfo
        {
            Frequency = int.Parse(response!.FreqPoint),
            CellId = int.Parse(response!.PhyCellId),
            Locked = response!.LockedStatus == "1"
        };
    }
    private Dictionary<string, string> GetLteStatus(string sessionId)
    {
        var request = new RestRequest(_address, Method.Post);
        request.AddBody(new ZitelRequest(CommandCodes.GET_LTE_STATUS, "POST", sessionId));
        var result = _client.Execute(request);
        var response = JsonSerializer.Deserialize<ZitelResponse>(result.Content!,_serializerOptions);

        var finalResult = new Dictionary<string, string>();
        foreach (var item in response!.Message.Split('$'))
        {
            var parts = item.Split('@');
            var key = parts[0];
            var value = parts[1];
            finalResult.Add(key, value);
        }
        return finalResult;
    }
    public string Login(string username, string password)
    {
        var sessionId = GenerateSessionId();
        var salt = GetSalt();
        var request = new RestRequest(_address, Method.Post);
        var loginRequest = new LoginRequest(username, GenerateSHA256(salt + GenerateMD5(password)), sessionId);
        request.AddBody(loginRequest);
        var result = _client.Execute(request);
        var response = JsonSerializer.Deserialize<ZitelResponse>(result.Content!, _serializerOptions);
        if (response!.Success)
            return response.SessionId;
        throw new AuthenticationException();
    }
    private string GenerateMD5(string value)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash).ToLower();
    }
}
