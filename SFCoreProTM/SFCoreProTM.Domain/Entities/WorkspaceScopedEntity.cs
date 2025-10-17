using System;

namespace SFCoreProTM.Domain.Entities;

public abstract class WorkspaceScopedEntity : Entity
{
    protected WorkspaceScopedEntity()
    {
    }

    protected WorkspaceScopedEntity(Guid id, Guid workspaceId) : base(id)
    {
        WorkspaceId = workspaceId;
    }

    public Guid WorkspaceId { get; init; }
}
