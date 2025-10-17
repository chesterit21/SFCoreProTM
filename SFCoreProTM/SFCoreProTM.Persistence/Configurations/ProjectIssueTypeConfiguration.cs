using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class ProjectIssueTypeConfiguration : IEntityTypeConfiguration<Domain.Entities.Issues.ProjectIssueType>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Issues.ProjectIssueType> builder)
        {
            builder.ToTable("project_issue_types");
            builder.HasKey(type => type.Id);
            builder.Property(type => type.Id).ValueGeneratedNever();
            builder.Property(type => type.WorkspaceId).IsRequired();
            builder.Property(type => type.ProjectId).IsRequired();
            builder.Property(type => type.IssueTypeId).IsRequired();
            builder.Property(type => type.Level).IsRequired();
            builder.Property(type => type.IsDefault).IsRequired();

            builder.Property(type => type.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
