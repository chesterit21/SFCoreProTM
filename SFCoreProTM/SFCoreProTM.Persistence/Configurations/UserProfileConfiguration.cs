using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Users;
using SFCoreProTM.Domain.ValueObjects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("user_profiles");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();
            builder.Property(p => p.UserId).IsRequired();

            var themeProperty = builder.Property(p => p.Theme)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb")
                .IsRequired(false);
            themeProperty.Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);

            builder.Property(p => p.IsAppRailDocked);
            builder.Property(p => p.IsTourCompleted);
            var onboardingProperty = builder.Property(p => p.OnboardingStep)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb");
            onboardingProperty.Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);

            builder.Property(p => p.UseCase);
            builder.Property(p => p.Role);
            builder.Property(p => p.IsOnboarded);
            builder.Property(p => p.LastWorkspaceId);
            builder.Property(p => p.BillingAddressCountry);
            var billingProperty = builder.Property(p => p.BillingAddress)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb");
            billingProperty.Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);
            builder.Property(p => p.HasBillingAddress);
            builder.Property(p => p.CompanyName);
            builder.Property(p => p.IsSmoothCursorEnabled);
            builder.Property(p => p.IsMobileOnboarded);
            var mobileOnboardingProperty = builder.Property(p => p.MobileOnboardingStep)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb");
            mobileOnboardingProperty.Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);
            builder.Property(p => p.MobileTimezoneAutoSet);
            builder.Property(p => p.Language);
            builder.Property(p => p.StartOfTheWeek);
            var goalsProperty = builder.Property(p => p.Goals)
                .HasConversion(ValueConverters.StructuredDataConverter)
                .HasColumnType("jsonb");
            goalsProperty.Metadata.SetValueComparer(ValueConverters.StructuredDataComparer);
            builder.Property(p => p.BackgroundColor)
                .HasConversion(color => color == null ? null : color.Value, value => value == null ? null : ColorCode.FromHex(value))
                .HasMaxLength(7);
            builder.Property(p => p.HasMarketingEmailConsent);
        }
    }
}
