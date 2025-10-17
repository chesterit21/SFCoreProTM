using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Drafts;

public sealed class DraftIssue : WorkspaceScopedEntity
{
    private readonly HashSet<Guid> _assigneeIds = new();
    private readonly HashSet<Guid> _labelIds = new();

    private DraftIssue()
    {
    }

    private DraftIssue(Guid id, Guid workspaceId, string name, IssuePriority priority, DateRange schedule, double sortOrder, RichTextContent description)
        : base(id, workspaceId)
    {
        Name = name;
        Priority = priority;
        Schedule = schedule;
        SortOrder = sortOrder;
        Description = description;
    }

    public Guid? ParentId { get; private set; }

    public Guid? StateId { get; private set; }

    public Guid? EstimatePointId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public RichTextContent Description { get; private set; } = RichTextContent.Create();

    public IssuePriority Priority { get; private set; }

    public DateRange Schedule { get; private set; } = DateRange.Create(null, null);

    public IReadOnlyCollection<Guid> AssigneeIds => _assigneeIds;

    public IReadOnlyCollection<Guid> LabelIds => _labelIds;

    public double SortOrder { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public Guid? IssueTypeId { get; private set; }

    public static DraftIssue Create(Guid id, Guid workspaceId, string name, IssuePriority priority, DateRange schedule, double sortOrder, RichTextContent description)
    {
        return new DraftIssue(id, workspaceId, name, priority, schedule, sortOrder, description);
    }

    public void SetHierarchy(Guid? parentId)
    {
        ParentId = parentId;
    }

    public void ConfigureState(Guid? stateId, Guid? estimatePointId, Guid? issueTypeId)
    {
        StateId = stateId;
        EstimatePointId = estimatePointId;
        IssueTypeId = issueTypeId;
    }

    public void UpdateContent(string name, RichTextContent description, IssuePriority priority, DateRange schedule, double sortOrder)
    {
        Name = name;
        Description = description;
        Priority = priority;
        Schedule = schedule;
        SortOrder = sortOrder;
    }

    public void SetCompletion(DateTime? completedAt)
    {
        CompletedAt = completedAt;
    }

    public void SetExternalReference(string? source, string? externalId)
    {
        ExternalReference = source is null && externalId is null ? null : ExternalReference.Create(source, externalId);
    }

    public void AddAssignee(Guid userId)
    {
        _assigneeIds.Add(userId);
    }

    public void RemoveAssignee(Guid userId)
    {
        _assigneeIds.Remove(userId);
    }

    public void AddLabel(Guid labelId)
    {
        _labelIds.Add(labelId);
    }

    public void RemoveLabel(Guid labelId)
    {
        _labelIds.Remove(labelId);
    }
}

public sealed class DraftIssueAssignee : WorkspaceScopedEntity
{
    private DraftIssueAssignee()
    {
    }

    private DraftIssueAssignee(Guid id, Guid workspaceId, Guid draftIssueId, Guid assigneeId)
        : base(id, workspaceId)
    {
        DraftIssueId = draftIssueId;
        AssigneeId = assigneeId;
    }

    public Guid DraftIssueId { get; private set; }

    public Guid AssigneeId { get; private set; }

    public static DraftIssueAssignee Create(Guid id, Guid workspaceId, Guid draftIssueId, Guid assigneeId)
    {
        return new DraftIssueAssignee(id, workspaceId, draftIssueId, assigneeId);
    }
}

public sealed class DraftIssueLabel : WorkspaceScopedEntity
{
    private DraftIssueLabel()
    {
    }

    private DraftIssueLabel(Guid id, Guid workspaceId, Guid draftIssueId, Guid labelId)
        : base(id, workspaceId)
    {
        DraftIssueId = draftIssueId;
        LabelId = labelId;
    }

    public Guid DraftIssueId { get; private set; }

    public Guid LabelId { get; private set; }

    public static DraftIssueLabel Create(Guid id, Guid workspaceId, Guid draftIssueId, Guid labelId)
    {
        return new DraftIssueLabel(id, workspaceId, draftIssueId, labelId);
    }
}

public sealed class DraftIssueModule : WorkspaceScopedEntity
{
    private DraftIssueModule()
    {
    }

    private DraftIssueModule(Guid id, Guid workspaceId, Guid draftIssueId, Guid moduleId)
        : base(id, workspaceId)
    {
        DraftIssueId = draftIssueId;
        ModuleId = moduleId;
    }

    public Guid DraftIssueId { get; private set; }

    public Guid ModuleId { get; private set; }

    public static DraftIssueModule Create(Guid id, Guid workspaceId, Guid draftIssueId, Guid moduleId)
    {
        return new DraftIssueModule(id, workspaceId, draftIssueId, moduleId);
    }
}

public sealed class DraftIssueCycle : WorkspaceScopedEntity
{
    private DraftIssueCycle()
    {
    }

    private DraftIssueCycle(Guid id, Guid workspaceId, Guid draftIssueId, Guid cycleId)
        : base(id, workspaceId)
    {
        DraftIssueId = draftIssueId;
        CycleId = cycleId;
    }

    public Guid DraftIssueId { get; private set; }

    public Guid CycleId { get; private set; }

    public static DraftIssueCycle Create(Guid id, Guid workspaceId, Guid draftIssueId, Guid cycleId)
    {
        return new DraftIssueCycle(id, workspaceId, draftIssueId, cycleId);
    }
}
