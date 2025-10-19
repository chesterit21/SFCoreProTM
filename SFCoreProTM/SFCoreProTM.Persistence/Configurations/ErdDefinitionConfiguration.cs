using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Persistence.Configurations;

public class ErdDefinitionConfiguration : IEntityTypeConfiguration<ErdDefinition>
{
    public void Configure(EntityTypeBuilder<ErdDefinition> builder)
    {
        builder.ToTable("erd_definitions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ModuleId).IsRequired();
        builder.Property(e => e.TName).HasMaxLength(255).IsRequired();
        builder.Property(e => e.Description);
        builder.Property(e => e.EntityName).HasMaxLength(255).IsRequired();
        builder.Property(e => e.AttributeName).HasMaxLength(255).IsRequired();
        builder.Property(e => e.AttributeType).HasMaxLength(100).IsRequired();
        builder.Property(e => e.IsPrimaryKey).IsRequired();
        builder.Property(e => e.IsAcceptNull).IsRequired();
        builder.Property(e => e.MaxChar).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.SortOrder).IsRequired();
        builder.Property(e => e.ErdStatus).IsRequired();
    }
}