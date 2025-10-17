using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Projects;

public enum IntakeSourceType
{
    InApp,
}

public enum IntakeIssueStatus
{
    Pending = -2,
    Rejected = -1,
    Snoozed = 0,
    Accepted = 1,
    Duplicate = 2,
}

public sealed class Intake : ProjectScopedEntity
{
    private Intake()
    {
    }

    private Intake(Guid id, Guid workspaceId, Guid projectId, string name, bool isDefault, StructuredData viewProps, StructuredData logoProps)
        : base(id, workspaceId, projectId)
    {
        Name = name;
        IsDefault = isDefault;
        ViewProperties = viewProps;
        LogoProperties = logoProps;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsDefault { get; private set; }

    public StructuredData ViewProperties { get; private set; } = StructuredData.FromJson(null);

    public StructuredData LogoProperties { get; private set; } = StructuredData.FromJson(null);

    public static Intake Create(Guid id, Guid workspaceId, Guid projectId, string name, bool isDefault, StructuredData viewProps, StructuredData logoProps)
    {
        return new Intake(id, workspaceId, projectId, name, isDefault, viewProps, logoProps);
    }

    public void UpdateDetails(string name, string? description, bool isDefault, StructuredData viewProps, StructuredData logoProps)
    {
        Name = name;
        Description = description;
        IsDefault = isDefault;
        ViewProperties = viewProps;
        LogoProperties = logoProps;
    }
}

public sealed class IntakeIssue : ProjectScopedEntity
{
    private IntakeIssue()
    {
    }

    private IntakeIssue(Guid id, Guid workspaceId, Guid projectId, Guid intakeId, Guid issueId, IntakeIssueStatus status)
        : base(id, workspaceId, projectId)
    {
        IntakeId = intakeId;
        IssueId = issueId;
        Status = status;
    }

    public Guid IntakeId { get; private set; }

    public Guid IssueId { get; private set; }

    public IntakeIssueStatus Status { get; private set; }

    public DateTime? SnoozedTill { get; private set; }

    public Guid? DuplicateToIssueId { get; private set; }

    public IntakeSourceType Source { get; private set; } = IntakeSourceType.InApp;

    public string? SourceEmail { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public StructuredData Extra { get; private set; } = StructuredData.FromJson(null);

    public static IntakeIssue Create(Guid id, Guid workspaceId, Guid projectId, Guid intakeId, Guid issueId, IntakeIssueStatus status)
    {
        return new IntakeIssue(id, workspaceId, projectId, intakeId, issueId, status);
    }

    public void UpdateStatus(IntakeIssueStatus status, DateTime? snoozedTill, Guid? duplicateToIssueId)
    {
        Status = status;
        SnoozedTill = snoozedTill;
        DuplicateToIssueId = duplicateToIssueId;
    }

    public void UpdateSource(IntakeSourceType source, string? sourceEmail)
    {
        Source = source;
        SourceEmail = sourceEmail;
    }

    public void SetExternalReference(string? externalSource, string? externalId)
    {
        ExternalReference = externalSource is null && externalId is null ? null : ExternalReference.Create(externalSource, externalId);
    }

    public void UpdateExtra(StructuredData extra)
    {
        Extra = extra;
    }
}
