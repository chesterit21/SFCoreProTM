using System;

namespace SFCoreProTM.Domain.Entities.Projects;

public abstract class ModuleScopedEntity : ProjectScopedEntity
{
    protected ModuleScopedEntity()
    {
    }

    protected ModuleScopedEntity(Guid id, Guid workspaceId, Guid projectId, Guid moduleId)
        : base(id, workspaceId, projectId)
    {
        ModuleId = moduleId;
    }

    public Guid ModuleId { get; private set; }
}