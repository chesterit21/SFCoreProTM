using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Pages;

public enum PageAccess
{
    Public = 0,
    Private = 1,
}

public sealed class Page : AggregateRoot
{
    private readonly HashSet<Guid> _labelIds = new();
    private readonly HashSet<Guid> _projectIds = new();

    private Page()
    {
    }

    private Page(Guid id, Guid workspaceId, Guid ownerId, string name, PageAccess access, RichTextContent description)
        : base(id)
    {
        WorkspaceId = workspaceId;
        OwnerId = ownerId;
        Name = name;
        Access = access;
        Description = description;
    }

    public Guid WorkspaceId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public RichTextContent Description { get; private set; } = RichTextContent.Create();

    public Guid OwnerId { get; private set; }

    public PageAccess Access { get; private set; } = PageAccess.Public;

    public ColorCode? Color { get; private set; }

    public IReadOnlyCollection<Guid> LabelIds => _labelIds;

    public Guid? ParentId { get; private set; }

    public DateTime? ArchivedAt { get; private set; }

    public bool IsLocked { get; private set; }

    public StructuredData ViewProperties { get; private set; } = StructuredData.FromJson(null);

    public StructuredData LogoProperties { get; private set; } = StructuredData.FromJson(null);

    public bool IsGlobal { get; private set; }

    public IReadOnlyCollection<Guid> ProjectIds => _projectIds;

    public Guid? MovedToPageId { get; private set; }

    public Guid? MovedToProjectId { get; private set; }

    public double SortOrder { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public static Page Create(Guid id, Guid workspaceId, Guid ownerId, string name, PageAccess access, RichTextContent description)
    {
        return new Page(id, workspaceId, ownerId, name, access, description);
    }

    public void UpdateContent(string name, RichTextContent description, ColorCode? color)
    {
        Name = name;
        Description = description;
        Color = color;
    }

    public void UpdateAccess(PageAccess access, bool isLocked, bool isGlobal)
    {
        Access = access;
        IsLocked = isLocked;
        IsGlobal = isGlobal;
    }

    public void UpdateHierarchy(Guid? parentId, Guid? movedToPageId, Guid? movedToProjectId, double sortOrder)
    {
        ParentId = parentId;
        MovedToPageId = movedToPageId;
        MovedToProjectId = movedToProjectId;
        SortOrder = sortOrder;
    }

    public void Archive(DateTime? archivedAt)
    {
        ArchivedAt = archivedAt;
    }

    public void ConfigureProperties(StructuredData viewProps, StructuredData logoProps)
    {
        ViewProperties = viewProps;
        LogoProperties = logoProps;
    }

    public void SetExternalReference(string? source, string? identifier)
    {
        ExternalReference = source is null && identifier is null ? null : ExternalReference.Create(source, identifier);
    }

    public void AssignOwner(Guid ownerId)
    {
        OwnerId = ownerId;
    }

    public void AddLabel(Guid labelId)
    {
        _labelIds.Add(labelId);
    }

    public void RemoveLabel(Guid labelId)
    {
        _labelIds.Remove(labelId);
    }

    public void AddProject(Guid projectId)
    {
        _projectIds.Add(projectId);
    }

    public void RemoveProject(Guid projectId)
    {
        _projectIds.Remove(projectId);
    }
}

public sealed class PageLog : Entity
{
    private PageLog()
    {
    }

    private PageLog(Guid id, Guid pageId, Guid workspaceId, string entityName)
        : base(id)
    {
        PageId = pageId;
        WorkspaceId = workspaceId;
        EntityName = entityName;
    }

    public Guid PageId { get; private set; }

    public Guid WorkspaceId { get; private set; }

    public Guid TransactionId { get; private set; } = Guid.NewGuid();

    public Guid? EntityIdentifier { get; private set; }

    public string EntityName { get; private set; } = string.Empty;

    public string? EntityType { get; private set; }

    public static PageLog Create(Guid id, Guid pageId, Guid workspaceId, string entityName)
    {
        return new PageLog(id, pageId, workspaceId, entityName);
    }

    public void SetTransaction(Guid transactionId)
    {
        TransactionId = transactionId;
    }

    public void SetEntity(string? entityType, Guid? entityIdentifier)
    {
        EntityType = entityType;
        EntityIdentifier = entityIdentifier;
    }
}

public sealed class PageLabel : Entity
{
    private PageLabel()
    {
    }

    private PageLabel(Guid id, Guid workspaceId, Guid pageId, Guid labelId)
        : base(id)
    {
        WorkspaceId = workspaceId;
        PageId = pageId;
        LabelId = labelId;
    }

    public Guid WorkspaceId { get; private set; }

    public Guid PageId { get; private set; }

    public Guid LabelId { get; private set; }

    public static PageLabel Create(Guid id, Guid workspaceId, Guid pageId, Guid labelId)
    {
        return new PageLabel(id, workspaceId, pageId, labelId);
    }
}

public sealed class ProjectPage : Entity
{
    private ProjectPage()
    {
    }

    private ProjectPage(Guid id, Guid workspaceId, Guid projectId, Guid pageId)
        : base(id)
    {
        WorkspaceId = workspaceId;
        ProjectId = projectId;
        PageId = pageId;
    }

    public Guid WorkspaceId { get; private set; }

    public Guid ProjectId { get; private set; }

    public Guid PageId { get; private set; }

    public static ProjectPage Create(Guid id, Guid workspaceId, Guid projectId, Guid pageId)
    {
        return new ProjectPage(id, workspaceId, projectId, pageId);
    }
}

public sealed class PageVersion : Entity
{
    private PageVersion()
    {
    }

    private PageVersion(Guid id, Guid workspaceId, Guid pageId, Guid ownerId, DateTime lastSavedAt, RichTextContent description, StructuredData subPages)
        : base(id)
    {
        WorkspaceId = workspaceId;
        PageId = pageId;
        OwnerId = ownerId;
        LastSavedAt = lastSavedAt;
        Description = description;
        SubPagesData = subPages;
    }

    public Guid WorkspaceId { get; private set; }

    public Guid PageId { get; private set; }

    public DateTime LastSavedAt { get; private set; }

    public Guid OwnerId { get; private set; }

    public RichTextContent Description { get; private set; } = RichTextContent.Create();

    public StructuredData SubPagesData { get; private set; } = StructuredData.FromJson(null);

    public static PageVersion Create(Guid id, Guid workspaceId, Guid pageId, Guid ownerId, DateTime lastSavedAt, RichTextContent description, StructuredData subPages)
    {
        return new PageVersion(id, workspaceId, pageId, ownerId, lastSavedAt, description, subPages);
    }

    public void Update(RichTextContent description, DateTime lastSavedAt, StructuredData subPages)
    {
        Description = description;
        LastSavedAt = lastSavedAt;
        SubPagesData = subPages;
    }
}
