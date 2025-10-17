using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class EstimatePointConfiguration : IEntityTypeConfiguration<EstimatePoint>
    {
        public void Configure(EntityTypeBuilder<EstimatePoint> builder)
        {
            builder.ToTable("estimate_points");
            builder.HasKey(point => point.Id);
            builder.Property(point => point.Id).ValueGeneratedNever();
            builder.Property(point => point.WorkspaceId).IsRequired();
            builder.Property(point => point.ProjectId).IsRequired();
            builder.Property(point => point.Key).HasMaxLength(50).IsRequired();
            builder.Property(point => point.Value).HasColumnType("decimal(18,2)");
            builder.Property(point => point.Description);

            builder.Property(point => point.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
