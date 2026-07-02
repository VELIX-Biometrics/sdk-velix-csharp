using Velix.SDK.Models;

namespace Velix.SDK.Modules;

/// <summary>
/// POST /v1/api/checkin/identify — scope checkin:write (Velix.ID).
/// Calls the same CheckinService.identifyFace() used by the public HMAC
/// flow (/v1/public/checkin/{tenantSlug}/identify); this route only swaps
/// the tenant-resolution mechanism (API key instead of tenantSlug in path).
/// </summary>
public class CheckinModule(VelixClient client)
{
    public Task<CheckinIdentifyResponse> IdentifyAsync(
        CheckinIdentifyRequest request,
        CancellationToken ct = default) =>
        client.PostAsync<CheckinIdentifyResponse>("v1/api/checkin/identify", request, ct);
}
