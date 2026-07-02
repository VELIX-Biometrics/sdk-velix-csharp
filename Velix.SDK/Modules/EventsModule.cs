using Velix.SDK.Models;

namespace Velix.SDK.Modules;

/// <summary>
/// Velix Events guest endpoints under /v1/api/events/{id}/guests. This is
/// the minimal API-key surface for Events — create + read a single guest.
/// There is no list/update/delete of events or guests in this API.
/// </summary>
public class EventsModule(VelixClient client)
{
    /// <summary>POST /v1/api/events/{id}/guests — scope events:write.</summary>
    public Task<GuestResponse> CreateGuestAsync(
        string eventId,
        CreateGuestRequest request,
        CancellationToken ct = default) =>
        client.PostAsync<GuestResponse>($"v1/api/events/{eventId}/guests", request, ct);

    /// <summary>GET /v1/api/events/{id}/guests/{guestId} — scope events:read.</summary>
    public Task<GuestResponse> GetGuestAsync(
        string eventId,
        string guestId,
        CancellationToken ct = default) =>
        client.GetAsync<GuestResponse>($"v1/api/events/{eventId}/guests/{guestId}", ct);
}
