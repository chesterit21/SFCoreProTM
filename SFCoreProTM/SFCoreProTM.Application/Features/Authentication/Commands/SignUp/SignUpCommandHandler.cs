using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Authentication;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Application.Interfaces.Security;
using SFCoreProTM.Domain.Entities.Users;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.Authentication.Commands.SignUp;

public sealed class SignUpCommandHandler : IRequestHandler<SignUpCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserProfileRepository _userProfileRepository;

    public SignUpCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork,
        IUserProfileRepository userProfileRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<AuthResultDto> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new AuthenticationException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new AuthenticationException("Password is required.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        EmailAddress email;
        try
        {
            email = EmailAddress.Create(normalizedEmail);
        }
        catch (FormatException ex)
        {
            throw new AuthenticationException("Invalid email address.", ex);
        }

        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException($"User with email '{normalizedEmail}' already exists.");
        }

        var now = _dateTimeProvider.UtcNow;
        var passwordHash = _passwordHasher.Hash(request.Password);

        var displayName = string.IsNullOrWhiteSpace(request.DisplayName)
            ? normalizedEmail.Split('@', StringSplitOptions.RemoveEmptyEntries)[0]
            : request.DisplayName.Trim();

        var firstName = request.FirstName?.Trim() ?? string.Empty;
        var lastName = request.LastName?.Trim() ?? string.Empty;

        var user = User.Create(
            Guid.NewGuid(),
            $"user_{Guid.NewGuid():N}",
            email,
            displayName);

        if (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName))
        {
            user.UpdateIdentity(displayName, firstName, lastName);
        }

        user.SetPasswordHash(passwordHash, isPasswordAutoset: false, now);
        user.RecordSuccessfulLogin(now, "email", request.IpAddress, request.UserAgent);

        await _userRepository.AddAsync(user, cancellationToken);

        // Create a default user profile similar to Plane's behavior
        var profile = UserProfile.Create(Guid.NewGuid(), user.Id, StructuredData.FromJson(null));
        await _userProfileRepository.AddAsync(profile, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto
        {
            UserId = user.Id,
            Email = user.Email?.Value ?? string.Empty,
            DisplayName = user.DisplayName,
            LastLoginAt = user.LastLoginTime,
            IsPasswordAutoset = user.IsPasswordAutoset,
        };
    }
}
