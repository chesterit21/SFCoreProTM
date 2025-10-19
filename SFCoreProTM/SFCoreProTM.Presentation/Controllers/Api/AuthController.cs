using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SFCoreProTM.Application.Features.Authentication.Commands.SignIn;
using SFCoreProTM.Application.Features.Authentication.Commands.SignUp;
using SFCoreProTM.Application.Mapping.Requests.Authentication;
using SFCoreProTM.Presentation.Models.Authentication;
using SFCoreProTM.Presentation.Options;
using SFCoreProTM.Presentation.Services;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SFCoreProTM.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenService _jwt;
    private readonly JwtOptions _jwtOptions;
    private readonly InstanceOptions _instanceOptions;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, IJwtTokenService jwt, IOptions<JwtOptions> jwtOptions, IOptions<InstanceOptions> instanceOptions, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _jwt = jwt;
        _jwtOptions = jwtOptions.Value;
        _instanceOptions = instanceOptions.Value;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!_instanceOptions.EmailPasswordLoginEnabled)
        {
            return Forbid();
        }

        var command = new SignInCommand(
            request.Email,
            request.Password,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            Request.Headers.UserAgent.ToString());

        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Login berhasil untuk pengguna {UserId} dengan email {Email}", result.UserId, result.Email);
        _logger.LogInformation("LastWorkspaceId: {LastWorkspaceId}", result.LastWorkspaceId);

        var (token, expiresAt) = _jwt.CreateToken(result.UserId, result.Email, result.DisplayName, result.LastWorkspaceId);
        
        // Logging tambahan untuk memastikan token dibuat dengan klaim yang benar
        LogTokenClaims(token);
        
        SetAuthCookie(token, expiresAt, request.RememberMe);

        var response = new AuthResponse
        {
            UserId = result.UserId,
            Email = result.Email,
            DisplayName = result.DisplayName,
            LastLoginAt = result.LastLoginAt,
            IsPasswordAutoset = result.IsPasswordAutoset,
            AccessToken = token,
            ExpiresAtUtc = expiresAt,
        };

        return Ok(response);
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!_instanceOptions.SignupsEnabled)
        {
            return Forbid();
        }

        if (_instanceOptions.InviteRequiredForSignup && string.IsNullOrWhiteSpace(request.InviteToken))
        {
            return Forbid();
        }

        // Enforce email allowlist / domain restrictions if configured
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var at = email.LastIndexOf('@');
            var domain = at >= 0 ? email[(at + 1)..] : string.Empty;
            if (_instanceOptions.WhitelistEmails != null && _instanceOptions.WhitelistEmails.Length > 0)
            {
                var ok = System.Array.Exists(_instanceOptions.WhitelistEmails, e => string.Equals(e?.Trim().ToLowerInvariant(), email, System.StringComparison.Ordinal));
                if (!ok) return Forbid();
            }
            if (_instanceOptions.AllowedEmailDomains != null && _instanceOptions.AllowedEmailDomains.Length > 0)
            {
                var ok = System.Array.Exists(_instanceOptions.AllowedEmailDomains, d => string.Equals(d?.Trim().ToLowerInvariant(), domain, System.StringComparison.Ordinal));
                if (!ok) return Forbid();
            }
        }

        // If invite is required or present, validate it before creating user
        Guid? inviteTokenGuid = null;
        if (!string.IsNullOrWhiteSpace(request.InviteToken))
        {
            if (!Guid.TryParse(request.InviteToken, out var parsed))
            {
                return Forbid();
            }
            inviteTokenGuid = parsed;
        }

        var command = new SignUpCommand(
            request.Email,
            request.Password,
            request.DisplayName,
            request.FirstName,
            request.LastName,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            Request.Headers.UserAgent.ToString());

        var result = await _mediator.Send(command, cancellationToken);


        // Immediately sign the user in
        var (token, expiresAt) = _jwt.CreateToken(result.UserId, result.Email, result.DisplayName, result.LastWorkspaceId);
        
        // Logging tambahan untuk memastikan token dibuat dengan klaim yang benar
        LogTokenClaims(token);
        
        SetAuthCookie(token, expiresAt, rememberMe: true);

        var response = new AuthResponse
        {
            UserId = result.UserId,
            Email = result.Email,
            DisplayName = result.DisplayName,
            LastLoginAt = result.LastLoginAt,
            IsPasswordAutoset = result.IsPasswordAutoset,
            AccessToken = token,
            ExpiresAtUtc = expiresAt,
        };

        return Ok(response);
    }

    private void SetAuthCookie(string token, DateTime expiresAt, bool rememberMe)
    {
        var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
        {
            HttpOnly = true,
            Secure = _jwtOptions.CookieSecure,
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
            Expires = rememberMe ? expiresAt : (DateTime?)null,
            IsEssential = true,
        };
        Response.Cookies.Append(_jwtOptions.CookieName, token, cookieOptions);
        
        // Logging untuk memastikan cookie diset dengan benar
        _logger.LogInformation("Auth cookie diset dengan nama {CookieName}", _jwtOptions.CookieName);
    }
    
    private void LogTokenClaims(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            _logger.LogInformation("Token klaim:");
            foreach (var claim in jsonToken.Claims)
            {
                _logger.LogInformation("  {ClaimType}: {ClaimValue}", claim.Type, claim.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gagal membaca klaim token");
        }
    }
}