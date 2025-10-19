using System;

namespace SFCoreProTM.Application.DTOs.UserProfiles;

public sealed class UserProfileDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string? Theme { get; init; }
    public bool IsAppRailDocked { get; init; }
    public bool IsTourCompleted { get; init; }
    public string? OnboardingStep { get; init; }
    public string? UseCase { get; init; }
    public string? Role { get; init; }
    public bool IsOnboarded { get; init; }
    public Guid? LastWorkspaceId { get; init; }
    public string? BillingAddressCountry { get; init; }
    public string? BillingAddress { get; init; }
    public bool HasBillingAddress { get; init; }
    public string? CompanyName { get; init; }
    public bool IsSmoothCursorEnabled { get; init; }
    public bool IsMobileOnboarded { get; init; }
    public string? MobileOnboardingStep { get; init; }
    public bool MobileTimezoneAutoSet { get; init; }
    public string? Language { get; init; }
    public int StartOfTheWeek { get; init; }
    public string? Goals { get; init; }
    public string? BackgroundColor { get; init; }
    public bool HasMarketingEmailConsent { get; init; }
}