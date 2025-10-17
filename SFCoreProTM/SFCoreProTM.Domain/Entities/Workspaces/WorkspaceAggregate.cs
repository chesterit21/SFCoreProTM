using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Workspaces;

public sealed class Workspace : AggregateRoot
{
    private readonly List<WorkspaceMember> _members = new();
    private readonly List<Team> _teams = new();

    private Workspace()
    {
    }

    private Workspace(Guid id, string name, Guid ownerId, Slug slug, string timezone)
        : base(id)
    {
        Name = name;
        OwnerId = ownerId;
        Slug = slug;
        Timezone = timezone;
    }

    public string Name { get; private set; } = string.Empty;

    public Url? Logo { get; private set; }

    public Guid? LogoAssetId { get; private set; }

    public Guid OwnerId { get; private set; }

    public Slug Slug { get; private set; } = Slug.Create("default-workspace");

    public string? OrganizationSize { get; private set; }

    public string Timezone { get; private set; } = "UTC";

    public ColorCode? BackgroundColor { get; private set; }

    public IReadOnlyCollection<WorkspaceMember> Members => _members.AsReadOnly();

    public IReadOnlyCollection<Team> Teams => _teams.AsReadOnly();

    public static Workspace Create(Guid id, string name, Guid ownerId, Slug slug, string timezone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(timezone);

        return new Workspace(id, name, ownerId, slug, timezone);
    }

    public void UpdateBranding(Url? logo, Guid? logoAssetId, ColorCode? backgroundColor)
    {
        Logo = logo;
        LogoAssetId = logoAssetId;
        BackgroundColor = backgroundColor;
    }

    public void UpdateOrganizationProfile(string? organizationSize, string timezone)
    {
        OrganizationSize = organizationSize;
        Timezone = timezone;
    }

    public WorkspaceMember AddMember(Guid id, Guid memberId, WorkspaceRole role, WorkspaceMemberPreferences preferences, bool isActive)
    {
        var effectivePreferences = preferences ?? WorkspaceMemberPreferences.CreateDefault();
        var member = WorkspaceMember.Create(id, Id, memberId, role, effectivePreferences, isActive);
        _members.Add(member);
        return member;
    }

    public Team AddTeam(Guid id, string name, StructuredData logoProps)
    {
        var team = Team.Create(id, Id, name, logoProps);
        _teams.Add(team);
        return team;
    }
}

public enum WorkspaceRole
{
    Guest = 0,
    Member = 1,
    Admin = 2,
    Owner = 3,
}

public sealed class WorkspaceMember : WorkspaceScopedEntity
{
    private WorkspaceMember()
    {
    }

    private WorkspaceMember(Guid id, Guid workspaceId, Guid memberId, WorkspaceRole role, WorkspaceMemberPreferences preferences, bool isActive)
        : base(id, workspaceId)
    {
        MemberId = memberId;
        Role = role;
        Preferences = preferences;
        IsActive = isActive;
    }

    public Guid MemberId { get; private set; }

    public WorkspaceRole Role { get; private set; }

    public string? CompanyRole { get; private set; }

    public WorkspaceMemberPreferences Preferences { get; private set; } = WorkspaceMemberPreferences.CreateDefault();

    public bool IsActive { get; private set; }

    public static WorkspaceMember Create(Guid id, Guid workspaceId, Guid memberId, WorkspaceRole role, WorkspaceMemberPreferences preferences, bool isActive)
    {
        var effectivePreferences = preferences ?? WorkspaceMemberPreferences.CreateDefault();
        return new WorkspaceMember(id, workspaceId, memberId, role, effectivePreferences, isActive);
    }

    public void UpdateCompanyRole(string? companyRole)
    {
        CompanyRole = companyRole;
    }

    public void UpdatePreferences(WorkspaceMemberPreferences preferences)
    {
        Preferences = preferences ?? WorkspaceMemberPreferences.CreateDefault();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }
}

public sealed class WorkspaceMemberInvite : WorkspaceScopedEntity
{
    private WorkspaceMemberInvite()
    {
    }

    private WorkspaceMemberInvite(Guid id, Guid workspaceId, EmailAddress email, Guid token, WorkspaceRole role)
        : base(id, workspaceId)
    {
        Email = email;
        Token = token;
        Role = role;
    }

    public EmailAddress Email { get; private set; } = EmailAddress.Create("placeholder@example.com");

    public bool Accepted { get; private set; }

    public Guid Token { get; private set; }

    public string? Message { get; private set; }

    public DateTime? RespondedAt { get; private set; }

    public WorkspaceRole Role { get; private set; }

    public static WorkspaceMemberInvite Create(Guid id, Guid workspaceId, EmailAddress email, Guid token, WorkspaceRole role)
    {
        return new WorkspaceMemberInvite(id, workspaceId, email, token, role);
    }

    public void Accept(DateTime respondedAt)
    {
        Accepted = true;
        RespondedAt = respondedAt;
    }

    public void AddMessage(string? message)
    {
        Message = message;
    }
}

public sealed class Team : WorkspaceScopedEntity
{
    private Team()
    {
    }

