using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class IssueUserPropertyConfiguration : IEntityTypeConfiguration<IssueUserProperty>
    {
        public void Configure(EntityTypeBuilder<IssueUserProperty> builder)
        {
            builder.ToTable("issue_user_properties");
            builder.HasKey(property => property.Id);
            builder.Property(property => property.Id).ValueGeneratedNever();
            builder.Property(property => property.WorkspaceId).IsRequired();
            builder.Property(property => property.ProjectId).IsRequired();
            builder.Property(property => property.UserId).IsRequired();

            builder.Property(property => property.Preferences)
                .HasConversion(ValueConverters.ViewPreferencesConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.ViewPreferencesComparer);

            builder.Property(property => property.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
