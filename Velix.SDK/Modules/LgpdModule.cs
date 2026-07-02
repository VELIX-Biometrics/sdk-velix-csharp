using Velix.SDK.Models;

namespace Velix.SDK.Modules;

/// <summary>
/// POST /v1/api/deletion-request — scope lgpd:write (Velix.ID).
/// Server-to-server equivalent of POST /v1/me/deletion-request (personal
/// portal, JWT), but receiving an explicit person_id instead of resolving
/// identity via JWT.
/// </summary>
public class LgpdModule(VelixClient client)
{
    public Task<DeletionRequestResponse> RequestDeletionAsync(
        string personId,
        CancellationToken ct = default) =>
        client.PostAsync<DeletionRequestResponse>(
            "v1/api/deletion-request",
            new DeletionRequestBody { PersonId = personId },
            ct);
}
