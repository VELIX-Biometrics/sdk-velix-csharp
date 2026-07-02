namespace Velix.SDK.Modules;

public record TenantSettings(
    string? WebhookUrl = null,
    bool? RequireLiveness = null,
    string? BiometricQualityLevel = null,
    int? GeofenceRadiusMetros = null,
    string? Timezone = null
);

public record Tenant(
    string Id,
    string Name,
    string Slug,
    string Plan,
    int MaxPersons,
    bool RequireLiveness,
    string Timezone
);

public class TenantsModule(VelixClient client)
{
    public Task<Tenant> GetMeAsync(CancellationToken ct = default) =>
        client.GetAsync<Tenant>("v1/tenants/me", ct);

    public Task<Tenant> UpdateSettingsAsync(TenantSettings settings, CancellationToken ct = default) =>
        client.PutAsync<Tenant>("v1/tenants/me/settings", settings, ct);
}
