using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Persistence.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.ToTable("modules");
            builder.HasKey(module => module.Id);
            builder.Property(module => module.Id).ValueGeneratedNever();
            builder.Property(module => module.WorkspaceId).IsRequired();
            builder.Property(module => module.ProjectId).IsRequired();
            builder.Property(module => module.Name).HasMaxLength(255).IsRequired();
            builder.Property(module => module.Description).IsRequired();
            builder.Property(module => module.SortOrder).IsRequired();
            builder.Property(module => module.Status).IsRequired();
        }
    }
}