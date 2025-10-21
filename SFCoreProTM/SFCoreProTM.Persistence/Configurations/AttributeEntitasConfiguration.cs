using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Persistence.Configurations;

public class AttributeEntitasConfiguration : IEntityTypeConfiguration<AttributeEntitas>
{
    public void Configure(EntityTypeBuilder<AttributeEntitas> builder)
    {
        builder.ToTable("erd_attributes");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.Property(a => a.ErdDefinitionId).IsRequired();
        builder.Property(a => a.Name).HasMaxLength(255).IsRequired(false);
        builder.Property(a => a.DataType).HasMaxLength(100).IsRequired(false);
        builder.Property(a => a.Description).IsRequired(false);
        builder.Property(a => a.MaxChar).IsRequired(false);
        builder.Property(a => a.SortOrder).IsRequired(false);
        builder.Property(a => a.IsPrimary).IsRequired(false);
        builder.Property(a => a.IsNull).IsRequired(false);
        builder.Property(a => a.IsForeignKey).IsRequired(false);
        builder.Property(a => a.ForeignKeyTable).HasMaxLength(255).IsRequired(false);
    }
}
