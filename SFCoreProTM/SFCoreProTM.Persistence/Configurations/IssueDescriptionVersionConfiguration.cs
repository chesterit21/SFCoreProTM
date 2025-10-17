using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class IssueDescriptionVersionConfiguration : IEntityTypeConfiguration<IssueDescriptionVersion>
    {
        public void Configure(EntityTypeBuilder<IssueDescriptionVersion> builder)
        {
            builder.ToTable("issue_description_versions");
            builder.HasKey(version => version.Id);
            builder.Property(version => version.Id).ValueGeneratedNever();
            builder.Property(version => version.WorkspaceId).IsRequired();
            builder.Property(version => version.ProjectId).IsRequired();
            builder.Property(version => version.IssueId).IsRequired();
            builder.Property(version => version.OwnerId).IsRequired();
            builder.Property(version => version.RecordedAt).IsRequired();

            builder.Property(version => version.Description)
                .HasConversion(ValueConverters.RichTextContentConverter)
                .HasColumnName("description")
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.RichTextContentComparer);

            builder.Property(version => version.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);

            builder.HasIndex(version => new { version.ProjectId, version.IssueId, version.RecordedAt })
                .HasDatabaseName("ix_issue_description_versions_timeline");
        }
    }
}
