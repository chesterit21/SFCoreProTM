using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Projects;

public sealed class Project : AggregateRoot
{
    private readonly List<ProjectMember> _members = new();

    private Project()
    {
    }

    private Project(Guid id, Guid workspaceId, string name, string identifier, ProjectVisibility visibility, string timezone)
        : base(id)
    {
        WorkspaceId = workspaceId;
        Name = name;
        Identifier = identifier;
        Visibility = visibility;
        Timezone = timezone;
    }

    public Guid WorkspaceId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public RichTextContent Description { get; private set; } = RichTextContent.Create();

    public StructuredData DescriptionRichText { get; private set; } = StructuredData.FromJson(null);

    public StructuredData DescriptionHtml { get; private set; } = StructuredData.FromJson(null);

    public ProjectVisibility Visibility { get; private set; } = ProjectVisibility.Public;

    public string Identifier { get; private set; } = string.Empty;

    public Guid? DefaultAssigneeId { get; private set; }

    public Guid? ProjectLeadId { get; private set; }

    public string? Emoji { get; private set; }

    public StructuredData IconProperties { get; private set; } = StructuredData.FromJson(null);

    public bool ModuleViewEnabled { get; private set; }

    public bool CycleViewEnabled { get; private set; }

    public bool IssueViewsEnabled { get; private set; }

    public bool PageViewEnabled { get; private set; } = true;

    public bool IntakeViewEnabled { get; private set; }

    public bool TimeTrackingEnabled { get; private set; }

    public bool IssueTypeEnabled { get; private set; }

    public bool GuestViewAllFeatures { get; private set; }

    public Url? CoverImage { get; private set; }

    public Guid? CoverImageAssetId { get; private set; }

    public Guid? EstimateId { get; private set; }

    public int ArchiveInMonths { get; private set; }

    public int CloseInMonths { get; private set; }

    public StructuredData LogoProperties { get; private set; } = StructuredData.FromJson(null);

    public Guid? DefaultStateId { get; private set; }

    public DateTime? ArchivedAt { get; private set; }

    public string Timezone { get; private set; } = "UTC";

    public ExternalReference? ExternalReference { get; private set; }

    public IReadOnlyCollection<ProjectMember> Members => _members.AsReadOnly();

    public static Project Create(Guid id, Guid workspaceId, string name, string identifier, ProjectVisibility visibility, string timezone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(timezone);

        return new Project(id, workspaceId, name, identifier.ToUpperInvariant(), visibility, timezone);
    }

    public void UpdateDetails(RichTextContent description, StructuredData richText, StructuredData html)
    {
        Description = description;
        DescriptionRichText = richText;
        DescriptionHtml = html;
    }

    public void ConfigureViews(bool moduleView, bool cycleView, bool issueViews, bool pageView, bool intakeView)
    {
        ModuleViewEnabled = moduleView;
        CycleViewEnabled = cycleView;
        IssueViewsEnabled = issueViews;
        PageViewEnabled = pageView;
        IntakeViewEnabled = intakeView;
    }

    public void ConfigureFeatures(bool timeTracking, bool issueType, bool guestViewAll)
    {
        TimeTrackingEnabled = timeTracking;
        IssueTypeEnabled = issueType;
        GuestViewAllFeatures = guestViewAll;
    }

    public void AssignLeadership(Guid? projectLeadId, Guid? defaultAssigneeId)
    {
        ProjectLeadId = projectLeadId;
        DefaultAssigneeId = defaultAssigneeId;
    }

    public void UpdateBranding(string? emoji, StructuredData iconProperties, StructuredData logoProperties, Url? coverImage, Guid? coverImageAssetId)
    {
        Emoji = emoji;
        IconProperties = iconProperties;
        LogoProperties = logoProperties;
        CoverImage = coverImage;
        CoverImageAssetId = coverImageAssetId;
    }

    public void ConfigureCadence(int archiveInMonths, int closeInMonths)
    {
        ArchiveInMonths = archiveInMonths;
        CloseInMonths = closeInMonths;
    }

    public void SetDefaultState(Guid? stateId)
    {
        DefaultStateId = stateId;
    }

    public void SetEstimate(Guid? estimateId)
    {
        EstimateId = estimateId;
    }

    public void SetVisibility(ProjectVisibility visibility)
    {
        Visibility = visibility;
    }

    public void Archive(DateTime? archivedAt)
    {
        ArchivedAt = archivedAt;
    }

    public void SetExternalReference(string? source, string? identifier)
    {
        ExternalReference = source is null && identifier is null ? null : ExternalReference.Create(source, identifier);
    }

    public ProjectMember AddMember(Guid id, Guid userId, ProjectRole role, ProjectMemberPreferences preferences, double sortOrder, bool isActive)
    {
        var effectivePreferences = preferences ?? ProjectMemberPreferences.CreateDefault();
        var member = ProjectMember.Create(id, WorkspaceId, Id, userId, role, effectivePreferences, sortOrder, isActive);
        _members.Add(member);
        return member;
    }
}

