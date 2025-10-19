using System;
using SFCoreProTM.Domain.Entities;

namespace SFCoreProTM.Domain.Entities.Projects;

public enum TaskStatus
{
    Backlog,
    Planned,
    InProgress,
    Paused,
    Completed,
    Cancelled,
}

public sealed class Task : ModuleScopedEntity
{
    private Task()
    {
    }

    private Task(Guid id, Guid workspaceId, Guid projectId, Guid moduleId, string name, string description, int sortOrder, TaskStatus status, bool isErd)
        : base(id, workspaceId, projectId, moduleId)
    {
        Name = name;
        Description = description;
        SortOrder = sortOrder;
        Status = status;
        IsErd = isErd;
    }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public TaskStatus Status { get; private set; }
    
    public bool IsErd { get; private set; }

    public static Task Create(Guid id, Guid workspaceId, Guid projectId, Guid moduleId, string name, string description, int sortOrder = 1, bool isErd = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        return new Task(id, workspaceId, projectId, moduleId, name, description ?? string.Empty, sortOrder, TaskStatus.Planned, isErd);
    }

    public void UpdateDetails(string name, string description, int sortOrder = 1, bool isErd = false)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        
        Description = description ?? string.Empty;
        SortOrder = sortOrder;
        IsErd = isErd;
    }
    
    public void UpdateStatus(TaskStatus status)
    {
        Status = status;
    }
}