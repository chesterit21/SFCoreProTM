using MediatR;
using SFCoreProTM.Application.DTOs.UserProfiles;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.UserProfiles.Queries.GetByUserId;

public sealed class GetUserProfileByUserIdQueryHandler : IRequestHandler<GetUserProfileByUserIdQuery, UserProfileDto?>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public GetUserProfileByUserIdQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfileDto?> Handle(GetUserProfileByUserIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        if (profile is null)
        {
            return null;
        }

        return new UserProfileDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            Theme = profile.Theme?.ToString(),
            IsAppRailDocked = profile.IsAppRailDocked ?? false,
            IsTourCompleted = profile.IsTourCompleted ?? false,
            OnboardingStep = profile.OnboardingStep?.ToString(),
            UseCase = profile.UseCase,
            Role = profile.Role,
            IsOnboarded = profile.IsOnboarded ?? false,
            LastWorkspaceId = profile.LastWorkspaceId,
            BillingAddressCountry = profile.BillingAddressCountry,
            BillingAddress = profile.BillingAddress?.ToString(),
            HasBillingAddress = profile.HasBillingAddress ?? false,
            CompanyName = profile.CompanyName,
            IsSmoothCursorEnabled = profile.IsSmoothCursorEnabled ?? false,
            IsMobileOnboarded = profile.IsMobileOnboarded ?? false,
            MobileOnboardingStep = profile.MobileOnboardingStep?.ToString(),
            MobileTimezoneAutoSet = profile.MobileTimezoneAutoSet ?? false,
            Language = profile.Language,
            StartOfTheWeek = profile.StartOfTheWeek ?? 0,
            Goals = profile.Goals?.ToString(),
            BackgroundColor = profile.BackgroundColor?.Value,
            HasMarketingEmailConsent = profile.HasMarketingEmailConsent ?? false
        };
    }
}