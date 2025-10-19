using System;
using SFCoreProTM.Domain.Entities;

namespace SFCoreProTM.Domain.Entities.Projects;

public enum SprintStatus
{
    InProgress,
    Done,
    Canceled,
    Pause
}

public sealed class SprintPlanning : Entity
{
    private SprintPlanning()
    {
    }

    private SprintPlanning(
        Guid id,
        Guid workspaceId,
        Guid projectId,
        Guid moduleId,
        Guid taskId,
        string name,
        string description,
        DateTime startDate,
        DateTime targetDate,
        int sortOrder,
        SprintStatus sprintStatus,
        string note)
    {
        Id = id;
        WorkspaceId = workspaceId;
        ProjectId = projectId;
        ModuleId = moduleId;
        TaskId = taskId;
        Name = name;
        Description = description;
        StartDate = startDate;
        TargetDate = targetDate;
        SortOrder = sortOrder;
        SprintStatus = sprintStatus;
        Note = note;
    }

    public Guid WorkspaceId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid ModuleId { get; private set; }
    public Guid TaskId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime TargetDate { get; private set; }
    public int SortOrder { get; private set; }
    public SprintStatus SprintStatus { get; private set; }
    public string Note { get; private set; } = string.Empty;

    public static SprintPlanning Create(
        Guid id,
        Guid workspaceId,
        Guid projectId,
        Guid moduleId,
        Guid taskId,
        string name,
        string description,
        DateTime startDate,
        DateTime targetDate,
        int sortOrder,
        SprintStatus sprintStatus,
        string note)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new SprintPlanning(
            id,
            workspaceId,
            projectId,
            moduleId,
            taskId,
            name,
            description ?? string.Empty,
            startDate,
            targetDate,
            sortOrder,
            sprintStatus,
            note ?? string.Empty);
    }

    public void UpdateDetails(
        string name,
        string description,
        DateTime startDate,
        DateTime targetDate,
        int sortOrder,
        SprintStatus sprintStatus,
        string note)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }

        Description = description ?? string.Empty;
        StartDate = startDate;
        TargetDate = targetDate;
        SortOrder = sortOrder;
        SprintStatus = sprintStatus;
        Note = note ?? string.Empty;
    }

    public void UpdateStatus(SprintStatus status)
    {
        SprintStatus = status;
    }
}