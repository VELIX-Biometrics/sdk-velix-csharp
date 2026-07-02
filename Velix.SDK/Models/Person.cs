namespace Velix.SDK.Models;

public record Person(
    string Id,
    string Name,
    string? Email,
    string? Document,
    string? Department,
    bool BiometricEnrolled,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreatePersonRequest(
    string Name,
    string? Email = null,
    string? Document = null,
    string? Department = null,
    string? ExternalId = null
);

public record UpdatePersonRequest(
    string? Name = null,
    string? Email = null,
    string? Document = null,
    string? Department = null
);

public record EnrollRequest(string FrameBase64);
