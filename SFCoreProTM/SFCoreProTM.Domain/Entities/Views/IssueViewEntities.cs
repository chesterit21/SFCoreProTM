using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Views;

public sealed class IssueView : WorkspaceScopedEntity
{
    private IssueView()
    {
    }

    private IssueView(Guid id, Guid workspaceId, string name, StructuredData query, ViewPreferences preferences, double sortOrder)
        : base(id, workspaceId)
    {
        Name = name;
        Query = query;
        Preferences = preferences;
        SortOrder = sortOrder;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public StructuredData Query { get; private set; } = StructuredData.FromJson(null);

    public ViewPreferences Preferences { get; private set; } = ViewPreferences.CreateIssueDefaults();

    public string Access { get; private set; } = "private";

    public double SortOrder { get; private set; }

    public StructuredData LogoProperties { get; private set; } = StructuredData.FromJson(null);

    public Guid? OwnedById { get; private set; }

    public bool IsLocked { get; private set; }

    public static IssueView Create(Guid id, Guid workspaceId, string name, StructuredData query, ViewPreferences preferences, double sortOrder)
    {
        var effectivePreferences = preferences ?? ViewPreferences.CreateIssueDefaults();
        return new IssueView(id, workspaceId, name, query, effectivePreferences, sortOrder);
    }

    public void UpdateDetails(string? description, StructuredData query, ViewPreferences preferences, StructuredData logoProperties)
    {
        Description = description;
        Query = query;
        Preferences = preferences ?? ViewPreferences.CreateIssueDefaults();
        LogoProperties = logoProperties;
    }

    public void UpdateAccess(string access, Guid? ownedById, bool isLocked)
    {
        Access = access;
        OwnedById = ownedById;
        IsLocked = isLocked;
    }

    public void UpdateSortOrder(double sortOrder)
    {
        SortOrder = sortOrder;
    }
}
