using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Velix.SDK;
using Velix.SDK.Models;
using Xunit;

namespace Velix.SDK.Tests;

public class OnboardingModuleTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly VelixClient _client;

    public OnboardingModuleTests()
    {
        _server = WireMockServer.Start();
        _client = new VelixClient(new VelixClientOptions
        {
            ApiUrl = _server.Url!,
            ApiKey = "test-key",
        });
    }

    [Fact]
    public async Task EnrollAsync_ReturnsEnrolledPerson()
    {
        _server.Given(Request.Create().WithPath("/v1/api/onboarding").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""
                {"data":{"person_id":"p-1","identity_id":"i-1","enrolled":true,"frames_processed":3,"frames_results":[{"frame_index":0,"quality_passed":true,"quality_score":0.9,"liveness_passed":true}],"embedding_id":"e-1","message":"ok"}}
                """));

        var result = await _client.Onboarding.EnrollAsync(new OnboardingRequest
        {
            Name = "Maria",
            Frames = ["frame1==", "frame2==", "frame3=="],
        });

        Assert.Equal("p-1", result.PersonId);
        Assert.True(result.Enrolled);
        Assert.Equal(3, result.FramesProcessed);
        Assert.Single(result.FramesResults!);
    }

    public void Dispose()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
