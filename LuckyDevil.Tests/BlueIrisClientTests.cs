using FluentAssertions;
using LuckyDevil.Client;
using LuckyDevil.Client.Requests;
using LuckyDevil.Client.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LuckyDevil.Tests;

[TestClass]
public class BlueIrisClientTests
{
    private IServiceProvider serviceProvider;
    private BlueIrisClientConfiguration clientConfig = new BlueIrisClientConfiguration();
    [TestInitialize]
    public void Init()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.local.json", false, true)
            .Build();

        configuration.GetSection(nameof(BlueIrisClientConfiguration)).Bind(clientConfig);
        ArgumentNullException.ThrowIfNull(clientConfig.BaseUrl);
        ArgumentNullException.ThrowIfNull(clientConfig.Username);
        ArgumentNullException.ThrowIfNull(clientConfig.Password);

        var services = new ServiceCollection();
        services.AddHttpClient<BlueIrisClient>();
        serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task GetSession()
    {
        var client = serviceProvider.GetRequiredService<BlueIrisClient>();
        var sessionResponse = await client.GetLoginSession(clientConfig.BaseUrl);

        sessionResponse?.Session.Should().NotBeNullOrWhiteSpace();
        sessionResponse?.Result.Should().Be(LoginResult.Failure);
    }

    [TestMethod]
    public async Task LoginToSession()
    {
        var client = serviceProvider.GetRequiredService<BlueIrisClient>();
        var sessionResponse = await client.GetLoginSession(clientConfig.BaseUrl);
        sessionResponse.Should().NotBeNull();

        var authenticatedSession = await client.AttemptLogin(
            clientConfig.BaseUrl,
            new AttemptLoginRequest(clientConfig.Username, clientConfig.Password, sessionResponse.Session)
            );

        authenticatedSession?.Session.Should().Be(sessionResponse.Session);
        authenticatedSession?.Result.Should().Be(LoginResult.Success);
    }

    // [TestMethod]
    // public async Task GetAlertList()
    // {
    //     var client = serviceProvider.GetRequiredService<BlueIrisClient>();
    //     var session = GetLoggedInSession(client);
    //
    //     var alertListResponse = client.GetAlertList(new AlertListRequest(session, ));
    // }

    [TestMethod]
    public async Task GetCamList()
    {
        var client = serviceProvider.GetRequiredService<BlueIrisClient>();
        var session = await GetLoggedInSession(client);

        var camList = await client.GetCamList(clientConfig.BaseUrl, new CamListRequest(session));
    }

    private async Task<string> GetLoggedInSession(BlueIrisClient client)
    {
        var sessionResponse = await client.GetLoginSession(clientConfig.BaseUrl);
        if (string.IsNullOrEmpty(sessionResponse?.Session))
        {
            throw new Exception("Unable to get a valid authentication session");
        }
        var authenticatedSession = await client.AttemptLogin(
            clientConfig.BaseUrl,
            new AttemptLoginRequest(clientConfig.Username, clientConfig.Password, sessionResponse.Session)
        );

        if (string.IsNullOrEmpty(authenticatedSession?.Session))
        {
            throw new Exception("Unable to successfully log into the supplied authentication session");
        }
        return authenticatedSession.Session;
    }
}