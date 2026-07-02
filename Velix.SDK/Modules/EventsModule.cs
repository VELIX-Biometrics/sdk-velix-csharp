using Velix.SDK.Models;

namespace Velix.SDK.Modules;

public class EventsModule(VelixClient client)
{
    public Task<PagedResult<VelixEvent>> ListAsync(
        int page = 1, int pageSize = 20,
        CancellationToken ct = default) =>
        client.GetAsync<PagedResult<VelixEvent>>($"v1/events?page={page}&pageSize={pageSize}", ct);

    public Task<VelixEvent> GetAsync(string id, CancellationToken ct = default) =>
        client.GetAsync<VelixEvent>($"v1/events/{id}", ct);

    public Task<VelixEvent> CreateAsync(CreateEventRequest request, CancellationToken ct = default) =>
        client.PostAsync<VelixEvent>("v1/events", request, ct);

    public Task<VelixEvent> UpdateConfigAsync(string id, object config, CancellationToken ct = default) =>
        client.PatchAsync<VelixEvent>($"v1/events/{id}/config", config, ct);
}
