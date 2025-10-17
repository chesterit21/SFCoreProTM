using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

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
    private readonly HashSet<Guid> _memberIds = new();

    private Module()
    {
    }

    private Module(Guid id, Guid workspaceId, Guid projectId, string name, ModuleStatus status, DateRange schedule, double sortOrder, RichTextContent description)
        : base(id, workspaceId, projectId)
    {
        Name = name;
        Status = status;
        Schedule = schedule;
        SortOrder = sortOrder;
        Description = description;
    }

    public string Name { get; private set; } = string.Empty;

    public RichTextContent Description { get; private set; } = RichTextContent.Create();

    public DateRange Schedule { get; private set; } = DateRange.Create(null, null);

    public ModuleStatus Status { get; private set; } = ModuleStatus.Planned;

    public Guid? LeadId { get; private set; }

    public IReadOnlyCollection<Guid> MemberIds => _memberIds;

    public StructuredData ViewProperties { get; private set; } = StructuredData.FromJson(null);

    public double SortOrder { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public DateTime? ArchivedAt { get; private set; }

    public StructuredData LogoProperties { get; private set; } = StructuredData.FromJson(null);

    public static Module Create(Guid id, Guid workspaceId, Guid projectId, string name, ModuleStatus status, DateRange schedule, double sortOrder, RichTextContent description)
    {
        return new Module(id, workspaceId, projectId, name, status, schedule, sortOrder, description);
    }

    public void AssignLead(Guid? leadId)
    {
        LeadId = leadId;
    }

    public void UpdateContent(string name, RichTextContent description, ModuleStatus status, DateRange schedule, double sortOrder)
    {
        Name = name;
        Description = description;
        Status = status;
        Schedule = schedule;
        SortOrder = sortOrder;
    }

    public void SetExternalReference(string? source, string? identifier)
    {
        ExternalReference = source is null && identifier is null ? null : ExternalReference.Create(source, identifier);
    }

    public void Archive(DateTime? archivedAt)
    {
        ArchivedAt = archivedAt;
    }

    public void UpdateVisuals(StructuredData viewProperties, StructuredData logoProperties)
    {
        ViewProperties = viewProperties;
        LogoProperties = logoProperties;
    }

    public void AddMember(Guid userId)
    {
        _memberIds.Add(userId);
    }

    public void RemoveMember(Guid userId)
    {
        _memberIds.Remove(userId);
    }
}

public sealed class ModuleMember : ProjectScopedEntity
{
    private ModuleMember()
    {
    }

    private ModuleMember(Guid id, Guid workspaceId, Guid projectId, Guid moduleId, Guid memberId)
        : base(id, workspaceId, projectId)
    {
        ModuleId = moduleId;
        MemberId = memberId;
    }

    public Guid ModuleId { get; private set; }

    public Guid MemberId { get; private set; }

    public static ModuleMember Create(Guid id, Guid workspaceId, Guid projectId, Guid moduleId, Guid memberId)
    {
        return new ModuleMember(id, workspaceId, projectId, moduleId, memberId);
    }
}

public sealed class ModuleIssue : ProjectScopedEntity
{
    private ModuleIssue()
    {
    }

    private ModuleIssue(Guid id, Guid workspaceId, Guid projectId, Guid moduleId, Guid issueId)
        : base(id, workspaceId, projectId)
    {
        ModuleId = moduleId;
        IssueId = issueId;
    }

    public Guid ModuleId { get; private set; }

    public Guid IssueId { get; private set; }

    public static ModuleIssue Create(Guid id, Guid workspaceId, Guid projectId, Guid moduleId, Guid issueId)
    {
        return new ModuleIssue(id, workspaceId, projectId, moduleId, issueId);
    }
}

public sealed class ModuleLink : ProjectScopedEntity
{
    private ModuleLink()
    {
    }

    private ModuleLink(Guid id, Guid workspaceId, Guid projectId, Guid moduleId, Url url, StructuredData metadata)
        : base(id, workspaceId, projectId)
    {
        ModuleId = moduleId;
        Url = url;
        Metadata = metadata;
    }

    public string? Title { get; private set; }

    public Url Url { get; private set; } = Url.Create("https://example.com");

    public Guid ModuleId { get; private set; }

    public StructuredData Metadata { get; private set; } = StructuredData.FromJson(null);

    public static ModuleLink Create(Guid id, Guid workspaceId, Guid projectId, Guid moduleId, Url url, StructuredData metadata)
    {
        return new ModuleLink(id, workspaceId, projectId, moduleId, url, metadata);
    }

    public void Update(string? title, Url url, StructuredData metadata)
    {
        Title = title;
        Url = url;
        Metadata = metadata;
    }
}

public sealed class ModuleUserProperties : ProjectScopedEntity
{
    private ModuleUserProperties()
    {
    }

    private ModuleUserProperties(Guid id, Guid workspaceId, Guid projectId, Guid moduleId, Guid userId, ViewPreferences preferences)
        : base(id, workspaceId, projectId)
    {
        ModuleId = moduleId;
        UserId = userId;
        Preferences = preferences;
    }

    public Guid ModuleId { get; private set; }

    public Guid UserId { get; private set; }

    public ViewPreferences Preferences { get; private set; } = ViewPreferences.CreateModuleDefaults();

    public static ModuleUserProperties Create(Guid id, Guid workspaceId, Guid projectId, Guid moduleId, Guid userId, ViewPreferences preferences)
    {
        var effectivePreferences = preferences ?? ViewPreferences.CreateModuleDefaults();
        return new ModuleUserProperties(id, workspaceId, projectId, moduleId, userId, effectivePreferences);
    }

    public void Update(ViewPreferences preferences)
    {
        Preferences = preferences ?? ViewPreferences.CreateModuleDefaults();
    }
}
