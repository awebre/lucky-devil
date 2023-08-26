using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
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

    public async Task<LoginSessionResponse?> AttemptLogin(string baseUrl, string session, string username, string password)
    {
        var hash = MD5.HashData(Encoding.ASCII.GetBytes($"{username}:{session}:{password}"));
        var httpRequest = await GetJsonRequest(baseUrl, new LoginCommand(session, Convert.ToHexString(hash)));
        var httpResponse = await client.SendAsync(httpRequest);
        var response = await httpResponse.Content.ReadFromJsonAsync<LoginSessionResponse>();
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

