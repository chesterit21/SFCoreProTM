using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Persistence.Configurations;

public class FlowTaskConfiguration : IEntityTypeConfiguration<FlowTask>
{
    public void Configure(EntityTypeBuilder<FlowTask> builder)
    {
        builder.ToTable("flow_tasks");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedNever();
        builder.Property(f => f.WorkspaceId).IsRequired();
        builder.Property(f => f.ProjectId).IsRequired();
        builder.Property(f => f.ModuleId).IsRequired();
        builder.Property(f => f.TaskId).IsRequired();
        builder.Property(f => f.Name).HasMaxLength(255).IsRequired();
        builder.Property(f => f.Description).HasMaxLength(1000).IsRequired();
        builder.Property(f => f.SortOrder).IsRequired();
        builder.Property(f => f.FlowStatus).IsRequired();
    }
}