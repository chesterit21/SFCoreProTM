using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class ProjectMemberInviteConfiguration : IEntityTypeConfiguration<ProjectMemberInvite>
    {
        public void Configure(EntityTypeBuilder<ProjectMemberInvite> builder)
        {
            builder.ToTable("project_member_invites");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedNever();
            builder.Property(i => i.WorkspaceId).IsRequired();
            builder.Property(i => i.ProjectId).IsRequired();

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

            var projectInviteAuditTrail = builder.Property(i => i.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb");
            projectInviteAuditTrail.Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
