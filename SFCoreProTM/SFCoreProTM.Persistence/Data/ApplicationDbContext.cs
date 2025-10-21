using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Domain.Entities.Users;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Domain.ValueObjects;
using TaskEntity = SFCoreProTM.Domain.Entities.Projects.Task;

namespace SFCoreProTM.Persistence.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<ErdDefinition> ErdDefinitions => Set<ErdDefinition>();
    public DbSet<AttributeEntitas> AttributeEntitas => Set<AttributeEntitas>();
    public DbSet<SprintPlanning> SprintPlannings => Set<SprintPlanning>();
    public DbSet<FlowTask> FlowTasks => Set<FlowTask>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

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
        //modelBuilder.Ignore<ProjectMemberPreferences>();

        base.OnModelCreating(modelBuilder);
    }
}