    private Team(Guid id, Guid workspaceId, string name, StructuredData logoProps)
        : base(id, workspaceId)
    {
        Name = name;
        LogoProperties = logoProps;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public StructuredData LogoProperties { get; private set; } = StructuredData.FromJson(null);

    public static Team Create(Guid id, Guid workspaceId, string name, StructuredData logoProps)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Team(id, workspaceId, name, logoProps);
    }

    public void UpdateDetails(string? description, StructuredData logoProps)
    {
        Description = description;
        LogoProperties = logoProps;
    }
}

public sealed class WorkspaceTheme : WorkspaceScopedEntity
{
    private WorkspaceTheme()
    {
    }

    private WorkspaceTheme(Guid id, Guid workspaceId, string name, Guid actorId, StructuredData colors)
        : base(id, workspaceId)
    {
        Name = name;
        ActorId = actorId;
        Colors = colors;
    }

    public string Name { get; private set; } = string.Empty;

    public Guid ActorId { get; private set; }

    public StructuredData Colors { get; private set; } = StructuredData.FromJson(null);

    public static WorkspaceTheme Create(Guid id, Guid workspaceId, string name, Guid actorId, StructuredData colors)
    {
        return new WorkspaceTheme(id, workspaceId, name, actorId, colors);
    }

    public void UpdatePalette(StructuredData colors)
    {
        Colors = colors;
    }
}

public sealed class WorkspaceUserProperties : WorkspaceScopedEntity
{
    private WorkspaceUserProperties()
    {
    }

    private WorkspaceUserProperties(Guid id, Guid workspaceId, Guid userId, ViewPreferences preferences)
        : base(id, workspaceId)
    {
        UserId = userId;
        Preferences = preferences;
    }

    public Guid UserId { get; private set; }

    public ViewPreferences Preferences { get; private set; } = ViewPreferences.CreateIssueDefaults();

    public static WorkspaceUserProperties Create(Guid id, Guid workspaceId, Guid userId, ViewPreferences preferences)
    {
        var effectivePreferences = preferences ?? ViewPreferences.CreateIssueDefaults();
        return new WorkspaceUserProperties(id, workspaceId, userId, effectivePreferences);
    }

    public void UpdatePreferences(ViewPreferences preferences)
    {
        Preferences = preferences ?? ViewPreferences.CreateIssueDefaults();
    }
}

public sealed class WorkspaceUserLink : WorkspaceScopedEntity
{
    private WorkspaceUserLink()
    {
    }

    private WorkspaceUserLink(Guid id, Guid workspaceId, string url, Guid ownerId, StructuredData metadata)
        : base(id, workspaceId)
    {
        Url = url;
        OwnerId = ownerId;
        Metadata = metadata;
    }

    public string? Title { get; private set; }

    public string Url { get; private set; } = string.Empty;

    public StructuredData Metadata { get; private set; } = StructuredData.FromJson(null);

    public Guid OwnerId { get; private set; }

    public static WorkspaceUserLink Create(Guid id, Guid workspaceId, string url, Guid ownerId, StructuredData metadata)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        return new WorkspaceUserLink(id, workspaceId, url, ownerId, metadata);
    }

    public void UpdateDetails(string? title, StructuredData metadata)
    {
        Title = title;
        Metadata = metadata;
    }
}

public sealed class WorkspaceHomePreference : WorkspaceScopedEntity
{
    private WorkspaceHomePreference()
    {
    }

    private WorkspaceHomePreference(Guid id, Guid workspaceId, Guid userId, string key, bool isEnabled, StructuredData config, double sortOrder)
        : base(id, workspaceId)
    {
        UserId = userId;
        Key = key;
        IsEnabled = isEnabled;
        Config = config;
        SortOrder = sortOrder;
    }

    public Guid UserId { get; private set; }

    public string Key { get; private set; } = string.Empty;

    public bool IsEnabled { get; private set; }

    public StructuredData Config { get; private set; } = StructuredData.FromJson(null);

    public double SortOrder { get; private set; }

    public static WorkspaceHomePreference Create(Guid id, Guid workspaceId, Guid userId, string key, bool isEnabled, StructuredData config, double sortOrder)
    {
        return new WorkspaceHomePreference(id, workspaceId, userId, key, isEnabled, config, sortOrder);
    }

    public void Update(bool isEnabled, StructuredData config, double sortOrder)
    {
        IsEnabled = isEnabled;
        Config = config;
        SortOrder = sortOrder;
    }
}

public sealed class WorkspaceUserPreference : WorkspaceScopedEntity
{
    private WorkspaceUserPreference()
    {
    }

    private WorkspaceUserPreference(Guid id, Guid workspaceId, Guid userId, string key, bool isPinned, double sortOrder)
        : base(id, workspaceId)
    {
        UserId = userId;
        Key = key;
        IsPinned = isPinned;
        SortOrder = sortOrder;
    }

    public Guid UserId { get; private set; }

    public string Key { get; private set; } = string.Empty;

    public bool IsPinned { get; private set; }

    public double SortOrder { get; private set; }

    public static WorkspaceUserPreference Create(Guid id, Guid workspaceId, Guid userId, string key, bool isPinned, double sortOrder)
    {
        return new WorkspaceUserPreference(id, workspaceId, userId, key, isPinned, sortOrder);
    }

    public void Update(bool isPinned, double sortOrder)
    {
        IsPinned = isPinned;
        SortOrder = sortOrder;
    }
}
