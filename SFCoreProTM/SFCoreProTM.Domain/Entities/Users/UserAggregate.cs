using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.Entities.Users.Events;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Users;

public sealed class User : AggregateRoot
{
    private User()
    {
    }

    private User(Guid id, string username, EmailAddress? email, string displayName)
        : base(id)
    {
        Username = username;
        Email = email;
        DisplayName = displayName;
    }

    public string Username { get; private set; } = string.Empty;

    public EmailAddress? Email { get; private set; }

    public string? MobileNumber { get; private set; }

    public string DisplayName { get; private set; } = string.Empty;

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public Url? AvatarUrl { get; private set; }

    public Guid? AvatarAssetId { get; private set; }

    public Url? CoverImageUrl { get; private set; }

    public Guid? CoverImageAssetId { get; private set; }

    public DateTime DateJoined { get; private set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public string? LastLocation { get; private set; }

    public string? CreatedLocation { get; private set; }

    public bool IsSuperUser { get; private set; }

    public bool IsManaged { get; private set; }

    public bool IsPasswordExpired { get; private set; }

    public bool IsActive { get; private set; } = true;

    public bool IsStaff { get; private set; }

    public bool IsEmailVerified { get; private set; }

    public bool IsPasswordAutoset { get; private set; }

    public string? PasswordHash { get; private set; }

    public string Token { get; private set; } = string.Empty;

    public DateTime? LastActive { get; private set; }

    public DateTime? LastLoginTime { get; private set; }

    public DateTime? LastLogoutTime { get; private set; }

    public string? LastLoginIp { get; private set; }

    public string? LastLogoutIp { get; private set; }

    public string LastLoginMedium { get; private set; } = "email";

    public string? LastLoginUserAgent { get; private set; }

    public DateTime? TokenUpdatedAt { get; private set; }

    public bool IsBot { get; private set; }

    public string? BotType { get; private set; }

    public string UserTimezone { get; private set; } = "UTC";

    public bool IsEmailValid { get; private set; }

    public DateTime? MaskedAt { get; private set; }

    public static User Create(Guid id, string username, EmailAddress? email, string displayName)
    {
        return new User(id, username, email, displayName);
    }

    public void UpdateIdentity(string displayName, string firstName, string lastName)
    {
        DisplayName = displayName;
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContact(EmailAddress? email, string? mobileNumber)
    {
        Email = email;
        MobileNumber = mobileNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAvatar(Url? avatarUrl, Guid? avatarAssetId, Url? coverImageUrl, Guid? coverImageAssetId)
    {
        AvatarUrl = avatarUrl;
        AvatarAssetId = avatarAssetId;
        CoverImageUrl = coverImageUrl;
        CoverImageAssetId = coverImageAssetId;
    }

    public void UpdateStatus(bool isActive, bool isStaff, bool isSuperUser, bool isManaged)
    {
        IsActive = isActive;
        IsStaff = isStaff;
        IsSuperUser = isSuperUser;
        IsManaged = isManaged;
    }

    public void UpdateSecurity(bool isEmailVerified, bool isPasswordExpired, bool isPasswordAutoset, string token, DateTime? tokenUpdatedAt)
    {
        IsEmailVerified = isEmailVerified;
        IsPasswordExpired = isPasswordExpired;
        IsPasswordAutoset = isPasswordAutoset;
        Token = token;
        TokenUpdatedAt = tokenUpdatedAt;
    }

    public void SetPasswordHash(string passwordHash, bool isPasswordAutoset, DateTime timestamp)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
        IsPasswordAutoset = isPasswordAutoset;
        TokenUpdatedAt = timestamp;
        UpdatedAt = timestamp;
    }

    public void UpdateLoginMetrics(DateTime? lastLoginTime, DateTime? lastLogoutTime, string? lastLoginIp, string? lastLogoutIp, string lastLoginMedium, string? lastLoginUserAgent)
    {
        LastLoginTime = lastLoginTime;
        LastLogoutTime = lastLogoutTime;
        LastLoginIp = lastLoginIp;
        LastLogoutIp = lastLogoutIp;
        LastLoginMedium = lastLoginMedium;
        LastLoginUserAgent = lastLoginUserAgent;
    }

    public void RecordSuccessfulLogin(DateTime timestamp, string medium, string? ipAddress, string? userAgent)
    {
        LastLoginTime = timestamp;
        LastLoginIp = ipAddress;
        LastLoginMedium = medium;
        LastLoginUserAgent = userAgent;
        TokenUpdatedAt = timestamp;
        Touch(timestamp);
        RaiseDomainEvent(new UserLoggedInDomainEvent(Id, timestamp, medium, ipAddress, userAgent));
    }

    public void Touch(DateTime timestamp)
    {
        LastActive = timestamp;
        UpdatedAt = timestamp;
    }

    public void ConfigureAutomation(bool isBot, string? botType)
    {
        IsBot = isBot;
        BotType = botType;
    }

    public void UpdateTimezone(string timezone)
    {
        UserTimezone = timezone;
    }

    public void UpdateValidation(bool isEmailValid, DateTime? maskedAt)
    {
        IsEmailValid = isEmailValid;
        MaskedAt = maskedAt;
    }
}

public sealed class UserProfile : Entity
{
    private UserProfile()
    {
    }

    private UserProfile(Guid id, Guid userId, StructuredData theme)
        : base(id)
    {
        UserId = userId;
        Theme = theme;
    }

    public Guid UserId { get; private set; }

    public StructuredData? Theme { get; private set; } = StructuredData.FromJson(null);

    public bool? IsAppRailDocked { get; private set; } = true;

    public bool? IsTourCompleted { get; private set; }

    public StructuredData? OnboardingStep { get; private set; } = StructuredData.FromJson(null);

    public string? UseCase { get; private set; }

    public string? Role { get; private set; }

    public bool? IsOnboarded { get; private set; }

    public Guid? LastWorkspaceId { get; private set; }

    public string? BillingAddressCountry { get; private set; } = "Indonesia";

    public StructuredData? BillingAddress { get; private set; } = StructuredData.FromJson(null);

    public bool? HasBillingAddress { get; private set; } = false;

    public string? CompanyName { get; private set; }

    public bool? IsSmoothCursorEnabled { get; private set; }

    public bool? IsMobileOnboarded { get; private set; }

    public StructuredData? MobileOnboardingStep { get; private set; } = StructuredData.FromJson(null);

    public bool? MobileTimezoneAutoSet { get; private set; }

    public string? Language { get; private set; } = "id";

    public int? StartOfTheWeek { get; private set; } = 0;

    public StructuredData? Goals { get; private set; } = StructuredData.FromJson(null);

    public ColorCode? BackgroundColor { get; private set; } = ColorCode.FromHex("#FFFFFF");

    public bool? HasMarketingEmailConsent { get; private set; } = false;

    public bool? HasAppRailDocked => IsAppRailDocked;

    public static UserProfile Create(Guid id, Guid userId, StructuredData theme)
    {
        return new UserProfile(id, userId, theme);
    }

    public void UpdateTheme(StructuredData? theme, ColorCode? backgroundColor)
    {
        Theme = theme;
        BackgroundColor = backgroundColor;
    }

    public void UpdateOnboarding(bool isTourCompleted, bool isOnboarded, StructuredData? onboardingStep, StructuredData? mobileOnboardingStep)
    {
        IsTourCompleted = isTourCompleted;
        IsOnboarded = isOnboarded;
        OnboardingStep = onboardingStep;
        MobileOnboardingStep = mobileOnboardingStep;
    }

    public void UpdatePreferences(bool? isAppRailDocked, bool? isSmoothCursorEnabled, bool? hasMarketingEmailConsent, bool? mobileTimezoneAutoSet, int? startOfTheWeek, string? language)
    {
        IsAppRailDocked = isAppRailDocked;
        IsSmoothCursorEnabled = isSmoothCursorEnabled;
        HasMarketingEmailConsent = hasMarketingEmailConsent;
        MobileTimezoneAutoSet = mobileTimezoneAutoSet;
        StartOfTheWeek = startOfTheWeek;
        Language = language;
    }

    public void UpdateProfessionalDetails(string? useCase, string? role, string? companyName)
    {
        UseCase = useCase;
        Role = role;
        CompanyName = companyName;
    }

    public void UpdateBilling(Guid? lastWorkspaceId, StructuredData? billingAddress, string? billingCountry, bool? hasBillingAddress)
    {
        LastWorkspaceId = lastWorkspaceId;
        BillingAddress = billingAddress;
        BillingAddressCountry = billingCountry;
        HasBillingAddress = hasBillingAddress;
    }

    public void UpdateGoals(StructuredData goals)
    {
        Goals = goals;
    }
}

public sealed class Account : Entity
{
    private Account()
    {
    }

    private Account(Guid id, Guid userId, string providerAccountId, string provider, string accessToken)
        : base(id)
    {
        UserId = userId;
        ProviderAccountId = providerAccountId;
        Provider = provider;
        AccessToken = accessToken;
    }

    public Guid UserId { get; private set; }

    public string ProviderAccountId { get; private set; } = string.Empty;

    public string Provider { get; private set; } = string.Empty;

    public string AccessToken { get; private set; } = string.Empty;

    public DateTime? AccessTokenExpiresAt { get; private set; }

    public string? RefreshToken { get; private set; }

    public DateTime? RefreshTokenExpiresAt { get; private set; }

    public DateTime? LastConnectedAt { get; private set; }

    public string? IdToken { get; private set; }

    public StructuredData? Metadata { get; private set; } = StructuredData.FromJson(null);

    public static Account Create(Guid id, Guid userId, string providerAccountId, string provider, string accessToken)
    {
        return new Account(id, userId, providerAccountId, provider, accessToken);
    }

    public void UpdateTokens(string accessToken, DateTime? accessTokenExpiresAt, string? refreshToken, DateTime? refreshTokenExpiresAt, string? idToken)
    {
        AccessToken = accessToken;
        AccessTokenExpiresAt = accessTokenExpiresAt;
        RefreshToken = refreshToken;
        RefreshTokenExpiresAt = refreshTokenExpiresAt;
        IdToken = idToken;
    }

    public void MarkConnected(DateTime connectedAt)
    {
        LastConnectedAt = connectedAt;
    }

    public void UpdateMetadata(StructuredData metadata)
    {
        Metadata = metadata;
    }
}
