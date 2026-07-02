using System.Text.Json.Serialization;

namespace Velix.SDK.Models;

/// <summary>
/// Envelope.data content for GET /v1/api/me/{personId} (scope me:read).
/// </summary>
public record MeResponse(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("phone")] string? Phone,
    [property: JsonPropertyName("photo_url")] string? PhotoUrl,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt
);
