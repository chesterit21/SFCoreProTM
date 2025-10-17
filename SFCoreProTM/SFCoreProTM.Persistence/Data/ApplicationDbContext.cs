using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Domain.Entities.Users;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Persistence.Data;

public class ApplicationDbContext : DbContext
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private static readonly HashSet<Guid> EmptyGuidSet = new();

    private static readonly ValueConverter<AuditTrail, string?> AuditTrailConverter = new(
        value => JsonSerializer.Serialize(
            new AuditTrailSurrogate(value.CreatedAt, value.CreatedById, value.UpdatedAt, value.UpdatedById, value.DeletedAt),
            JsonOptions),
        value => DeserializeAuditTrail(value));

    private static readonly ValueComparer<AuditTrail> AuditTrailComparer = new(
        (left, right) =>
            left.CreatedAt == right.CreatedAt &&
            left.CreatedById == right.CreatedById &&
            left.UpdatedAt == right.UpdatedAt &&
            left.UpdatedById == right.UpdatedById &&
            left.DeletedAt == right.DeletedAt,
        value => HashCode.Combine(value.CreatedAt, value.CreatedById, value.UpdatedAt, value.UpdatedById, value.DeletedAt),
        value => AuditTrail.Create(value.CreatedAt, value.CreatedById, value.UpdatedAt, value.UpdatedById, value.DeletedAt));

    private static readonly ValueConverter<HashSet<Guid>, Guid[]> GuidSetConverter = new(
        set => set == null ? Array.Empty<Guid>() : set.ToArray(),
        array => array == null ? new HashSet<Guid>() : new HashSet<Guid>(array));

    private static readonly ValueComparer<HashSet<Guid>> GuidSetComparer = new(
        (left, right) => (left ?? EmptyGuidSet).SetEquals(right ?? EmptyGuidSet),
        set => set == null ? 0 : set.Aggregate(0, (current, guid) => HashCode.Combine(current, guid.GetHashCode())),
        set => set == null ? new HashSet<Guid>() : new HashSet<Guid>(set));

    private static readonly ValueConverter<EmailAddress?, string?> EmailAddressConverter = new(
        value => value == null ? null : value.Value,
        value => string.IsNullOrWhiteSpace(value)
            ? null
            : EmailAddress.Create(value));

    private static readonly ValueComparer<EmailAddress?> EmailAddressComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null && string.Equals(left.Value, right.Value, StringComparison.OrdinalIgnoreCase)),
        value => value == null
            ? 0
            : value.Value.ToUpperInvariant().GetHashCode(StringComparison.Ordinal),
        value => value == null
            ? null
            : EmailAddress.Create(value.Value));

    private static readonly ValueConverter<StructuredData, string?> StructuredDataConverter = new(
        value => value == null ? null : value.RawJson,
        json => StructuredData.FromJson(json));

    private static readonly ValueComparer<StructuredData> StructuredDataComparer = new(
        (left, right) => string.Equals(left == null ? null : left.RawJson, right == null ? null : right.RawJson, StringComparison.Ordinal),
        value => value == null || value.RawJson == null ? 0 : value.RawJson.GetHashCode(StringComparison.Ordinal),
        value => value == null ? StructuredData.FromJson(null) : StructuredData.FromJson(value.RawJson));

    private static readonly ValueConverter<RichTextContent, string?> RichTextContentConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(
                new RichTextContentSurrogate(
                    value.PlainText,
                    value.Html,
                    value.Json,
                    value.Binary == null ? null : Convert.ToBase64String(value.Binary)),
                JsonOptions),
        json => string.IsNullOrWhiteSpace(json)
            ? RichTextContent.Create()
            : DeserializeRichText(json!));

    private static readonly ValueComparer<RichTextContent> RichTextContentComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             string.Equals(left.PlainText, right.PlainText, StringComparison.Ordinal) &&
             string.Equals(left.Html, right.Html, StringComparison.Ordinal) &&
             string.Equals(left.Json, right.Json, StringComparison.Ordinal) &&
             StructuralComparisons.StructuralEqualityComparer.Equals(left.Binary, right.Binary)),
        value => value == null ? 0 : HashCode.Combine(value.PlainText, value.Html, value.Json, value.Binary),
        value => value == null ? RichTextContent.Create() : RichTextContent.Create(value.PlainText, value.Html, value.Binary, value.Json));

    private static readonly ValueConverter<DateRange, string?> DateRangeConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(new DateRangeSurrogate(value.Start, value.End), JsonOptions),
        json => string.IsNullOrWhiteSpace(json)
            ? DateRange.Create(null, null)
            : DeserializeDateRange(json!));

    private static readonly ValueComparer<DateRange> DateRangeComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             Nullable.Equals(left.Start, right.Start) &&
             Nullable.Equals(left.End, right.End)),
        value => value == null ? 0 : HashCode.Combine(value.Start, value.End),
        value => value == null ? DateRange.Create(null, null) : DateRange.Create(value.Start, value.End));

    private static readonly ValueConverter<ExternalReference?, string?> ExternalReferenceConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(new ExternalReferenceSurrogate(value.Source, value.Identifier), JsonOptions),
        json => string.IsNullOrWhiteSpace(json)
            ? null
            : DeserializeExternalReference(json!));

    private static readonly ValueComparer<ExternalReference?> ExternalReferenceComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             string.Equals(left.Source, right.Source, StringComparison.Ordinal) &&
             string.Equals(left.Identifier, right.Identifier, StringComparison.Ordinal)),
        value => value == null ? 0 : HashCode.Combine(value.Source, value.Identifier),
        value => value == null ? null : ExternalReference.Create(value.Source, value.Identifier));

    private static readonly ValueConverter<List<Url>, string?> UrlListConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(value.Select(url => url.Value), JsonOptions),
        json => string.IsNullOrWhiteSpace(json)
            ? new List<Url>()
            : JsonSerializer.Deserialize<List<string>>(json!, JsonOptions)!.Select(Url.Create).ToList());

    private static readonly ValueComparer<List<Url>> UrlListComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             left.Count == right.Count &&
             left.Select(url => url.Value).SequenceEqual(right.Select(url => url.Value), StringComparer.Ordinal)),
        value => value == null
            ? 0
            : value.Aggregate(0, (current, url) => HashCode.Combine(current, url.Value.GetHashCode(StringComparison.Ordinal))),
        value => value == null
            ? new List<Url>()
            : value.Select(url => Url.Create(url.Value)).ToList());

    private static readonly ValueConverter<Url?, string?> UrlConverter = new(
        value => value == null ? null : value.Value,
        json => string.IsNullOrWhiteSpace(json) ? null : Url.Create(json!));

    private static readonly ValueComparer<Url?> UrlComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null && string.Equals(left.Value, right.Value, StringComparison.Ordinal)),
        value => value == null ? 0 : value.Value.GetHashCode(StringComparison.Ordinal),
        value => value == null ? null : Url.Create(value.Value));

    private static readonly ValueConverter<ViewPreferences, string?> ViewPreferencesConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(
                new ViewPreferencesSurrogate(
                    value.Filters,
                    value.DisplayFilters,
                    value.DisplayProperties,
                    value.RichFilters),
                JsonOptions),
        json => string.IsNullOrWhiteSpace(json)
            ? ViewPreferences.CreateIssueDefaults()
            : DeserializeViewPreferences(json!));

    private static readonly ValueComparer<ViewPreferences> ViewPreferencesComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             string.Equals(left.Filters, right.Filters, StringComparison.Ordinal) &&
             string.Equals(left.DisplayFilters, right.DisplayFilters, StringComparison.Ordinal) &&
             string.Equals(left.DisplayProperties, right.DisplayProperties, StringComparison.Ordinal) &&
             string.Equals(left.RichFilters, right.RichFilters, StringComparison.Ordinal)),
        value => value == null
            ? 0
            : HashCode.Combine(value.Filters, value.DisplayFilters, value.DisplayProperties, value.RichFilters),
        value => value == null
            ? ViewPreferences.CreateIssueDefaults()
            : ViewPreferences.Create(value.Filters, value.DisplayFilters, value.DisplayProperties, value.RichFilters));

    private static readonly ValueConverter<WorkspaceMemberPreferences, string?> WorkspaceMemberPreferencesConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(
                new WorkspaceMemberPreferencesSurrogate(
                    value.View.RawJson,
                    value.Defaults.RawJson,
                    value.Issue.RawJson),
                JsonOptions),
        json => DeserializeWorkspaceMemberPreferences(json));

    private static readonly ValueComparer<WorkspaceMemberPreferences> WorkspaceMemberPreferencesComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             string.Equals(left.View.RawJson, right.View.RawJson, StringComparison.Ordinal) &&
             string.Equals(left.Defaults.RawJson, right.Defaults.RawJson, StringComparison.Ordinal) &&
             string.Equals(left.Issue.RawJson, right.Issue.RawJson, StringComparison.Ordinal)),
        value => value == null
            ? 0
            : HashCode.Combine(value.View.RawJson, value.Defaults.RawJson, value.Issue.RawJson),
        value => value == null
            ? WorkspaceMemberPreferences.CreateDefault()
            : WorkspaceMemberPreferences.Create(
                StructuredData.FromJson(value.View.RawJson),
                StructuredData.FromJson(value.Defaults.RawJson),
                StructuredData.FromJson(value.Issue.RawJson)));

    private static readonly ValueConverter<ProjectMemberPreferences, string?> ProjectMemberPreferencesConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(
                new ProjectMemberPreferencesSurrogate(
                    value.View.RawJson,
                    value.Defaults.RawJson,
                    value.Preferences.RawJson),
                JsonOptions),
        json => DeserializeProjectMemberPreferences(json));

    private static readonly ValueComparer<ProjectMemberPreferences> ProjectMemberPreferencesComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             string.Equals(left.View.RawJson, right.View.RawJson, StringComparison.Ordinal) &&
             string.Equals(left.Defaults.RawJson, right.Defaults.RawJson, StringComparison.Ordinal) &&
             string.Equals(left.Preferences.RawJson, right.Preferences.RawJson, StringComparison.Ordinal)),
        value => value == null
            ? 0
            : HashCode.Combine(value.View.RawJson, value.Defaults.RawJson, value.Preferences.RawJson),
        value => value == null
            ? ProjectMemberPreferences.CreateDefault()
            : ProjectMemberPreferences.Create(
                StructuredData.FromJson(value.View.RawJson),
                StructuredData.FromJson(value.Defaults.RawJson),
                StructuredData.FromJson(value.Preferences.RawJson)));

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Issue> Issues => Set<Issue>();

    public DbSet<IssueAssignee> IssueAssignees => Set<IssueAssignee>();

    public DbSet<IssueLabel> IssueLabels => Set<IssueLabel>();

    public DbSet<IssueComment> IssueComments => Set<IssueComment>();

    public DbSet<IssueVersion> IssueVersions => Set<IssueVersion>();

    public DbSet<IssueDescriptionVersion> IssueDescriptionVersions => Set<IssueDescriptionVersion>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    public DbSet<State> States => Set<State>();

    public DbSet<Label> Labels => Set<Label>();

    public DbSet<EstimatePoint> EstimatePoints => Set<EstimatePoint>();

    public DbSet<IssueType> IssueTypes => Set<IssueType>();

    public DbSet<ProjectIssueType> ProjectIssueTypes => Set<ProjectIssueType>();

    public DbSet<IssueUserProperty> IssueUserProperties => Set<IssueUserProperty>();

    public DbSet<ProjectIdentifier> ProjectIdentifiers => Set<ProjectIdentifier>();

    public DbSet<Workspace> Workspaces => Set<Workspace>();

    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<WorkspaceMemberInvite> WorkspaceMemberInvites => Set<WorkspaceMemberInvite>();
    public DbSet<ProjectMemberInvite> ProjectMemberInvites => Set<ProjectMemberInvite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureIssue(modelBuilder);
        ConfigureIssueAssignee(modelBuilder);
        ConfigureIssueLabel(modelBuilder);
        ConfigureIssueComment(modelBuilder);
        ConfigureProject(modelBuilder);
        ConfigureProjectMember(modelBuilder);
        ConfigureState(modelBuilder);
        ConfigureLabel(modelBuilder);
        ConfigureEstimatePoint(modelBuilder);
        ConfigureIssueType(modelBuilder);
        ConfigureProjectIssueType(modelBuilder);
        ConfigureIssueUserProperty(modelBuilder);
        ConfigureIssueVersion(modelBuilder);
        ConfigureIssueDescriptionVersion(modelBuilder);
        ConfigureWorkspace(modelBuilder);
        ConfigureWorkspaceMember(modelBuilder);
        ConfigureProjectIdentifier(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureUserProfile(modelBuilder);
        ConfigureWorkspaceMemberInvite(modelBuilder);
        ConfigureProjectMemberInvite(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null || !typeof(Entity).IsAssignableFrom(clrType))
            {
                continue;
            }

            var auditTrailProperty = entityType.FindProperty(nameof(Entity.AuditTrail));
            if (auditTrailProperty == null)
            {
                continue;
            }

            auditTrailProperty.SetValueConverter(AuditTrailConverter);
            auditTrailProperty.SetValueComparer(AuditTrailComparer);
            auditTrailProperty.SetColumnType("jsonb");
        }

        modelBuilder.Ignore<AuditTrail>();
        modelBuilder.Ignore<StructuredData>();
        modelBuilder.Ignore<RichTextContent>();
        modelBuilder.Ignore<ExternalReference>();
        modelBuilder.Ignore<ViewPreferences>();
        modelBuilder.Ignore<DateRange>();
        modelBuilder.Ignore<Slug>();
        modelBuilder.Ignore<ColorCode>();
        modelBuilder.Ignore<EmailAddress>();
        modelBuilder.Ignore<Url>();
        modelBuilder.Ignore<WorkspaceMemberPreferences>();
        modelBuilder.Ignore<ProjectMemberPreferences>();

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<User>();
        builder.ToTable("users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();

        builder.Property(user => user.Username)
            .HasColumnName("username")
            .HasMaxLength(128)
            .IsRequired();

        var emailProperty = builder.Property(user => user.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .HasConversion(EmailAddressConverter);
        emailProperty.Metadata.SetValueComparer(EmailAddressComparer);

        builder.Property(user => user.MobileNumber)
            .HasColumnName("mobile_number")
            .HasMaxLength(255);

        builder.Property(user => user.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(user => user.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(255);

        builder.Property(user => user.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(255);

        var avatarProperty = builder.Property(user => user.AvatarUrl)
            .HasColumnName("avatar")
            .HasConversion(UrlConverter);
        avatarProperty.Metadata.SetValueComparer(UrlComparer);

        builder.Property(user => user.AvatarAssetId)
            .HasColumnName("avatar_asset_id");

        var coverImageProperty = builder.Property(user => user.CoverImageUrl)
            .HasColumnName("cover_image")
            .HasConversion(UrlConverter);
        coverImageProperty.Metadata.SetValueComparer(UrlComparer);

        builder.Property(user => user.CoverImageAssetId)
            .HasColumnName("cover_image_asset_id");

        builder.Property(user => user.DateJoined).HasColumnName("date_joined");
        builder.Property(user => user.CreatedAt).HasColumnName("created_at");
        builder.Property(user => user.UpdatedAt).HasColumnName("updated_at");

        builder.Property(user => user.LastLocation).HasColumnName("last_location");
        builder.Property(user => user.CreatedLocation).HasColumnName("created_location");
        builder.Property(user => user.IsSuperUser).HasColumnName("is_superuser");
        builder.Property(user => user.IsManaged).HasColumnName("is_managed");
        builder.Property(user => user.IsPasswordExpired).HasColumnName("is_password_expired");
        builder.Property(user => user.IsActive).HasColumnName("is_active");
        builder.Property(user => user.IsStaff).HasColumnName("is_staff");
        builder.Property(user => user.IsEmailVerified).HasColumnName("is_email_verified");
        builder.Property(user => user.IsPasswordAutoset).HasColumnName("is_password_autoset");

        builder.Property(user => user.PasswordHash)
            .HasColumnName("password")
            .HasMaxLength(255);

        builder.Property(user => user.Token).HasColumnName("token");
        builder.Property(user => user.LastActive).HasColumnName("last_active");
        builder.Property(user => user.LastLoginTime).HasColumnName("last_login_time");
        builder.Property(user => user.LastLogoutTime).HasColumnName("last_logout_time");
        builder.Property(user => user.LastLoginIp).HasColumnName("last_login_ip");
        builder.Property(user => user.LastLogoutIp).HasColumnName("last_logout_ip");
        builder.Property(user => user.LastLoginMedium)
            .HasColumnName("last_login_medium")
            .HasMaxLength(20);
        builder.Property(user => user.LastLoginUserAgent).HasColumnName("last_login_uagent");
        builder.Property(user => user.TokenUpdatedAt).HasColumnName("token_updated_at");
        builder.Property(user => user.IsBot).HasColumnName("is_bot");
        builder.Property(user => user.BotType)
            .HasColumnName("bot_type")
            .HasMaxLength(30);
        builder.Property(user => user.UserTimezone)
            .HasColumnName("user_timezone")
            .HasMaxLength(255);
        builder.Property(user => user.IsEmailValid).HasColumnName("is_email_valid");
        builder.Property(user => user.MaskedAt).HasColumnName("masked_at");
    }

    private static void ConfigureUserProfile(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<UserProfile>();
        builder.ToTable("user_profiles");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.UserId).IsRequired();

        var themeProperty = builder.Property(p => p.Theme)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        themeProperty.Metadata.SetValueComparer(StructuredDataComparer);

        builder.Property(p => p.IsAppRailDocked);
        builder.Property(p => p.IsTourCompleted);
        var onboardingProperty = builder.Property(p => p.OnboardingStep)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        onboardingProperty.Metadata.SetValueComparer(StructuredDataComparer);

        builder.Property(p => p.UseCase);
        builder.Property(p => p.Role);
        builder.Property(p => p.IsOnboarded);
        builder.Property(p => p.LastWorkspaceId);
        builder.Property(p => p.BillingAddressCountry);
        var billingProperty = builder.Property(p => p.BillingAddress)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        billingProperty.Metadata.SetValueComparer(StructuredDataComparer);
        builder.Property(p => p.HasBillingAddress);
        builder.Property(p => p.CompanyName);
        builder.Property(p => p.IsSmoothCursorEnabled);
        builder.Property(p => p.IsMobileOnboarded);
        var mobileOnboardingProperty = builder.Property(p => p.MobileOnboardingStep)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        mobileOnboardingProperty.Metadata.SetValueComparer(StructuredDataComparer);
        builder.Property(p => p.MobileTimezoneAutoSet);
        builder.Property(p => p.Language);
        builder.Property(p => p.StartOfTheWeek);
        var goalsProperty = builder.Property(p => p.Goals)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        goalsProperty.Metadata.SetValueComparer(StructuredDataComparer);
        builder.Property(p => p.BackgroundColor)
            .HasConversion(color => color.Value, value => ColorCode.FromHex(value))
            .HasMaxLength(7);
        builder.Property(p => p.HasMarketingEmailConsent);
    }

    private static void ConfigureWorkspaceMemberInvite(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<WorkspaceMemberInvite>();
        builder.ToTable("workspace_member_invites");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();
        builder.Property(i => i.WorkspaceId).IsRequired();

        var emailProperty = builder.Property(i => i.Email)
            .HasConversion(EmailAddressConverter)
            .HasColumnName("email")
            .HasMaxLength(255);
        emailProperty.Metadata.SetValueComparer(EmailAddressComparer);

        builder.Property(i => i.Accepted).HasColumnName("accepted");
        builder.Property(i => i.Token).HasColumnName("token").IsRequired();
        builder.Property(i => i.Message).HasColumnName("message");
        builder.Property(i => i.RespondedAt).HasColumnName("responded_at");
        builder.Property(i => i.Role).HasConversion<int>().HasColumnName("role");

        var workspaceInviteAuditTrail = builder.Property(i => i.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        workspaceInviteAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureProjectMemberInvite(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<ProjectMemberInvite>();
        builder.ToTable("project_member_invites");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();
        builder.Property(i => i.WorkspaceId).IsRequired();
        builder.Property(i => i.ProjectId).IsRequired();

        var emailProperty = builder.Property(i => i.Email)
            .HasConversion(EmailAddressConverter)
            .HasColumnName("email")
            .HasMaxLength(255);
        emailProperty.Metadata.SetValueComparer(EmailAddressComparer);

        builder.Property(i => i.Accepted).HasColumnName("accepted");
        builder.Property(i => i.Token).HasColumnName("token").IsRequired();
        builder.Property(i => i.Message).HasColumnName("message");
        builder.Property(i => i.RespondedAt).HasColumnName("responded_at");
        builder.Property(i => i.Role).HasConversion<int>().HasColumnName("role");

        var projectInviteAuditTrail = builder.Property(i => i.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        projectInviteAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureIssue(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Issue>();
        builder.ToTable("issues");
        builder.HasKey(issue => issue.Id);

        builder.Property(issue => issue.Id).ValueGeneratedNever();
        builder.Property(issue => issue.WorkspaceId).IsRequired();
        builder.Property(issue => issue.ProjectId).IsRequired();
        builder.Property(issue => issue.Name).HasMaxLength(255).IsRequired();
        builder.Property(issue => issue.Priority)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(issue => issue.SequenceId).IsRequired();
        builder.Property(issue => issue.SortOrder).IsRequired();
        builder.Property(issue => issue.PointEstimate);
        builder.Property(issue => issue.StateId);
        builder.Property(issue => issue.ParentId);
        builder.Property(issue => issue.EstimatePointId);
        builder.Property(issue => issue.IssueTypeId);
        builder.Property(issue => issue.CompletedAt);
        builder.Property(issue => issue.ArchivedAt);
        builder.Property(issue => issue.IsDraft).IsRequired();

        var issueDescriptionProperty = builder.Property(issue => issue.Description)
            .HasConversion(RichTextContentConverter)
            .HasColumnType("jsonb");
        issueDescriptionProperty.Metadata.SetValueComparer(RichTextContentComparer);

        var issueScheduleProperty = builder.Property(issue => issue.Schedule)
            .HasConversion(DateRangeConverter)
            .HasColumnType("jsonb");
        issueScheduleProperty.Metadata.SetValueComparer(DateRangeComparer);

        builder.OwnsOne(issue => issue.ExternalReference, navigation =>
        {
            navigation.Property(reference => reference.Source)
                .HasColumnName("external_source")
                .HasMaxLength(255);
            navigation.Property(reference => reference.Identifier)
                .HasColumnName("external_id")
                .HasMaxLength(255);
        });

        var issueAuditTrailProperty = builder.Property(issue => issue.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        issueAuditTrailProperty.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureIssueAssignee(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<IssueAssignee>();
        builder.ToTable("issue_assignees");
        builder.HasKey(assignee => assignee.Id);
        builder.Property(assignee => assignee.Id).ValueGeneratedNever();
        builder.Property(assignee => assignee.WorkspaceId).IsRequired();
        builder.Property(assignee => assignee.ProjectId).IsRequired();
        builder.Property(assignee => assignee.IssueId).IsRequired();
        builder.Property(assignee => assignee.AssigneeId).IsRequired();

        var issueAssigneeAuditTrail = builder.Property(assignee => assignee.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        issueAssigneeAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureIssueLabel(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<IssueLabel>();
        builder.ToTable("issue_labels");
        builder.HasKey(label => label.Id);
        builder.Property(label => label.Id).ValueGeneratedNever();
        builder.Property(label => label.WorkspaceId).IsRequired();
        builder.Property(label => label.ProjectId).IsRequired();
        builder.Property(label => label.IssueId).IsRequired();
        builder.Property(label => label.LabelId).IsRequired();

        var issueLabelAuditTrail = builder.Property(label => label.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        issueLabelAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureIssueComment(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<IssueComment>();
        builder.ToTable("issue_comments");
        builder.HasKey(comment => comment.Id);
        builder.Property(comment => comment.Id).ValueGeneratedNever();
        builder.Property(comment => comment.WorkspaceId).IsRequired();
        builder.Property(comment => comment.ProjectId).IsRequired();
        builder.Property(comment => comment.IssueId).IsRequired();
        builder.Property(comment => comment.ActorId);
        builder.Property(comment => comment.Access)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(comment => comment.CommentStripped)
            .HasColumnName("comment_stripped");
        var commentJsonProperty = builder.Property(comment => comment.CommentJson)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        commentJsonProperty.Metadata.SetValueComparer(StructuredDataComparer);

        var commentRichTextProperty = builder.Property(comment => comment.Comment)
            .HasConversion(RichTextContentConverter)
            .HasColumnType("jsonb");
        commentRichTextProperty.Metadata.SetValueComparer(RichTextContentComparer);

        var attachmentsProperty = builder.Property<List<Url>>("_attachments")
            .HasConversion(UrlListConverter)
            .HasColumnName("attachments")
            .HasColumnType("jsonb");
        attachmentsProperty.Metadata.SetValueComparer(UrlListComparer);
        builder.Property(comment => comment.EditedAt);
        builder.OwnsOne(comment => comment.ExternalReference, navigation =>
        {
            navigation.Property(reference => reference.Source)
                .HasColumnName("external_source")
                .HasMaxLength(255);
            navigation.Property(reference => reference.Identifier)
                .HasColumnName("external_id")
                .HasMaxLength(255);
        });

        var commentAuditTrailProperty = builder.Property(comment => comment.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        commentAuditTrailProperty.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureProject(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Project>();
        builder.ToTable("projects");
        builder.HasKey(project => project.Id);
        builder.Property(project => project.Id).ValueGeneratedNever();
        builder.Property(project => project.WorkspaceId).IsRequired();
        builder.Property(project => project.Name).HasMaxLength(255).IsRequired();
        builder.Property(project => project.Identifier).HasMaxLength(12).IsRequired();
        builder.Property(project => project.Visibility)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();
        builder.Property(project => project.DefaultAssigneeId);
        builder.Property(project => project.ProjectLeadId);
        builder.Property(project => project.ModuleViewEnabled);
        builder.Property(project => project.CycleViewEnabled);
        builder.Property(project => project.IssueViewsEnabled);
        builder.Property(project => project.PageViewEnabled);
        builder.Property(project => project.IntakeViewEnabled);
        builder.Property(project => project.TimeTrackingEnabled);
        builder.Property(project => project.IssueTypeEnabled);
        builder.Property(project => project.GuestViewAllFeatures);
        builder.Property(project => project.ArchiveInMonths);
        builder.Property(project => project.CloseInMonths);
        builder.Property(project => project.CoverImageAssetId).HasColumnName("cover_image_asset_id");
        var coverImageProperty = builder.Property(project => project.CoverImage)
            .HasConversion(UrlConverter)
            .HasColumnName("cover_image");
        coverImageProperty.Metadata.SetValueComparer(UrlComparer);
        builder.Property(project => project.EstimateId);
        builder.Property(project => project.DefaultStateId);
        builder.Property(project => project.ArchivedAt);
        builder.Property(project => project.Timezone).HasMaxLength(255);

        var projectDescriptionProperty = builder.Property(project => project.Description)
            .HasConversion(RichTextContentConverter)
            .HasColumnType("jsonb");
        projectDescriptionProperty.Metadata.SetValueComparer(RichTextContentComparer);

        var projectDescriptionRichTextProperty = builder.Property(project => project.DescriptionRichText)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        projectDescriptionRichTextProperty.Metadata.SetValueComparer(StructuredDataComparer);

        var projectDescriptionHtmlProperty = builder.Property(project => project.DescriptionHtml)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        projectDescriptionHtmlProperty.Metadata.SetValueComparer(StructuredDataComparer);

        var iconPropertiesProperty = builder.Property(project => project.IconProperties)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        iconPropertiesProperty.Metadata.SetValueComparer(StructuredDataComparer);

        var logoPropertiesProperty = builder.Property(project => project.LogoProperties)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        logoPropertiesProperty.Metadata.SetValueComparer(StructuredDataComparer);

        var externalReferenceProperty = builder.Property(project => project.ExternalReference)
            .HasConversion(ExternalReferenceConverter)
            .HasColumnType("jsonb");
        externalReferenceProperty.Metadata.SetValueComparer(ExternalReferenceComparer);

        var projectAuditTrailProperty = builder.Property(project => project.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        projectAuditTrailProperty.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureProjectMember(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<ProjectMember>();
        builder.ToTable("project_members");
        builder.HasKey(member => member.Id);
        builder.Property(member => member.Id).ValueGeneratedNever();
        builder.Property(member => member.WorkspaceId).IsRequired();
        builder.Property(member => member.ProjectId).IsRequired();
        builder.Property(member => member.MemberId).IsRequired();
        builder.Property(member => member.Role).HasConversion<int>();
        builder.Property(member => member.IsActive);
        builder.Property(member => member.SortOrder);
        builder.Property(member => member.Comment);
        var preferencesProperty = builder.Property(member => member.Preferences)
            .HasConversion(ProjectMemberPreferencesConverter)
            .HasColumnType("jsonb");
        preferencesProperty.Metadata.SetValueComparer(ProjectMemberPreferencesComparer);

        var projectMemberAuditTrail = builder.Property(member => member.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        projectMemberAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureState(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<State>();
        builder.ToTable("states");
        builder.HasKey(state => state.Id);
        builder.Property(state => state.Id).ValueGeneratedNever();
        builder.Property(state => state.WorkspaceId).IsRequired();
        builder.Property(state => state.ProjectId).IsRequired();
        builder.Property(state => state.Name).HasMaxLength(255).IsRequired();
        builder.Property(state => state.Color)
            .HasConversion(color => color.Value, value => ColorCode.FromHex(value))
            .HasMaxLength(7)
            .IsRequired();
        builder.Property(state => state.Slug)
            .HasConversion(slug => slug.Value, value => Slug.Create(value))
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(state => state.Sequence).IsRequired();
        builder.Property(state => state.Group).HasMaxLength(64);
        builder.Property(state => state.IsTriage);
        builder.Property(state => state.IsDefault);
        builder.Property(state => state.Description);
        var stateExternalReferenceProperty = builder.Property(state => state.ExternalReference)
            .HasConversion(ExternalReferenceConverter)
            .HasColumnType("jsonb");
        stateExternalReferenceProperty.Metadata.SetValueComparer(ExternalReferenceComparer);

        var stateAuditTrailProperty = builder.Property(state => state.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        stateAuditTrailProperty.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureLabel(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Label>();
        builder.ToTable("labels");
        builder.HasKey(label => label.Id);
        builder.Property(label => label.Id).ValueGeneratedNever();
        builder.Property(label => label.WorkspaceId).IsRequired();
        builder.Property(label => label.Name).HasMaxLength(255).IsRequired();
        builder.Property(label => label.Color)
            .HasConversion(color => color.Value, value => ColorCode.FromHex(value))
            .HasMaxLength(7)
            .IsRequired();
        builder.Property(label => label.Slug)
            .HasConversion(slug => slug.Value, value => Slug.Create(value))
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(label => label.Description);
        var labelExternalReferenceProperty = builder.Property(label => label.ExternalReference)
            .HasConversion(ExternalReferenceConverter)
            .HasColumnType("jsonb");
        labelExternalReferenceProperty.Metadata.SetValueComparer(ExternalReferenceComparer);

        var labelAuditTrailProperty = builder.Property(label => label.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        labelAuditTrailProperty.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureEstimatePoint(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<EstimatePoint>();
        builder.ToTable("estimate_points");
        builder.HasKey(point => point.Id);
        builder.Property(point => point.Id).ValueGeneratedNever();
        builder.Property(point => point.WorkspaceId).IsRequired();
        builder.Property(point => point.ProjectId).IsRequired();
        builder.Property(point => point.Key).HasMaxLength(50).IsRequired();
        builder.Property(point => point.Value).HasColumnType("decimal(18,2)");
        builder.Property(point => point.Description);

        var estimatePointAuditTrail = builder.Property(point => point.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        estimatePointAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureIssueType(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<IssueType>();
        builder.ToTable("issue_types");
        builder.HasKey(type => type.Id);
        builder.Property(type => type.Id).ValueGeneratedNever();
        builder.Property(type => type.WorkspaceId).IsRequired();
        builder.Property(type => type.Name).HasMaxLength(255).IsRequired();
        builder.Property(type => type.Description);
        builder.Property(type => type.Level);
        builder.Property(type => type.IsEpic);
        builder.Property(type => type.IsDefault);
        builder.Property(type => type.IsActive).IsRequired();
        var issueTypeLogoProperty = builder.Property(type => type.LogoProperties)
            .HasConversion(StructuredDataConverter)
            .HasColumnType("jsonb");
        issueTypeLogoProperty.Metadata.SetValueComparer(StructuredDataComparer);

        var issueTypeExternalReferenceProperty = builder.Property(type => type.ExternalReference)
            .HasConversion(ExternalReferenceConverter)
            .HasColumnType("jsonb");
        issueTypeExternalReferenceProperty.Metadata.SetValueComparer(ExternalReferenceComparer);

        var issueTypeAuditTrailProperty = builder.Property(type => type.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        issueTypeAuditTrailProperty.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureProjectIssueType(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<ProjectIssueType>();
        builder.ToTable("project_issue_types");
        builder.HasKey(type => type.Id);
        builder.Property(type => type.Id).ValueGeneratedNever();
        builder.Property(type => type.WorkspaceId).IsRequired();
        builder.Property(type => type.ProjectId).IsRequired();
        builder.Property(type => type.IssueTypeId).IsRequired();
        builder.Property(type => type.Level).IsRequired();
        builder.Property(type => type.IsDefault).IsRequired();

        var projectIssueTypeAuditTrailProperty = builder.Property(type => type.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        projectIssueTypeAuditTrailProperty.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureIssueUserProperty(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<IssueUserProperty>();
        builder.ToTable("issue_user_properties");
        builder.HasKey(property => property.Id);
        builder.Property(property => property.Id).ValueGeneratedNever();
        builder.Property(property => property.WorkspaceId).IsRequired();
        builder.Property(property => property.ProjectId).IsRequired();
        builder.Property(property => property.UserId).IsRequired();

        var preferencesProperty = builder.Property(property => property.Preferences)
            .HasConversion(ViewPreferencesConverter)
            .HasColumnType("jsonb");
        preferencesProperty.Metadata.SetValueComparer(ViewPreferencesComparer);

        var issueUserPropertyAuditTrail = builder.Property(property => property.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        issueUserPropertyAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureIssueVersion(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<IssueVersion>();
        builder.ToTable("issue_versions");
        builder.HasKey(version => version.Id);
        builder.Property(version => version.Id).ValueGeneratedNever();
        builder.Property(version => version.WorkspaceId).IsRequired();
        builder.Property(version => version.ProjectId).IsRequired();
        builder.Property(version => version.IssueId).IsRequired();
        builder.Property(version => version.Name).HasMaxLength(255).IsRequired();
        builder.Property(version => version.Priority)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();
        builder.Property(version => version.SequenceId).IsRequired();
        builder.Property(version => version.SortOrder).IsRequired();
        builder.Property(version => version.IsDraft).IsRequired();
        builder.Property(version => version.RecordedAt).IsRequired();
        builder.Property(version => version.OwnerId).IsRequired();
        builder.Property(version => version.ParentId);
        builder.Property(version => version.StateId);
        builder.Property(version => version.EstimatePointId);
        builder.Property(version => version.StartDate);
        builder.Property(version => version.TargetDate);
        builder.Property(version => version.CompletedAt);
        builder.Property(version => version.ArchivedAt);
        builder.Property(version => version.IssueTypeId);
        builder.Property(version => version.CycleId);
        builder.Property(version => version.ActivityId);

        var assigneesProperty = builder.Property<HashSet<Guid>>("_assigneeIds")
            .HasColumnName("assignees")
            .HasColumnType("uuid[]")
            .HasConversion(GuidSetConverter);
        assigneesProperty.Metadata.SetValueComparer(GuidSetComparer);

        var labelsProperty = builder.Property<HashSet<Guid>>("_labelIds")
            .HasColumnName("labels")
            .HasColumnType("uuid[]")
            .HasConversion(GuidSetConverter);
        labelsProperty.Metadata.SetValueComparer(GuidSetComparer);

        var modulesProperty = builder.Property<HashSet<Guid>>("_moduleIds")
            .HasColumnName("modules")
            .HasColumnType("uuid[]")
            .HasConversion(GuidSetConverter);
        modulesProperty.Metadata.SetValueComparer(GuidSetComparer);

        builder.OwnsOne(version => version.ExternalReference, navigation =>
        {
            navigation.Property(reference => reference.Source)
                .HasColumnName("external_source")
                .HasMaxLength(255);
            navigation.Property(reference => reference.Identifier)
                .HasColumnName("external_id")
                .HasMaxLength(255);
        });

        var propertiesProperty = builder.Property(version => version.Properties)
            .HasConversion(StructuredDataConverter)
            .HasColumnName("properties")
            .HasColumnType("jsonb");
        propertiesProperty.Metadata.SetValueComparer(StructuredDataComparer);

        var metadataProperty = builder.Property(version => version.Metadata)
            .HasConversion(StructuredDataConverter)
            .HasColumnName("meta")
            .HasColumnType("jsonb");
        metadataProperty.Metadata.SetValueComparer(StructuredDataComparer);

        var issueVersionAuditTrail = builder.Property(version => version.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        issueVersionAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);

        builder.HasIndex(version => new { version.ProjectId, version.IssueId, version.SequenceId })
            .HasDatabaseName("ix_issue_versions_project_issue_sequence");
    }

    private static void ConfigureIssueDescriptionVersion(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<IssueDescriptionVersion>();
        builder.ToTable("issue_description_versions");
        builder.HasKey(version => version.Id);
        builder.Property(version => version.Id).ValueGeneratedNever();
        builder.Property(version => version.WorkspaceId).IsRequired();
        builder.Property(version => version.ProjectId).IsRequired();
        builder.Property(version => version.IssueId).IsRequired();
        builder.Property(version => version.OwnerId).IsRequired();
        builder.Property(version => version.RecordedAt).IsRequired();

        var descriptionProperty = builder.Property(version => version.Description)
            .HasConversion(RichTextContentConverter)
            .HasColumnName("description")
            .HasColumnType("jsonb");
        descriptionProperty.Metadata.SetValueComparer(RichTextContentComparer);

        var issueDescriptionAuditTrail = builder.Property(version => version.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        issueDescriptionAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);

        builder.HasIndex(version => new { version.ProjectId, version.IssueId, version.RecordedAt })
            .HasDatabaseName("ix_issue_description_versions_timeline");
    }

    private static void ConfigureWorkspace(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Workspace>();
        builder.ToTable("workspaces");
        builder.HasKey(workspace => workspace.Id);
        builder.Property(workspace => workspace.Id).ValueGeneratedNever();
        builder.Property(workspace => workspace.Name).HasMaxLength(255).IsRequired();
        builder.Property(workspace => workspace.OwnerId).IsRequired();

        builder.Property(workspace => workspace.Slug)
            .HasConversion(slug => slug.Value, value => Slug.Create(value))
            .HasMaxLength(255)
            .IsRequired();

        var logoProperty = builder.Property(workspace => workspace.Logo)
            .HasConversion(UrlConverter)
            .HasColumnName("logo");
        logoProperty.Metadata.SetValueComparer(UrlComparer);

        builder.Property(workspace => workspace.LogoAssetId).HasColumnName("logo_asset_id");
        builder.Property(workspace => workspace.OrganizationSize).HasColumnName("organization_size");
        builder.Property(workspace => workspace.Timezone).HasMaxLength(255);

        builder.Property(workspace => workspace.BackgroundColor)
            .HasConversion(color => color == null ? null : color.Value, value => value == null ? null : ColorCode.FromHex(value))
            .HasColumnName("background_color");

        var workspaceAuditTrailProperty = builder.Property(workspace => workspace.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        workspaceAuditTrailProperty.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureWorkspaceMember(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<WorkspaceMember>();
        builder.ToTable("workspace_members");
        builder.HasKey(member => member.Id);
        builder.Property(member => member.Id).ValueGeneratedNever();
        builder.Property(member => member.WorkspaceId).IsRequired();
        builder.Property(member => member.MemberId).IsRequired();
        builder.Property(member => member.Role).HasConversion<int>();
        builder.Property(member => member.CompanyRole);
        builder.Property(member => member.IsActive);

        var preferencesProperty = builder.Property(member => member.Preferences)
            .HasConversion(WorkspaceMemberPreferencesConverter)
            .HasColumnType("jsonb");
        preferencesProperty.Metadata.SetValueComparer(WorkspaceMemberPreferencesComparer);

        var workspaceMemberAuditTrail = builder.Property(member => member.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        workspaceMemberAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static void ConfigureProjectIdentifier(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<ProjectIdentifier>();
        builder.ToTable("project_identifiers");
        builder.HasKey(identifier => identifier.Id);
        builder.Property(identifier => identifier.Id).ValueGeneratedNever();
        builder.Property(identifier => identifier.WorkspaceId).IsRequired();
        builder.Property(identifier => identifier.ProjectId).IsRequired();
        builder.Property(identifier => identifier.Name).HasMaxLength(255).IsRequired();

        var projectIdentifierAuditTrail = builder.Property(identifier => identifier.AuditTrail)
            .HasConversion(AuditTrailConverter)
            .HasColumnType("jsonb");
        projectIdentifierAuditTrail.Metadata.SetValueComparer(AuditTrailComparer);
    }

    private static RichTextContent DeserializeRichText(string value)
    {
        var data = JsonSerializer.Deserialize<RichTextContentSurrogate>(value, JsonOptions);
        if (data == null)
        {
            return RichTextContent.Create();
        }

        var binary = string.IsNullOrWhiteSpace(data.Binary) ? null : Convert.FromBase64String(data.Binary);
        return RichTextContent.Create(data.PlainText, data.Html, binary, data.Json);
    }

    private static DateRange DeserializeDateRange(string value)
    {
        var data = JsonSerializer.Deserialize<DateRangeSurrogate>(value, JsonOptions);
        return data == null ? DateRange.Create(null, null) : DateRange.Create(data.Start, data.End);
    }

    private static ExternalReference? DeserializeExternalReference(string value)
    {
        var data = JsonSerializer.Deserialize<ExternalReferenceSurrogate>(value, JsonOptions);
        return data == null ? null : ExternalReference.Create(data.Source, data.Identifier);
    }

    private static ProjectMemberPreferences DeserializeProjectMemberPreferences(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ProjectMemberPreferences.CreateDefault();
        }

        var data = JsonSerializer.Deserialize<ProjectMemberPreferencesSurrogate>(value!, JsonOptions);
        if (data == null)
        {
            return ProjectMemberPreferences.CreateDefault();
        }
        return ProjectMemberPreferences.Create(
            StructuredData.FromJson(data.View),
            StructuredData.FromJson(data.Defaults),
            StructuredData.FromJson(data.Preferences));
    }

    private static WorkspaceMemberPreferences DeserializeWorkspaceMemberPreferences(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return WorkspaceMemberPreferences.CreateDefault();
        }

        var data = JsonSerializer.Deserialize<WorkspaceMemberPreferencesSurrogate>(value!, JsonOptions);
        if (data == null)
        {
            return WorkspaceMemberPreferences.CreateDefault();
        }

        return WorkspaceMemberPreferences.Create(
            StructuredData.FromJson(data.View),
            StructuredData.FromJson(data.Defaults),
            StructuredData.FromJson(data.Issue));
    }

    private static ViewPreferences DeserializeViewPreferences(string value)
    {
        var data = JsonSerializer.Deserialize<ViewPreferencesSurrogate>(value, JsonOptions);
        return data == null
            ? ViewPreferences.CreateIssueDefaults()
            : ViewPreferences.Create(data.Filters, data.DisplayFilters, data.DisplayProperties, data.RichFilters);
    }

    private static AuditTrail DeserializeAuditTrail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return AuditTrail.Empty;
        }

        var data = JsonSerializer.Deserialize<AuditTrailSurrogate>(value!, JsonOptions);
        if (data == null)
        {
            return AuditTrail.Empty;
        }

        return AuditTrail.Create(data.CreatedAt, data.CreatedById, data.UpdatedAt, data.UpdatedById, data.DeletedAt);
    }

    private sealed record RichTextContentSurrogate(string? PlainText, string? Html, string? Json, string? Binary);

    private sealed record DateRangeSurrogate(DateTime? Start, DateTime? End);

    private sealed record ExternalReferenceSurrogate(string? Source, string? Identifier);

    private sealed record AuditTrailSurrogate(DateTime CreatedAt, Guid? CreatedById, DateTime? UpdatedAt, Guid? UpdatedById, DateTime? DeletedAt);

    private sealed record ProjectMemberPreferencesSurrogate(string? View, string? Defaults, string? Preferences);

    private sealed record WorkspaceMemberPreferencesSurrogate(string? View, string? Defaults, string? Issue);

    private sealed record ViewPreferencesSurrogate(string? Filters, string? DisplayFilters, string? DisplayProperties, string? RichFilters);
}
