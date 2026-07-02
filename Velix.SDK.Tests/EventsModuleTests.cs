using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Velix.SDK;
using Velix.SDK.Models;
using Xunit;

namespace Velix.SDK.Tests;

public class EventsModuleTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly VelixClient _client;

    public EventsModuleTests()
    {
        _server = WireMockServer.Start();
        _client = new VelixClient(new VelixClientOptions
        {
            ApiUrl = _server.Url!,
            ApiKey = "test-key",
        });
    }

    [Fact]
    public async Task CreateGuestAsync_ReturnsCreatedGuest()
    {
        _server.Given(Request.Create().WithPath("/v1/api/events/evt-1/guests").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"id":"guest-1","eventId":"evt-1","name":"Maria","email":"maria@acme.com","status":"invited","categoryId":null}}"""));

        var guest = await _client.Events.CreateGuestAsync("evt-1", new CreateGuestRequest
        {
            Name = "Maria",
            Email = "maria@acme.com",
        });

        Assert.Equal("guest-1", guest.Id);
        Assert.Equal("evt-1", guest.EventId);
        Assert.Equal("invited", guest.Status);
    }

    [Fact]
    public async Task GetGuestAsync_ReturnsGuest()
    {
        _server.Given(Request.Create().WithPath("/v1/api/events/evt-1/guests/guest-1").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"id":"guest-1","eventId":"evt-1","name":"Maria","email":"maria@acme.com","status":"checked_in"}}"""));

        var guest = await _client.Events.GetGuestAsync("evt-1", "guest-1");

        Assert.Equal("guest-1", guest.Id);
        Assert.Equal("checked_in", guest.Status);
    }

    [Fact]
    public async Task GetGuestAsync_ThrowsOnNotFound()
    {
        _server.Given(Request.Create().WithPath("/v1/api/events/evt-1/guests/bad").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NotFound)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"error":{"message":"Not found","code":"NOT_FOUND","statusCode":404}}"""));

        await Assert.ThrowsAsync<VelixException>(() => _client.Events.GetGuestAsync("evt-1", "bad"));
    }

    public void Dispose()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
