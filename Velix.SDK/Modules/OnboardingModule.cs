using Velix.SDK.Models;

namespace Velix.SDK.Modules;

/// <summary>
/// POST /v1/api/onboarding — scope onboarding:write (Velix.ID).
/// </summary>
public class OnboardingModule(VelixClient client)
{
    public Task<OnboardingResponse> EnrollAsync(
        OnboardingRequest request,
        CancellationToken ct = default) =>
        client.PostAsync<OnboardingResponse>("v1/api/onboarding", request, ct);
}
