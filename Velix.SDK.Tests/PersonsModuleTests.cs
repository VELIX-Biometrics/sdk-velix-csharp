using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Velix.SDK;
using Xunit;

namespace Velix.SDK.Tests;

public class PersonsModuleTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly VelixClient _client;

    public PersonsModuleTests()
    {
        _server = WireMockServer.Start();
        _client = new VelixClient(new VelixClientOptions
        {
            ApiUrl = _server.Url!,
            ApiKey = "test-key",
        });
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedPerson()
    {
        _server.Given(
            Request.Create().WithPath("/v1/persons").UsingPost()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"id":"p-1","name":"Maria","email":"maria@acme.com","document":null,"department":null,"biometric_enrolled":false,"created_at":"2026-01-01T00:00:00Z","updated_at":"2026-01-01T00:00:00Z"}}""")
        );

        var person = await _client.Persons.CreateAsync(new("Maria", Email: "maria@acme.com"));

        Assert.Equal("p-1", person.Id);
        Assert.Equal("Maria", person.Name);
        Assert.False(person.BiometricEnrolled);
    }

    [Fact]
    public async Task DeleteAsync_Succeeds()
    {
        _server.Given(
            Request.Create().WithPath("/v1/persons/p-1").UsingDelete()
        ).RespondWith(
            Response.Create().WithStatusCode(HttpStatusCode.NoContent)
        );

        await _client.Persons.DeleteAsync("p-1");
    }

    public void Dispose()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
