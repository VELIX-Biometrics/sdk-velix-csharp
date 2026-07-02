using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Velix.SDK;
using Xunit;

namespace Velix.SDK.Tests;

public class TenantsModuleTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly VelixClient _client;

    public TenantsModuleTests()
    {
        _server = WireMockServer.Start();
        _client = new VelixClient(new VelixClientOptions
        {
            ApiUrl = _server.Url!,
            ApiKey = "test-key",
        });
    }

    [Fact]
    public async Task MeAsync_ReturnsTenant()
    {
        _server.Given(Request.Create().WithPath("/v1/tenants/me").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"id":"tenant-uuid","name":"Acme Corp","slug":"acme","plan":"enterprise","maxPersons":1000}}"""));

        var tenant = await _client.Tenants.MeAsync();

        Assert.Equal("tenant-uuid", tenant.Id);
        Assert.Equal("acme", tenant.Slug);
        Assert.Equal("enterprise", tenant.Plan);
    }

    [Fact]
    public async Task UpdateSettingsAsync_ReturnsUpdated()
    {
        _server.Given(Request.Create().WithPath("/v1/tenants/me/settings").UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"data":{"id":"tenant-uuid","requireLiveness":true,"timezone":"America/Sao_Paulo"}}"""));

        var tenant = await _client.Tenants.UpdateSettingsAsync(new UpdateTenantSettingsRequest
        {
            RequireLiveness = true,
            Timezone = "America/Sao_Paulo",
        });

        Assert.True(tenant.RequireLiveness);
        Assert.Equal("America/Sao_Paulo", tenant.Timezone);
    }

    public void Dispose()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
