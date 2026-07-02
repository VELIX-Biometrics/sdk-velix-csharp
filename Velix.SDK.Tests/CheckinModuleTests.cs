using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Velix.SDK;
using Velix.SDK.Models;
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
    public async Task IdentifyAsync_Matched_ReturnsPersonId()
    {
        _server.Given(
            Request.Create().WithPath("/v1/api/checkin/identify").UsingPost()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"matched":true,"person_id":"uuid-123","quality_score":0.94,"message":"ok"}}""")
        );

        var result = await _client.Checkin.IdentifyAsync(new CheckinIdentifyRequest
        {
            ImageBase64 = "base64frame==",
        });

        Assert.True(result.Matched);
        Assert.Equal("uuid-123", result.PersonId);
    }

    [Fact]
    public async Task IdentifyAsync_NotMatched_ReturnsNullPerson()
    {
        _server.Given(
            Request.Create().WithPath("/v1/api/checkin/identify").UsingPost()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"matched":false,"person_id":null,"message":"not found"}}""")
        );

        var result = await _client.Checkin.IdentifyAsync(new CheckinIdentifyRequest
        {
            ImageBase64 = "base64frame==",
        });

        Assert.False(result.Matched);
        Assert.Null(result.PersonId);
    }

    [Fact]
    public async Task IdentifyAsync_SendsApiKeyHeader()
    {
        _server.Given(
            Request.Create().WithPath("/v1/api/checkin/identify").UsingPost()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"matched":true,"person_id":"uuid-123"}}""")
        );

        await _client.Checkin.IdentifyAsync(new CheckinIdentifyRequest { ImageBase64 = "x" });

        var logEntry = _server.LogEntries.Last();
        Assert.Equal("test-key", logEntry.RequestMessage.Headers!["x-api-key"].First());
    }

    public void Dispose()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
