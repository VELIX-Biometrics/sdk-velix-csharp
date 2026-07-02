using System.Text.Json.Serialization;

namespace Velix.SDK.Models;

/// <summary>
/// Body for POST /v1/api/onboarding (scope onboarding:write).
/// Wire casing is snake_case per lib-velix-contracts/openapi/public-api.yaml.
/// </summary>
public class OnboardingRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    [JsonPropertyName("document")]
    public string? Document { get; init; }

    /// <summary>One of: CPF, CNPJ, RG, PASSPORT, OTHER.</summary>
    [JsonPropertyName("document_type")]
    public string? DocumentType { get; init; }

    /// <summary>If provided, upserts by external key.</summary>
    [JsonPropertyName("external_id")]
    public string? ExternalId { get; init; }

    [JsonPropertyName("metadata")]
    public IDictionary<string, object>? Metadata { get; init; }

    /// <summary>Base64 JPEG frames, no data-URI prefix. Minimum defined by tenant.settings.enroll_frames.</summary>
    [JsonPropertyName("frames")]
    public required IReadOnlyList<string> Frames { get; init; }

    /// <summary>One of: member, admin, tenant_admin. Defaults to member server-side.</summary>
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("access_groups")]
    public IReadOnlyList<string>? AccessGroups { get; init; }
}

public record OnboardingFrameResult(
    [property: JsonPropertyName("frame_index")] int FrameIndex,
    [property: JsonPropertyName("quality_passed")] bool QualityPassed,
    [property: JsonPropertyName("quality_score")] double QualityScore,
    [property: JsonPropertyName("liveness_passed")] bool LivenessPassed
);

/// <summary>
/// Envelope.data content for POST /v1/api/onboarding.
/// </summary>
public record OnboardingResponse(
    [property: JsonPropertyName("person_id")] string? PersonId,
    [property: JsonPropertyName("identity_id")] string? IdentityId,
    [property: JsonPropertyName("enrolled")] bool Enrolled,
    [property: JsonPropertyName("frames_processed")] int FramesProcessed,
    [property: JsonPropertyName("frames_results")] IReadOnlyList<OnboardingFrameResult>? FramesResults,
    [property: JsonPropertyName("embedding_id")] string? EmbeddingId,
    [property: JsonPropertyName("message")] string? Message
);
