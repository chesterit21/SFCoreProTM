using System;
using System.Collections.Generic;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class AuditTrail : ValueObject
{
    private AuditTrail()
        : this(DateTime.MinValue, null, null, null, null)
    {
    }

    private AuditTrail(DateTime createdAt, Guid? createdById, DateTime? updatedAt, Guid? updatedById, DateTime? deletedAt)
    {
        CreatedAt = createdAt;
        CreatedById = createdById;
        UpdatedAt = updatedAt;
        UpdatedById = updatedById;
        DeletedAt = deletedAt;
    }

    public DateTime CreatedAt { get; }

    public Guid? CreatedById { get; }

    public DateTime? UpdatedAt { get; }

    public Guid? UpdatedById { get; }

    public DateTime? DeletedAt { get; }

    public static AuditTrail Create(DateTime createdAt, Guid? createdById, DateTime? updatedAt, Guid? updatedById, DateTime? deletedAt)
    {
        return new AuditTrail(createdAt, createdById, updatedAt, updatedById, deletedAt);
    }

    public static AuditTrail Empty => new(DateTime.MinValue, null, null, null, null);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CreatedAt;
        yield return CreatedById;
        yield return UpdatedAt;
        yield return UpdatedById;
        yield return DeletedAt;
    }
}
