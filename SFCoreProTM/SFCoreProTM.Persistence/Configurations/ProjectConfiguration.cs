using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.WorkspaceId).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(255).IsRequired();
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.ProjectPath).HasMaxLength(500).IsRequired();
        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}