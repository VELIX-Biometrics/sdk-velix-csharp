using System.Text.Json.Serialization;

namespace Velix.SDK.Models;

/// <summary>
/// Body for POST /v1/api/events/{id}/guests (scope events:write).
/// Wire field names are camelCase (birthDate, categoryId, companionOf)
/// per the spec — NOT snake_case.
/// </summary>
public class CreateGuestRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("cpf")]
    public string? Cpf { get; init; }

    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    [JsonPropertyName("birthDate")]
    public DateOnly? BirthDate { get; init; }

    [JsonPropertyName("categoryId")]
    public string? CategoryId { get; init; }

    [JsonPropertyName("companionOf")]
    public string? CompanionOf { get; init; }
}

/// <summary>
/// EventGuest — returned by both POST /v1/api/events/{id}/guests and
/// GET /v1/api/events/{id}/guests/{guestId}.
/// </summary>
public record GuestResponse(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("eventId")] string? EventId,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("status")] string? Status,
    [property: JsonPropertyName("categoryId")] string? CategoryId
);