public enum ProjectVisibility
{
    Secret = 0,
    Public = 2,
}

public enum ProjectRole
{
    Guest = 5,
    Member = 15,
    Admin = 20,
}

public sealed class ProjectMemberInvite : ProjectScopedEntity
{
    private ProjectMemberInvite()
    {
    }

    private ProjectMemberInvite(Guid id, Guid workspaceId, Guid projectId, EmailAddress email, Guid token, ProjectRole role)
        : base(id, workspaceId, projectId)
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

    public ProjectRole Role { get; private set; }

    public static ProjectMemberInvite Create(Guid id, Guid workspaceId, Guid projectId, EmailAddress email, Guid token, ProjectRole role)
    {
        return new ProjectMemberInvite(id, workspaceId, projectId, email, token, role);
    }

    public void Accept(DateTime respondedAt)
    {
        Accepted = true;
        RespondedAt = respondedAt;
    }

    public void UpdateMessage(string? message)
    {
        Message = message;
    }
}

public sealed class ProjectMember : ProjectScopedEntity
{
    private ProjectMember()
    {
    }

    private ProjectMember(Guid id, Guid workspaceId, Guid projectId, Guid memberId, ProjectRole role, ProjectMemberPreferences preferences, double sortOrder, bool isActive)
        : base(id, workspaceId, projectId)
    {
        MemberId = memberId;
        Role = role;
        Preferences = preferences;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public Guid MemberId { get; private set; }

    public string? Comment { get; private set; }

    public ProjectRole Role { get; private set; }

    public ProjectMemberPreferences Preferences { get; private set; } = ProjectMemberPreferences.CreateDefault();

    public double SortOrder { get; private set; }

    public bool IsActive { get; private set; }

    public static ProjectMember Create(Guid id, Guid workspaceId, Guid projectId, Guid memberId, ProjectRole role, ProjectMemberPreferences preferences, double sortOrder, bool isActive)
    {
        var effectivePreferences = preferences ?? ProjectMemberPreferences.CreateDefault();
        return new ProjectMember(id, workspaceId, projectId, memberId, role, effectivePreferences, sortOrder, isActive);
    }

    public void UpdateParticipation(ProjectRole role, ProjectMemberPreferences preferences, double sortOrder, bool isActive)
    {
        Role = role;
        Preferences = preferences ?? ProjectMemberPreferences.CreateDefault();
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public void UpdateComment(string? comment)
    {
        Comment = comment;
    }
}

public sealed class ProjectIdentifier : AggregateRoot
{
    private ProjectIdentifier()
    {
    }

    private ProjectIdentifier(Guid id, Guid workspaceId, Guid projectId, string name)
        : base(id)
    {
        WorkspaceId = workspaceId;
        ProjectId = projectId;
        Name = name;
    }

    public Guid WorkspaceId { get; private set; }

    public Guid ProjectId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public static ProjectIdentifier Create(Guid id, Guid workspaceId, Guid projectId, string name)
    {
        return new ProjectIdentifier(id, workspaceId, projectId, name);
    }
}

public sealed class ProjectDeployBoard : ProjectScopedEntity
{
    private ProjectDeployBoard()
    {
    }

    private ProjectDeployBoard(Guid id, Guid workspaceId, Guid projectId, string anchor)
        : base(id, workspaceId, projectId)
    {
        Anchor = anchor;
    }

    public string Anchor { get; private set; } = string.Empty;

    public bool Comments { get; private set; }

    public bool Reactions { get; private set; }

    public Guid? IntakeId { get; private set; }

    public bool Votes { get; private set; }

    public StructuredData Views { get; private set; } = StructuredData.FromJson(null);

    public static ProjectDeployBoard Create(Guid id, Guid workspaceId, Guid projectId, string anchor)
    {
        return new ProjectDeployBoard(id, workspaceId, projectId, anchor);
    }

    public void Configure(bool comments, bool reactions, bool votes, Guid? intakeId, StructuredData views)
    {
        Comments = comments;
        Reactions = reactions;
        Votes = votes;
        IntakeId = intakeId;
        Views = views;
    }
}

public sealed class ProjectPublicMember : ProjectScopedEntity
{
    private ProjectPublicMember()
    {
    }

    private ProjectPublicMember(Guid id, Guid workspaceId, Guid projectId, Guid memberId)
        : base(id, workspaceId, projectId)
    {
        MemberId = memberId;
    }

    public Guid MemberId { get; private set; }

    public static ProjectPublicMember Create(Guid id, Guid workspaceId, Guid projectId, Guid memberId)
    {
        return new ProjectPublicMember(id, workspaceId, projectId, memberId);
    }
}
