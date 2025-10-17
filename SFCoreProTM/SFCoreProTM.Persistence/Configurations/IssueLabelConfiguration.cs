using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class IssueLabelConfiguration : IEntityTypeConfiguration<IssueLabel>
    {
        public void Configure(EntityTypeBuilder<IssueLabel> builder)
        {
            builder.ToTable("issue_labels");
            builder.HasKey(label => label.Id);
            builder.Property(label => label.Id).ValueGeneratedNever();
            builder.Property(label => label.WorkspaceId).IsRequired();
            builder.Property(label => label.ProjectId).IsRequired();
            builder.Property(label => label.IssueId).IsRequired();
            builder.Property(label => label.LabelId).IsRequired();

            var issueLabelAuditTrail = builder.Property(label => label.AuditTrail)
                .HasConversion(ValueConverters.AuditTrailConverter)
                .HasColumnType("jsonb");
            issueLabelAuditTrail.Metadata.SetValueComparer(ValueConverters.AuditTrailComparer);
        }
    }
}
