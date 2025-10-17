using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Domain.ValueObjects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class StateConfiguration : IEntityTypeConfiguration<State>
    {
        public void Configure(EntityTypeBuilder<State> builder)
        {
            builder.ToTable("states");
            builder.HasKey(state => state.Id);
            builder.Property(state => state.Id).ValueGeneratedNever();
            builder.Property(state => state.WorkspaceId).IsRequired();
            builder.Property(state => state.ProjectId).IsRequired();
            builder.Property(state => state.Name).HasMaxLength(255).IsRequired();
            builder.Property(state => state.Color)
                .HasConversion(color => color.Value, value => ColorCode.FromHex(value))
                .HasMaxLength(7)
                .IsRequired();
            builder.Property(state => state.Slug)
                .HasConversion(slug => slug.Value, value => Slug.Create(value))
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(state => state.Sequence).IsRequired();
            builder.Property(state => state.Group).HasMaxLength(64);
            builder.Property(state => state.IsTriage);
            builder.Property(state => state.IsDefault);
            builder.Property(state => state.Description);

            builder.Property(state => state.ExternalReference)
                .HasConversion(ValueConverters.ExternalReferenceConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.ExternalReferenceComparer);

            builder.Property(state => state.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
