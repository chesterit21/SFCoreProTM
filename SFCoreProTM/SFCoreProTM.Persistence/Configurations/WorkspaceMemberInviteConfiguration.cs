using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Users;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class WorkspaceMemberInviteConfiguration : IEntityTypeConfiguration<WorkspaceMemberInvite>
    {
        public void Configure(EntityTypeBuilder<WorkspaceMemberInvite> builder)
        {
            builder.ToTable("workspace_member_invites");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedNever();
            builder.Property(i => i.WorkspaceId).IsRequired();

            var emailProperty = builder.Property(i => i.Email)
                .HasConversion(ValueConverters.NonNullEmailAddressConverter)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();
            emailProperty.Metadata.SetValueComparer(ValueConverters.NonNullEmailAddressComparer);

            builder.Property(i => i.Accepted).HasColumnName("accepted");
            builder.Property(i => i.Token).HasColumnName("token").IsRequired();
            builder.Property(i => i.Message).HasColumnName("message");
            builder.Property(i => i.RespondedAt).HasColumnName("responded_at");
            builder.Property(i => i.Role).HasConversion<int>().HasColumnName("role");

            var workspaceInviteAuditTrail = builder.Property(i => i.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb");
            workspaceInviteAuditTrail.Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
