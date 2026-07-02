using System.Text.Json.Serialization;

namespace Velix.SDK.Models;

/// <summary>
/// One liveness sample. Wire field names are camelCase per the spec
/// (action, imageBase64) — NOT snake_case.
/// </summary>
public class LivenessSample
{
    /// <summary>One of: center, move_closer, move_away.</summary>
    [JsonPropertyName("action")]
    public required string Action { get; init; }

    [JsonPropertyName("imageBase64")]
    public required string ImageBase64 { get; init; }
}

/// <summary>
/// Liveness block for checkin identify. `Token` is the nonce obtained from
/// GET /v1/public/checkin/{tenantSlug}/liveness/challenge (public, no API key).
/// </summary>
public class LivenessBlock
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }

    [JsonPropertyName("samples")]
    public required IReadOnlyList<LivenessSample> Samples { get; init; }
}

public class CheckinLocation
{
    [JsonPropertyName("latitude")]
    public double? Latitude { get; init; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; init; }

    [JsonPropertyName("accuracy")]
    public double? Accuracy { get; init; }
}

/// <summary>
/// Body for POST /v1/api/checkin/identify (scope checkin:write).
/// Wire field names are camelCase (imageBase64, topK) per the spec —
/// NOT snake_case, unlike most other /v1/api/* payloads.
/// </summary>
public class CheckinIdentifyRequest
{
    /// <summary>Main base64 frame.</summary>
    [JsonPropertyName("imageBase64")]
    public required string ImageBase64 { get; init; }

    [JsonPropertyName("images")]
    public IReadOnlyList<string>? Images { get; init; }

    [JsonPropertyName("topK")]
    public int? TopK { get; init; }

    [JsonPropertyName("liveness")]
    public LivenessBlock? Liveness { get; init; }

    [JsonPropertyName("location")]
    public CheckinLocation? Location { get; init; }
}

/// <summary>
/// Envelope.data content for POST /v1/api/checkin/identify.
/// The only liveness indicator ever exposed is implicit in `Matched` —
/// a liveness score is NEVER returned by this API. Never surface one
/// client-side either.
/// </summary>
public record CheckinIdentifyResponse(
    [property: JsonPropertyName("matched")] bool Matched,
    [property: JsonPropertyName("person_id")] string? PersonId,
    [property: JsonPropertyName("quality_score")] double? QualityScore,
    [property: JsonPropertyName("message")] string? Message
);
