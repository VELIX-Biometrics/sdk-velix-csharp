using System.Text.Json;

namespace Velix.SDK.Modules;

/// <summary>
/// /v1/contexts/* — Identity Context (Velix.ID). BearerAuth (session JWT).
/// See code/lib/lib-velix-contracts/openapi/public-api.yaml, tag "Identity Context".
/// </summary>
public class ContextModule(VelixClient client)
{
    public Task<JsonElement> CreateAsync(object payload, CancellationToken ct = default) =>
        client.PostAsync<JsonElement>("v1/contexts", payload, ct);

    public Task<JsonElement> GetAsync(string id, CancellationToken ct = default) =>
        client.GetAsync<JsonElement>($"v1/contexts/{id}", ct);

    public Task<JsonElement> ListAsync(CancellationToken ct = default) =>
        client.GetAsync<JsonElement>("v1/contexts", ct);

    public Task<JsonElement> UpdateAsync(string id, object payload, CancellationToken ct = default) =>
        client.PatchAsync<JsonElement>($"v1/contexts/{id}", payload, ct);

    public Task RemoveAsync(string id, CancellationToken ct = default) =>
        client.DeleteAsync($"v1/contexts/{id}", ct);

    /// <summary>Authorization Engine — POST /v1/contexts/{contextId}/authorize.</summary>
    public Task<JsonElement> AuthorizeAsync(string contextId, object payload, CancellationToken ct = default) =>
        client.PostAsync<JsonElement>($"v1/contexts/{contextId}/authorize", payload, ct);

    public Task<JsonElement> ListAuthorizationDecisionsAsync(string contextId, CancellationToken ct = default) =>
        client.GetAsync<JsonElement>($"v1/contexts/{contextId}/authorization-decisions", ct);

    /// <summary>
    /// POST /v1/contexts/{contextId}/link-requests — solicita vínculo cross-tenant.
    /// Nunca cria membership diretamente: retorna 202 (PENDING) aguardando
    /// consentimento via magic link/notificação. A API pública não expõe
    /// approve/reject — isso acontece fora do SDK.
    /// </summary>
    public Task<JsonElement> CreateLinkRequestAsync(string contextId, object payload, CancellationToken ct = default) =>
        client.PostAsync<JsonElement>($"v1/contexts/{contextId}/link-requests", payload, ct);
}

public class ContextMembershipModule(VelixClient client)
{
    public Task<JsonElement> CreateAsync(string contextId, object payload, CancellationToken ct = default) =>
        client.PostAsync<JsonElement>($"v1/contexts/{contextId}/memberships", payload, ct);

    public Task<JsonElement> ListByContextAsync(string contextId, CancellationToken ct = default) =>
        client.GetAsync<JsonElement>($"v1/contexts/{contextId}/memberships", ct);

    public Task<JsonElement> ListByIdentityAsync(string identityId, CancellationToken ct = default) =>
        client.GetAsync<JsonElement>($"v1/identities/{identityId}/memberships", ct);

    /// <summary>status="revoked" é a saída de contexto (definitiva, sem carência, task #834).</summary>
    public Task<JsonElement> UpdateStatusAsync(string membershipId, string status, CancellationToken ct = default) =>
        client.PatchAsync<JsonElement>($"v1/memberships/{membershipId}/status", new { status }, ct);

    public Task<JsonElement> AddRolesAsync(string membershipId, IEnumerable<string> roleIds, CancellationToken ct = default) =>
        client.PostAsync<JsonElement>($"v1/memberships/{membershipId}/roles", new { roleIds }, ct);

    public Task<JsonElement> RemoveRolesAsync(string membershipId, IEnumerable<string> roleIds, CancellationToken ct = default) =>
        client.PostAsync<JsonElement>($"v1/memberships/{membershipId}/roles/remove", new { roleIds }, ct);
}

public class ContextRoleModule(VelixClient client)
{
    public Task<JsonElement> CreateAsync(object payload, CancellationToken ct = default) =>
        client.PostAsync<JsonElement>("v1/context-roles", payload, ct);

    public Task<JsonElement> ListAsync(string contextType, CancellationToken ct = default) =>
        client.GetAsync<JsonElement>($"v1/context-roles?contextType={Uri.EscapeDataString(contextType)}", ct);

    public Task<JsonElement> LinkPermissionsAsync(string roleId, IEnumerable<string> permissionIds, CancellationToken ct = default) =>
        client.PostAsync<JsonElement>($"v1/context-roles/{roleId}/permissions", new { permissionIds }, ct);
}

public class ContextPermissionModule(VelixClient client)
{
    public Task<JsonElement> CreateAsync(object payload, CancellationToken ct = default) =>
        client.PostAsync<JsonElement>("v1/context-permissions", payload, ct);

    public Task<JsonElement> ListAsync(string? category = null, CancellationToken ct = default) =>
        client.GetAsync<JsonElement>(
            category is null ? "v1/context-permissions" : $"v1/context-permissions?category={Uri.EscapeDataString(category)}",
            ct);
}

public class AuthorizationTokenModule(VelixClient client)
{
    /// <summary>POST /v1/authorization-tokens/validate — valida (e opcionalmente consome) um token vat_*.</summary>
    public Task<JsonElement> ValidateAsync(string token, bool consume = false, CancellationToken ct = default) =>
        client.PostAsync<JsonElement>("v1/authorization-tokens/validate", new { token, consume }, ct);
}
