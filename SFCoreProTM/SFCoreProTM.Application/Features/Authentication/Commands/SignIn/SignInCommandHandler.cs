using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Authentication;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Application.Interfaces.Security;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.Authentication.Commands.SignIn;

public sealed class SignInCommandHandler : IRequestHandler<SignInCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public SignInCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(SignInCommand request, CancellationToken cancellationToken)
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
            throw new AuthenticationException("Invalid email or password.", ex);
        }

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
        {
            throw new AuthenticationException("Invalid email or password.");
        }

        if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            throw new AuthenticationException("Invalid email or password.");
        }

        var now = _dateTimeProvider.UtcNow;
        user.RecordSuccessfulLogin(now, "email", request.IpAddress, request.UserAgent);

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
