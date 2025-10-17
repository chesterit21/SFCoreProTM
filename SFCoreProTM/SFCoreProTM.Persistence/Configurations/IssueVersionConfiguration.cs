using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class IssueVersionConfiguration : IEntityTypeConfiguration<IssueVersion>
    {
        public void Configure(EntityTypeBuilder<IssueVersion> builder)
        {
            builder.ToTable("issue_versions");
            builder.HasKey(version => version.Id);
            builder.Property(version => version.Id).ValueGeneratedNever();
            builder.Property(version => version.WorkspaceId).IsRequired();
            builder.Property(version => version.ProjectId).IsRequired();
            builder.Property(version => version.IssueId).IsRequired();
            builder.Property(version => version.Name).HasMaxLength(255).IsRequired();
            builder.Property(version => version.Priority)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
            builder.Property(version => version.SequenceId).IsRequired();
            builder.Property(version => version.SortOrder).IsRequired();
            builder.Property(version => version.IsDraft).IsRequired();
            builder.Property(version => version.RecordedAt).IsRequired();
            builder.Property(version => version.OwnerId).IsRequired();
            builder.Property(version => version.ParentId);
            builder.Property(version => version.StateId);
            builder.Property(version => version.EstimatePointId);
            builder.Property(version => version.StartDate);
            builder.Property(version => version.TargetDate);
            builder.Property(version => version.CompletedAt);
            builder.Property(version => version.ArchivedAt);
            builder.Property(version => version.IssueTypeId);
            builder.Property(version => version.CycleId);
            builder.Property(version => version.ActivityId);

            builder.Property<HashSet<Guid>>("_assigneeIds")
                .HasColumnName("assignees")
                .HasColumnType("uuid[]")
                .HasConversion(ValueConverters.GuidSetConverter)
                .Metadata.SetValueComparer(ValueConverters.GuidSetComparer);

            builder.Property<HashSet<Guid>>("_labelIds")
                .HasColumnName("labels")
                .HasColumnType("uuid[]")
                .HasConversion(ValueConverters.GuidSetConverter)
                .Metadata.SetValueComparer(ValueConverters.GuidSetComparer);

            builder.Property<HashSet<Guid>>("_moduleIds")
                .HasColumnName("modules")
                .HasColumnType("uuid[]")
                .HasConversion(ValueConverters.GuidSetConverter)
                .Metadata.SetValueComparer(ValueConverters.GuidSetComparer);

            builder.OwnsOne(version => version.ExternalReference, navigation =>
            {
                navigation.Property(reference => reference.Source)
                    .HasColumnName("external_source")
                    .HasMaxLength(255);
                navigation.Property(reference => reference.Identifier)
                    .HasColumnName("external_id")
                    .HasMaxLength(255);
            });

            builder.Property(version => version.Properties)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnName("properties")
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);

            builder.Property(version => version.Metadata)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnName("meta")
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);

            builder.Property(version => version.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);

            builder.HasIndex(version => new { version.ProjectId, version.IssueId, version.SequenceId })
                .HasDatabaseName("ix_issue_versions_project_issue_sequence");
        }
    }
}
