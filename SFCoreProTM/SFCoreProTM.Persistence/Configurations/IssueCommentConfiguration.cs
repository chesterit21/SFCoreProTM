using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.ValueObjects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class IssueCommentConfiguration : IEntityTypeConfiguration<IssueComment>
    {
        public void Configure(EntityTypeBuilder<IssueComment> builder)
        {
            builder.ToTable("issue_comments");
            builder.HasKey(comment => comment.Id);
            builder.Property(comment => comment.Id).ValueGeneratedNever();
            builder.Property(comment => comment.WorkspaceId).IsRequired();
            builder.Property(comment => comment.ProjectId).IsRequired();
            builder.Property(comment => comment.IssueId).IsRequired();
            builder.Property(comment => comment.ActorId);
            builder.Property(comment => comment.Access)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();
            builder.Property(comment => comment.CommentStripped)
                .HasColumnName("comment_stripped");
            var commentJsonProperty = builder.Property(comment => comment.CommentJson)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb");
            commentJsonProperty.Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);

            var commentRichTextProperty = builder.Property(comment => comment.Comment)
                .HasConversion(ValueConverters.RichTextContentConverter)
                .HasColumnType("jsonb");
            commentRichTextProperty.Metadata.SetValueComparer(ValueConverters.RichTextContentComparer);

            var attachmentsProperty = builder.Property<List<Url>>("_attachments")
                .HasConversion(ValueConverters.UrlListConverter)
                .HasColumnName("attachments")
                .HasColumnType("jsonb");
            attachmentsProperty.Metadata.SetValueComparer(ValueConverters.UrlListComparer);
            builder.Property(comment => comment.EditedAt);
            builder.OwnsOne(comment => comment.ExternalReference, navigation =>
            {
                navigation.Property(reference => reference.Source)
                    .HasColumnName("external_source")
                    .HasMaxLength(255);
                navigation.Property(reference => reference.Identifier)
                    .HasColumnName("external_id")
                    .HasMaxLength(255);
            });

            var commentAuditTrailProperty = builder.Property(comment => comment.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb");
            commentAuditTrailProperty.Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
