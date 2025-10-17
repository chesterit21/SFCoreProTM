using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Issues;

public sealed class IssueType : Entity
{
    private IssueType()
    {
    }

    private IssueType(Guid id, Guid workspaceId, string name, bool isEpic, bool isDefault, bool isActive, double level, StructuredData logoProps)
        : base(id)
    {
        WorkspaceId = workspaceId;
        Name = name;
        IsEpic = isEpic;
        IsDefault = isDefault;
        IsActive = isActive;
        Level = level;
        LogoProperties = logoProps;
    }

    public Guid WorkspaceId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public StructuredData LogoProperties { get; private set; } = StructuredData.FromJson(null);

    public bool IsEpic { get; private set; }

    public bool IsDefault { get; private set; }

    public bool IsActive { get; private set; } = true;

    public double Level { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public static IssueType Create(Guid id, Guid workspaceId, string name, bool isEpic, bool isDefault, bool isActive, double level, StructuredData logoProps)
    {
        return new IssueType(id, workspaceId, name, isEpic, isDefault, isActive, level, logoProps);
    }

    public void Update(string name, string? description, bool isEpic, bool isDefault, bool isActive, double level, StructuredData logoProps)
    {
        Name = name;
        Description = description;
        IsEpic = isEpic;
        IsDefault = isDefault;
        IsActive = isActive;
        Level = level;
        LogoProperties = logoProps;
    }

    public void SetExternalReference(string? source, string? identifier)
    {
        ExternalReference = source is null && identifier is null ? null : ExternalReference.Create(source, identifier);
    }
}

public sealed class ProjectIssueType : ProjectScopedEntity
{
    private ProjectIssueType()
    {
    }

    private ProjectIssueType(Guid id, Guid workspaceId, Guid projectId, Guid issueTypeId, int level, bool isDefault)
        : base(id, workspaceId, projectId)
    {
        IssueTypeId = issueTypeId;
        Level = level;
        IsDefault = isDefault;
    }

    public Guid IssueTypeId { get; private set; }

    public int Level { get; private set; }

    public bool IsDefault { get; private set; }

    public static ProjectIssueType Create(Guid id, Guid workspaceId, Guid projectId, Guid issueTypeId, int level, bool isDefault)
    {
        return new ProjectIssueType(id, workspaceId, projectId, issueTypeId, level, isDefault);
    }

    public void Update(int level, bool isDefault)
    {
        Level = level;
        IsDefault = isDefault;
    }
}
