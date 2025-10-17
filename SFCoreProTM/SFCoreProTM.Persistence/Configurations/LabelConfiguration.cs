using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Domain.ValueObjects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class LabelConfiguration : IEntityTypeConfiguration<Label>
    {
        public void Configure(EntityTypeBuilder<Label> builder)
        {
            builder.ToTable("labels");
            builder.HasKey(label => label.Id);
            builder.Property(label => label.Id).ValueGeneratedNever();
            builder.Property(label => label.WorkspaceId).IsRequired();
            builder.Property(label => label.Name).HasMaxLength(255).IsRequired();
            builder.Property(label => label.Color)
                .HasConversion(color => color.Value, value => ColorCode.FromHex(value))
                .HasMaxLength(7)
                .IsRequired();
            builder.Property(label => label.Slug)
                .HasConversion(slug => slug.Value, value => Slug.Create(value))
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(label => label.Description);

            builder.Property(label => label.ExternalReference)
                .HasConversion(ValueConverters.ExternalReferenceConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.ExternalReferenceComparer);

            builder.Property(label => label.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
