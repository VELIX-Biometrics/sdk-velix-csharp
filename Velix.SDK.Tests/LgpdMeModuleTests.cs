using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Velix.SDK;
using Xunit;

namespace Velix.SDK.Tests;

public class LgpdMeModuleTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly VelixClient _client;

    public LgpdMeModuleTests()
    {
        _server = WireMockServer.Start();
        _client = new VelixClient(new VelixClientOptions
        {
            ApiUrl = _server.Url!,
            ApiKey = "test-key",
        });
    }

    [Fact]
    public async Task RequestDeletionAsync_ReturnsProtocolNumber()
    {
        _server.Given(Request.Create().WithPath("/v1/api/deletion-request").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"protocol_number":"DEL-2026-001","message":"Solicitação registrada"}}"""));

        var result = await _client.Lgpd.RequestDeletionAsync("p-1");

        Assert.Equal("DEL-2026-001", result.ProtocolNumber);
    }

    [Fact]
    public async Task GetAsync_ReturnsMeResponse()
    {
        _server.Given(Request.Create().WithPath("/v1/api/me/p-1").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"id":"p-1","name":"Maria","email":"maria@acme.com","phone":null,"photo_url":null,"created_at":"2026-01-01T00:00:00Z"}}"""));

        var me = await _client.Me.GetAsync("p-1");

        Assert.Equal("p-1", me.Id);
        Assert.Equal("Maria", me.Name);
    }

    public void Dispose()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
