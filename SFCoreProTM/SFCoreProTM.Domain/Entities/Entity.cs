using System;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities;

public abstract class Entity : SFCoreProTM.Domain.IEntity
{
    protected Entity()
    {
    }

    protected Entity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; protected set; }

    public AuditTrail AuditTrail { get; private set; } = AuditTrail.Empty;

    public void SetAuditTrail(AuditTrail auditTrail)
    {
        AuditTrail = auditTrail ?? throw new ArgumentNullException(nameof(auditTrail));
    }
}
