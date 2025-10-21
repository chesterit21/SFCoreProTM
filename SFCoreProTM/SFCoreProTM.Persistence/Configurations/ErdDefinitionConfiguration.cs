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
        builder.Property(e => e.EntityName).HasMaxLength(255);
        builder.Property(e => e.SortOrder).IsRequired();
        builder.Property(e => e.ErdStatus).IsRequired();

        builder.HasMany(e => e.Attributes)
            .WithOne(a => a.ErdDefinition!)
            .HasForeignKey(a => a.ErdDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(nameof(ErdDefinition.Attributes))
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
