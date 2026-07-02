using Velix.SDK.Models;

namespace Velix.SDK.Modules;

public class PersonsModule(VelixClient client)
{
    public Task<PagedResult<Person>> ListAsync(
        int page = 1, int pageSize = 20,
        string? search = null,
        CancellationToken ct = default)
    {
        var qs = $"v1/persons?page={page}&pageSize={pageSize}";
        if (search != null) qs += $"&search={Uri.EscapeDataString(search)}";
        return client.GetAsync<PagedResult<Person>>(qs, ct);
    }

    public Task<Person> GetAsync(string id, CancellationToken ct = default) =>
        client.GetAsync<Person>($"v1/persons/{id}", ct);

    public Task<Person> CreateAsync(CreatePersonRequest request, CancellationToken ct = default) =>
        client.PostAsync<Person>("v1/persons", request, ct);

    public Task<Person> UpdateAsync(string id, UpdatePersonRequest request, CancellationToken ct = default) =>
        client.PutAsync<Person>($"v1/persons/{id}", request, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default) =>
        client.DeleteAsync($"v1/persons/{id}", ct);

    public Task<Person> EnrollAsync(string id, string frameBase64, CancellationToken ct = default) =>
        client.PostAsync<Person>($"v1/persons/{id}/enroll", new EnrollRequest(frameBase64), ct);
}
