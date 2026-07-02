using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Velix.SDK;
using Xunit;

namespace Velix.SDK.Tests;

public class CheckinModuleTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly VelixClient _client;

    public CheckinModuleTests()
    {
        _server = WireMockServer.Start();
        _client = new VelixClient(new VelixClientOptions
        {
            ApiUrl = _server.Url!,
            ApiKey = "test-key",
        });
    }

    [Fact]
    public async Task FacialAsync_Granted_ReturnsPassed()
    {
        _server.Given(
            Request.Create().WithPath("/v1/checkin/acme/identify").UsingPost()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"passed":true,"person_id":"uuid-123","person_name":"João","request_id":"req-1"}}""")
        );

        var result = await _client.Checkin.FacialAsync("acme", "base64frame==");

        Assert.True(result.Passed);
        Assert.Equal("uuid-123", result.PersonId);
    }

    [Fact]
    public async Task FacialAsync_Denied_ReturnsFailed()
    {
        _server.Given(
            Request.Create().WithPath("/v1/checkin/acme/identify").UsingPost()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"passed":false,"person_id":null,"request_id":"req-2"}}""")
        );

        var result = await _client.Checkin.FacialAsync("acme", "base64frame==");

        Assert.False(result.Passed);
        Assert.Null(result.PersonId);
    }

    public void Dispose()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
