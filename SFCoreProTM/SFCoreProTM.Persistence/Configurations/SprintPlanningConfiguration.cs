using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Persistence.Configurations;

public class SprintPlanningConfiguration : IEntityTypeConfiguration<SprintPlanning>
{
    public void Configure(EntityTypeBuilder<SprintPlanning> builder)
    {
        builder.ToTable("sprint_plannings");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.WorkspaceId).IsRequired();
        builder.Property(s => s.ProjectId).IsRequired();
        builder.Property(s => s.ModuleId).IsRequired();
        builder.Property(s => s.TaskId).IsRequired();
        builder.Property(s => s.Name).HasMaxLength(255).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(1000).IsRequired();
        builder.Property(s => s.StartDate).IsRequired();
        builder.Property(s => s.TargetDate).IsRequired();
        builder.Property(s => s.SortOrder).IsRequired();
        builder.Property(s => s.SprintStatus).IsRequired();
        builder.Property(s => s.Note).HasMaxLength(2000).IsRequired();
    }
}