using System;
using SFCoreProTM.Domain.Entities;

namespace SFCoreProTM.Domain.Entities.Projects;

public enum FlowStatus
{
    InProgress,
    Done,
    Canceled,
    Pause
}

public sealed class FlowTask : Entity
{
    private FlowTask()
    {
    }

    private FlowTask(
        Guid id,
        Guid workspaceId,
        Guid projectId,
        Guid moduleId,
        Guid taskId,
        string name,
        string description,
        int sortOrder,
        FlowStatus flowStatus)
    {
        Id = id;
        WorkspaceId = workspaceId;
        ProjectId = projectId;
        ModuleId = moduleId;
        TaskId = taskId;
        Name = name;
        Description = description;
        SortOrder = sortOrder;
        FlowStatus = flowStatus;
    }

    public Guid WorkspaceId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid ModuleId { get; private set; }
    public Guid TaskId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public FlowStatus FlowStatus { get; private set; }

    public static FlowTask Create(
        Guid id,
        Guid workspaceId,
        Guid projectId,
        Guid moduleId,
        Guid taskId,
        string name,
        string description,
        int sortOrder,
        FlowStatus flowStatus)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new FlowTask(
            id,
            workspaceId,
            projectId,
            moduleId,
            taskId,
            name,
            description ?? string.Empty,
            sortOrder,
            flowStatus);
    }

    public void UpdateDetails(
        string name,
        string description,
        int sortOrder,
        FlowStatus flowStatus)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }

        Description = description ?? string.Empty;
        SortOrder = sortOrder;
        FlowStatus = flowStatus;
    }

    public void UpdateStatus(FlowStatus status)
    {
        FlowStatus = status;
    }
}