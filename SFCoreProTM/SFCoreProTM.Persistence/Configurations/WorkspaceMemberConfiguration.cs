using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
    {
        public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
        {
            builder.ToTable("workspace_members");
            builder.HasKey(member => member.Id);
            builder.Property(member => member.Id).ValueGeneratedNever();
            builder.Property(member => member.WorkspaceId).IsRequired();
            builder.Property(member => member.MemberId).IsRequired();
            builder.Property(member => member.Role).HasConversion<int>();
            builder.Property(member => member.CompanyRole);
            builder.Property(member => member.IsActive);

            builder.Property(member => member.Preferences)
                .HasConversion(ValueConverters.WorkspaceMemberPreferencesConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.WorkspaceMemberPreferencesComparer);

            builder.Property(member => member.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
