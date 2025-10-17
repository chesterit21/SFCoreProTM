using MediatR;
using SFCoreProTM.Application.DTOs.Authentication;

namespace SFCoreProTM.Application.Features.Authentication.Commands.SignUp;

public sealed record SignUpCommand(
    string Email,
    string Password,
    string? DisplayName,
    string? FirstName,
    string? LastName,
    string? IpAddress,
    string? UserAgent) : IRequest<AuthResultDto>;
