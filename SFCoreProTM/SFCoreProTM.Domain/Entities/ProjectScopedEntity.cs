using System;

namespace SFCoreProTM.Domain.Entities;

public abstract class ProjectScopedEntity : WorkspaceScopedEntity
{
    protected ProjectScopedEntity()
    {
    }

    protected ProjectScopedEntity(Guid id, Guid workspaceId, Guid projectId) : base(id, workspaceId)
    {
        ProjectId = projectId;
    }

    public Guid ProjectId { get; init; }
}
