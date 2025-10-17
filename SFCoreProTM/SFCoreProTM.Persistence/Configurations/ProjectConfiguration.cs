using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("projects");
            builder.HasKey(project => project.Id);
            builder.Property(project => project.Id).ValueGeneratedNever();
            builder.Property(project => project.WorkspaceId).IsRequired();
            builder.Property(project => project.Name).HasMaxLength(255).IsRequired();
            builder.Property(project => project.Identifier).HasMaxLength(12).IsRequired();
            builder.Property(project => project.Visibility)
                .HasConversion<string>()
                .HasMaxLength(16)
                .IsRequired();
            builder.Property(project => project.DefaultAssigneeId);
            builder.Property(project => project.ProjectLeadId);
            builder.Property(project => project.ModuleViewEnabled);
            builder.Property(project => project.CycleViewEnabled);
            builder.Property(project => project.IssueViewsEnabled);
            builder.Property(project => project.PageViewEnabled);
            builder.Property(project => project.IntakeViewEnabled);
            builder.Property(project => project.TimeTrackingEnabled);
            builder.Property(project => project.IssueTypeEnabled);
            builder.Property(project => project.GuestViewAllFeatures);
            builder.Property(project => project.ArchiveInMonths);
            builder.Property(project => project.CloseInMonths);
            builder.Property(project => project.CoverImageAssetId).HasColumnName("cover_image_asset_id");
            var coverImageProperty = builder.Property(project => project.CoverImage)
                .HasConversion(ValueConverters.UrlConverter)
                .HasColumnName("cover_image");
            coverImageProperty.Metadata.SetValueComparer(ValueConverters.UrlComparer);
            builder.Property(project => project.EstimateId);
            builder.Property(project => project.DefaultStateId);
            builder.Property(project => project.ArchivedAt);
            builder.Property(project => project.Timezone).HasMaxLength(255);

            var projectDescriptionProperty = builder.Property(project => project.Description)
                .HasConversion(ValueConverters.RichTextContentConverter)
                .HasColumnType("jsonb");
            projectDescriptionProperty.Metadata.SetValueComparer(ValueConverters.RichTextContentComparer);

            var projectDescriptionRichTextProperty = builder.Property(project => project.DescriptionRichText)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb");
            projectDescriptionRichTextProperty.Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);

            var projectDescriptionHtmlProperty = builder.Property(project => project.DescriptionHtml)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb");
            projectDescriptionHtmlProperty.Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);

            var iconPropertiesProperty = builder.Property(project => project.IconProperties)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb");
            iconPropertiesProperty.Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);

            var logoPropertiesProperty = builder.Property(project => project.LogoProperties)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb");
            logoPropertiesProperty.Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);

            var externalReferenceProperty = builder.Property(project => project.ExternalReference)
                .HasConversion(ValueConverters.ExternalReferenceConverter)
                .HasColumnType("jsonb");
            externalReferenceProperty.Metadata.SetValueComparer(ValueConverters.ExternalReferenceComparer);

            var projectAuditTrailProperty = builder.Property(project => project.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb");
            projectAuditTrailProperty.Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
