using System;
using SFCoreProTM.Domain.Entities;

namespace SFCoreProTM.Domain.Entities.Projects;

public enum ModuleStatus
{
    Backlog,
    Planned,
    InProgress,
    Paused,
    Completed,
    Cancelled,
}

public sealed class Module : ProjectScopedEntity
{
    private Module()
    {
    }

    private Module(Guid id, Guid workspaceId, Guid projectId, string name, string description, int sortOrder, ModuleStatus status)
        : base(id, workspaceId, projectId)
    {
        Name = name;
        Description = description;
        SortOrder = sortOrder;
        Status = status;
    }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public ModuleStatus Status { get; private set; }

    public static Module Create(Guid id, Guid workspaceId, Guid projectId, string name, string description, int sortOrder = 1)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        return new Module(id, workspaceId, projectId, name, description ?? string.Empty, sortOrder, ModuleStatus.Planned);
    }

    public void UpdateDetails(string name, string description, int sortOrder = 1)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        
        Description = description ?? string.Empty;
        SortOrder = sortOrder;
    }
}
