using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class IssueConfiguration : IEntityTypeConfiguration<Issue>
    {
        public void Configure(EntityTypeBuilder<Issue> builder)
        {
            builder.ToTable("issues");
            builder.HasKey(issue => issue.Id);

            builder.Property(issue => issue.Id).ValueGeneratedNever();
            builder.Property(issue => issue.WorkspaceId).IsRequired();
            builder.Property(issue => issue.ProjectId).IsRequired();
            builder.Property(issue => issue.Name).HasMaxLength(255).IsRequired();
            builder.Property(issue => issue.Priority)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(issue => issue.SequenceId).IsRequired();
            builder.Property(issue => issue.SortOrder).IsRequired();
            builder.Property(issue => issue.PointEstimate);
            builder.Property(issue => issue.StateId);
            builder.Property(issue => issue.ParentId);
            builder.Property(issue => issue.EstimatePointId);
            builder.Property(issue => issue.IssueTypeId);
            builder.Property(issue => issue.CompletedAt);
            builder.Property(issue => issue.ArchivedAt);
            builder.Property(issue => issue.IsDraft).IsRequired();

            var issueDescriptionProperty = builder.Property(issue => issue.Description)
                .HasConversion(ValueConverters.RichTextContentConverter)
                .HasColumnType("jsonb");
            issueDescriptionProperty.Metadata.SetValueComparer(ValueConverters.RichTextContentComparer);

            var issueScheduleProperty = builder.Property(issue => issue.Schedule)
                .HasConversion(ValueConverters.DateRangeConverter)
                .HasColumnType("jsonb");
            issueScheduleProperty.Metadata.SetValueComparer(ValueConverters.DateRangeComparer);

            builder.OwnsOne(issue => issue.ExternalReference, navigation =>
            {
                navigation.Property(reference => reference.Source)
                    .HasColumnName("external_source")
                    .HasMaxLength(255);
                navigation.Property(reference => reference.Identifier)
                    .HasColumnName("external_id")
                    .HasMaxLength(255);
            });

            var issueAuditTrailProperty = builder.Property(issue => issue.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb");
            issueAuditTrailProperty.Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
