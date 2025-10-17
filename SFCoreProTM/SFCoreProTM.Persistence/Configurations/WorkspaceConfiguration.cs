using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Domain.ValueObjects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(EntityTypeBuilder<Workspace> builder)
        {
            builder.ToTable("workspaces");
            builder.HasKey(workspace => workspace.Id);
            builder.Property(workspace => workspace.Id).ValueGeneratedNever();
            builder.Property(workspace => workspace.Name).HasMaxLength(255).IsRequired();
            builder.Property(workspace => workspace.OwnerId).IsRequired();

            builder.Property(workspace => workspace.Slug)
                .HasConversion(slug => slug.Value, value => Slug.Create(value))
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(workspace => workspace.Logo)
                .HasConversion(ValueConverters.UrlConverter)
                .HasColumnName("logo")
                .Metadata.SetValueComparer(ValueConverters.UrlComparer);

            builder.Property(workspace => workspace.LogoAssetId).HasColumnName("logo_asset_id");
            builder.Property(workspace => workspace.OrganizationSize).HasColumnName("organization_size");
            builder.Property(workspace => workspace.Timezone).HasMaxLength(255);

            builder.Property(workspace => workspace.BackgroundColor)
                .HasConversion(color => color == null ? null : color.Value, value => value == null ? null : ColorCode.FromHex(value))
                .HasColumnName("background_color");

            builder.Property(workspace => workspace.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
