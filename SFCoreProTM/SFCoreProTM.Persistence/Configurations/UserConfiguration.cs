using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFCoreProTM.Domain.Entities.Users;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Id).ValueGeneratedNever();

            builder.Property(user => user.Username)
                .HasColumnName("username")
                .HasMaxLength(128)
                .IsRequired();

            var emailProperty = builder.Property(user => user.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .HasConversion(ValueConverters.EmailAddressConverter);
            emailProperty.Metadata.SetValueComparer(ValueConverters.EmailAddressComparer);

            builder.Property(user => user.MobileNumber)
                .HasColumnName("mobile_number")
                .HasMaxLength(255);

            builder.Property(user => user.DisplayName)
                .HasColumnName("display_name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(user => user.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(255);

            builder.Property(user => user.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(255);

            var avatarProperty = builder.Property(user => user.AvatarUrl)
                .HasColumnName("avatar")
                .HasConversion(ValueConverters.UrlConverter);
            avatarProperty.Metadata.SetValueComparer(ValueConverters.UrlComparer);

            builder.Property(user => user.AvatarAssetId)
                .HasColumnName("avatar_asset_id");

            var coverImageProperty = builder.Property(user => user.CoverImageUrl)
                .HasColumnName("cover_image")
                .HasConversion(ValueConverters.UrlConverter);
            coverImageProperty.Metadata.SetValueComparer(ValueConverters.UrlComparer);

            builder.Property(user => user.CoverImageAssetId)
                .HasColumnName("cover_image_asset_id");

            builder.Property(user => user.DateJoined).HasColumnName("date_joined");
            builder.Property(user => user.CreatedAt).HasColumnName("created_at");
            builder.Property(user => user.UpdatedAt).HasColumnName("updated_at");

            builder.Property(user => user.LastLocation).HasColumnName("last_location");
            builder.Property(user => user.CreatedLocation).HasColumnName("created_location");
            builder.Property(user => user.IsSuperUser).HasColumnName("is_superuser");
            builder.Property(user => user.IsManaged).HasColumnName("is_managed");
            builder.Property(user => user.IsPasswordExpired).HasColumnName("is_password_expired");
            builder.Property(user => user.IsActive).HasColumnName("is_active");
            builder.Property(user => user.IsStaff).HasColumnName("is_staff");
            builder.Property(user => user.IsEmailVerified).HasColumnName("is_email_verified");
            builder.Property(user => user.IsPasswordAutoset).HasColumnName("is_password_autoset");

            builder.Property(user => user.PasswordHash)
                .HasColumnName("password")
                .HasMaxLength(255);

            builder.Property(user => user.Token).HasColumnName("token");
            builder.Property(user => user.LastActive).HasColumnName("last_active");
            builder.Property(user => user.LastLoginTime).HasColumnName("last_login_time");
            builder.Property(user => user.LastLogoutTime).HasColumnName("last_logout_time");
            builder.Property(user => user.LastLoginIp).HasColumnName("last_login_ip");
            builder.Property(user => user.LastLogoutIp).HasColumnName("last_logout_ip");
            builder.Property(user => user.LastLoginMedium)
                .HasColumnName("last_login_medium")
                .HasMaxLength(20);
            builder.Property(user => user.LastLoginUserAgent).HasColumnName("last_login_uagent");
            builder.Property(user => user.TokenUpdatedAt).HasColumnName("token_updated_at");
            builder.Property(user => user.IsBot).HasColumnName("is_bot");
            builder.Property(user => user.BotType)
                .HasColumnName("bot_type")
                .HasMaxLength(30);
            builder.Property(user => user.UserTimezone)
                .HasColumnName("user_timezone")
                .HasMaxLength(255);
            builder.Property(user => user.IsEmailValid).HasColumnName("is_email_valid");
            builder.Property(user => user.MaskedAt).HasColumnName("masked_at");
        }
    }
}
