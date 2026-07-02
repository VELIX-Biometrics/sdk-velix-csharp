namespace Velix.SDK.Models;

public record CheckinResult(
    bool Passed,
    string? PersonId,
    string? PersonName,
    string? AgePolicy,
    string RequestId
);
