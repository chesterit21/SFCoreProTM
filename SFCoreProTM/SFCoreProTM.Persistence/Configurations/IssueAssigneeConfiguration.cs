using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class IssueAssigneeConfiguration : IEntityTypeConfiguration<IssueAssignee>
    {
        public void Configure(EntityTypeBuilder<IssueAssignee> builder)
        {
            builder.ToTable("issue_assignees");
            builder.HasKey(assignee => assignee.Id);
            builder.Property(assignee => assignee.Id).ValueGeneratedNever();
            builder.Property(assignee => assignee.WorkspaceId).IsRequired();
            builder.Property(assignee => assignee.ProjectId).IsRequired();
            builder.Property(assignee => assignee.IssueId).IsRequired();
            builder.Property(assignee => assignee.AssigneeId).IsRequired();

            var issueAssigneeAuditTrail = builder.Property(assignee => assignee.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb");
            issueAssigneeAuditTrail.Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
