using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Projects;

public sealed class Cycle : ProjectScopedEntity
{
    private Cycle()
    {
    }

    private Cycle(Guid id, Guid workspaceId, Guid projectId, string name, DateRange schedule, double sortOrder)
        : base(id, workspaceId, projectId)
    {
        Name = name;
        Schedule = schedule;
        SortOrder = sortOrder;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateRange Schedule { get; private set; } = DateRange.Create(null, null);

    public Guid? OwnedById { get; private set; }

    public StructuredData ViewProperties { get; private set; } = StructuredData.FromJson(null);

    public double SortOrder { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public StructuredData ProgressSnapshot { get; private set; } = StructuredData.FromJson(null);

    public DateTime? ArchivedAt { get; private set; }

    public StructuredData LogoProperties { get; private set; } = StructuredData.FromJson(null);

    public string? Timezone { get; private set; }

    public string Version { get; private set; } = "v1";

    public static Cycle Create(Guid id, Guid workspaceId, Guid projectId, string name, DateRange schedule, double sortOrder)
    {
        return new Cycle(id, workspaceId, projectId, name, schedule, sortOrder);
    }

    public void UpdateDetails(string? description, Guid? ownedById, DateRange schedule, double sortOrder, StructuredData viewProps)
    {
        Description = description;
        OwnedById = ownedById;
        Schedule = schedule;
        SortOrder = sortOrder;
        ViewProperties = viewProps;
    }

    public void UpdateProgress(StructuredData progressSnapshot, StructuredData logoProps, string? timezone, string version)
    {
        ProgressSnapshot = progressSnapshot;
        LogoProperties = logoProps;
        Timezone = timezone;
        Version = version;
    }

    public void SetExternalReference(string? source, string? identifier)
    {
        ExternalReference = source is null && identifier is null ? null : ExternalReference.Create(source, identifier);
    }

    public void Archive(DateTime? archivedAt)
    {
        ArchivedAt = archivedAt;
    }
}

public sealed class CycleIssue : ProjectScopedEntity
{
    private CycleIssue()
    {
    }

    private CycleIssue(Guid id, Guid workspaceId, Guid projectId, Guid cycleId, Guid issueId)
        : base(id, workspaceId, projectId)
    {
        CycleId = cycleId;
        IssueId = issueId;
    }

    public Guid CycleId { get; private set; }

    public Guid IssueId { get; private set; }

    public static CycleIssue Create(Guid id, Guid workspaceId, Guid projectId, Guid cycleId, Guid issueId)
    {
        return new CycleIssue(id, workspaceId, projectId, cycleId, issueId);
    }
}

public sealed class CycleUserProperties : ProjectScopedEntity
{
    private CycleUserProperties()
    {
    }

    private CycleUserProperties(Guid id, Guid workspaceId, Guid projectId, Guid cycleId, Guid userId, ViewPreferences preferences)
        : base(id, workspaceId, projectId)
    {
        CycleId = cycleId;
        UserId = userId;
        Preferences = preferences;
    }

    public Guid CycleId { get; private set; }

    public Guid UserId { get; private set; }

    public ViewPreferences Preferences { get; private set; } = ViewPreferences.CreateCycleDefaults();

    public static CycleUserProperties Create(Guid id, Guid workspaceId, Guid projectId, Guid cycleId, Guid userId, ViewPreferences preferences)
    {
        var effectivePreferences = preferences ?? ViewPreferences.CreateCycleDefaults();
        return new CycleUserProperties(id, workspaceId, projectId, cycleId, userId, effectivePreferences);
    }

    public void Update(ViewPreferences preferences)
    {
        Preferences = preferences ?? ViewPreferences.CreateCycleDefaults();
    }
}
