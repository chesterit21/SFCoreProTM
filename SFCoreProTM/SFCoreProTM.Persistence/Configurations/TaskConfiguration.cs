using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskEntity = SFCoreProTM.Domain.Entities.Projects.Task;

namespace SFCoreProTM.Persistence.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.ToTable("tasks");
            builder.HasKey(task => task.Id);
            builder.Property(task => task.Id).ValueGeneratedNever();
            builder.Property(task => task.WorkspaceId).IsRequired();
            builder.Property(task => task.ProjectId).IsRequired();
            builder.Property(task => task.ModuleId).IsRequired();
            builder.Property(task => task.Name).HasMaxLength(255).IsRequired();
            builder.Property(task => task.Description).IsRequired();
            builder.Property(task => task.SortOrder).IsRequired();
            builder.Property(task => task.Status).IsRequired();
            builder.Property(task => task.IsErd).IsRequired();
        }
    }
}