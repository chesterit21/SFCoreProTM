using System;

namespace SFCoreProTM.Domain.Entities.Users.Events;

public sealed record UserLoggedInDomainEvent(
    Guid UserId,
    DateTime OccurredAt,
    string Medium,
    string? IpAddress,
    string? UserAgent);
