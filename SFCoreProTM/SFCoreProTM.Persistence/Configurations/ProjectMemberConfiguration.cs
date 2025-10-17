using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {
            builder.ToTable("project_members");
            builder.HasKey(member => member.Id);
            builder.Property(member => member.Id).ValueGeneratedNever();
            builder.Property(member => member.WorkspaceId).IsRequired();
            builder.Property(member => member.ProjectId).IsRequired();
            builder.Property(member => member.MemberId).IsRequired();
            builder.Property(member => member.Role).HasConversion<int>();
            builder.Property(member => member.IsActive);
            builder.Property(member => member.SortOrder);
            builder.Property(member => member.Comment);
            var preferencesProperty = builder.Property(member => member.Preferences)
                .HasConversion(ValueConverters.ProjectMemberPreferencesConverter)
                .HasColumnType("jsonb");
            preferencesProperty.Metadata.SetValueComparer(ValueConverters.ProjectMemberPreferencesComparer);

            var projectMemberAuditTrail = builder.Property(member => member.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb");
            projectMemberAuditTrail.Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
