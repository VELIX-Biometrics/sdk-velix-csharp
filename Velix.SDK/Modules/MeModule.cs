using Velix.SDK.Models;

namespace Velix.SDK.Modules;

/// <summary>
/// GET /v1/api/me/{personId} — scope me:read (Velix.ID). Server-to-server
/// equivalent of GET /v1/me (personal portal), by explicit personId.
/// </summary>
public class MeModule(VelixClient client)
{
    public Task<MeResponse> GetAsync(string personId, CancellationToken ct = default) =>
        client.GetAsync<MeResponse>($"v1/api/me/{personId}", ct);
}
