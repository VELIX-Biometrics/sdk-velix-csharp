using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Velix.SDK;
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
    public async Task ListAsync_ReturnsPaged()
    {
        _server.Given(Request.Create().WithPath("/v1/events").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"items":[{"id":"evt-1","name":"Tech Summit","status":"active"}],"total":1,"page":1,"limit":20}}"""));

        var result = await _client.Events.ListAsync(page: 1, limit: 20);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Equal("evt-1", result.Items[0].Id);
    }

    [Fact]
    public async Task GetAsync_ReturnsEvent()
    {
        _server.Given(Request.Create().WithPath("/v1/events/evt-1").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"id":"evt-1","name":"Tech Summit","status":"active"}}"""));

        var ev = await _client.Events.GetAsync("evt-1");

        Assert.Equal("evt-1", ev.Id);
        Assert.Equal("Tech Summit", ev.Name);
    }

    [Fact]
    public async Task GetAsync_ThrowsOnNotFound()
    {
        _server.Given(Request.Create().WithPath("/v1/events/bad").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NotFound)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"message":"Not found","code":"NOT_FOUND"}"""));

        await Assert.ThrowsAsync<VelixException>(() => _client.Events.GetAsync("bad"));
    }

    [Fact]
    public async Task CreateAsync_ReturnsNewEvent()
    {
        _server.Given(Request.Create().WithPath("/v1/events").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"id":"evt-new","name":"New Event","status":"draft"}}"""));

        var ev = await _client.Events.CreateAsync(new CreateEventRequest { Name = "New Event" });

        Assert.Equal("evt-new", ev.Id);
        Assert.Equal("draft", ev.Status);
    }

    public void Dispose()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
