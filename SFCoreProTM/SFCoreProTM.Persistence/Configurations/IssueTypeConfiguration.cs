using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class IssueTypeConfiguration : IEntityTypeConfiguration<IssueType>
    {
        public void Configure(EntityTypeBuilder<IssueType> builder)
        {
            builder.ToTable("issue_types");
            builder.HasKey(type => type.Id);
            builder.Property(type => type.Id).ValueGeneratedNever();
            builder.Property(type => type.WorkspaceId).IsRequired();
            builder.Property(type => type.Name).HasMaxLength(255).IsRequired();
            builder.Property(type => type.Description);
            builder.Property(type => type.Level);
            builder.Property(type => type.IsEpic);
            builder.Property(type => type.IsDefault);
            builder.Property(type => type.IsActive).IsRequired();

            builder.Property(type => type.LogoProperties)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);

            builder.Property(type => type.ExternalReference)
                .HasConversion(ValueConverters.ExternalReferenceConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.ExternalReferenceComparer);

            builder.Property(type => type.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
