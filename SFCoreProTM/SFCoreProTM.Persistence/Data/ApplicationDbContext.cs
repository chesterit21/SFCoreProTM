using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Domain.Entities.Users;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Persistence.Data;

public class ApplicationDbContext : DbContext
{
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

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

            auditTrailProperty.SetValueConverter(ValueConverters.AuditTrailConverter);
            auditTrailProperty.SetValueComparer(ValueConverters.AuditTrailComparer);
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
}
