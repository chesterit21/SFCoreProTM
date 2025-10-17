using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class ProjectIdentifierConfiguration : IEntityTypeConfiguration<ProjectIdentifier>
    {
        public void Configure(EntityTypeBuilder<ProjectIdentifier> builder)
        {
            builder.ToTable("project_identifiers");
            builder.HasKey(identifier => identifier.Id);
            builder.Property(identifier => identifier.Id).ValueGeneratedNever();
            builder.Property(identifier => identifier.WorkspaceId).IsRequired();
            builder.Property(identifier => identifier.ProjectId).IsRequired();
            builder.Property(identifier => identifier.Name).HasMaxLength(255).IsRequired();

            builder.Property(identifier => identifier.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
