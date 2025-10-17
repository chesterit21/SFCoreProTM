using MediatR;
using SFCoreProTM.Application.DTOs.Authentication;

namespace SFCoreProTM.Application.Features.Authentication.Commands.SignIn;

public sealed record SignInCommand(
    string Email,
    string Password,
    string? IpAddress,
    string? UserAgent) : IRequest<AuthResultDto>;
