using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using LuckyDevil.Client.Commands;
using LuckyDevil.Client.Requests;
using LuckyDevil.Client.Responses;

namespace LuckyDevil.Client;

public class BlueIrisClient
{
    private readonly HttpClient client;
    public BlueIrisClient(HttpClient client)
    {
        this.client = client;
    }

    public async Task<LoginSessionResponse?> GetLoginSession(string baseUrl)
    {
        var httpRequest = await GetJsonRequest(baseUrl, new LoginCommand());
        var httpResponse = await client.SendAsync(httpRequest);
        var response = await httpResponse.Content.ReadFromJsonAsync<LoginSessionResponse>();
        return response;
    }

    public async Task<LoginSessionResponse?> AttemptLogin(string baseUrl, AttemptLoginRequest request)
    {
        var hash = MD5.HashData(Encoding.ASCII.GetBytes($"{request.Username}:{request.Session}:{request.Password}"));
        var httpRequest = await GetJsonRequest(baseUrl, new LoginCommand(request.Session, Convert.ToHexString(hash)));
        var httpResponse = await client.SendAsync(httpRequest);
        var response = await httpResponse.Content.ReadFromJsonAsync<LoginSessionResponse>();
        return response;
    }

    public async Task<string> GetCamList(string baseUrl, CamListRequest request)
    {
        var reset = request.Reset ?? CamListResetOptions.None;
        BaseCamListCommand command = reset switch
        {
            CamListResetOptions.None => new CamListCommandFullReset(request.Session, false),
            _ => new CamListCommandPartialOrFullReset(request.Session, (int)reset),
        };
        var httpRequest = await GetJsonRequest(baseUrl, command);
        var httpResponse = await client.SendAsync(httpRequest);
        var response = await httpResponse.Content.ReadAsStringAsync();
        return response;
    }

    private async Task<HttpRequestMessage> GetJsonRequest<T>(string baseUrl, T payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}/json");
        request.Content = JsonContent.Create(payload);
        await request.Content.LoadIntoBufferAsync();
        return request;
    }
}

public record CamListCommandFullReset(string Session, bool Reset) : BaseCamListCommand(Session);

public record CamListCommandPartialOrFullReset(string Session, int Reset) : BaseCamListCommand(Session);

public record BaseCamListCommand(string Session);
