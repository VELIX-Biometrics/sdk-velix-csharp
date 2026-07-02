using System.Text.Json.Serialization;

namespace Velix.SDK.Models;

/// <summary>
/// Body for POST /v1/api/deletion-request (scope lgpd:write). The person
/// must have an active Identity linked to the API key's tenant, otherwise
/// the server returns 403.
/// </summary>
public class DeletionRequestBody
{
    [JsonPropertyName("person_id")]
    public required string PersonId { get; init; }
}

public record DeletionRequestResponse(
    [property: JsonPropertyName("protocol_number")] string? ProtocolNumber,
    [property: JsonPropertyName("message")] string? Message
);
