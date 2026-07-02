namespace Velix.SDK.Models;

public record VelixEvent(
    string Id,
    string Name,
    string? Description,
    string Status,
    DateTime? StartDate,
    DateTime? EndDate,
    int GuestCount,
    DateTime CreatedAt
);

public record CreateEventRequest(
    string Name,
    string? Description = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
);

public record PagedResult<T>(
    IReadOnlyList<T> Data,
    int Total,
    int Page,
    int PageSize
);
