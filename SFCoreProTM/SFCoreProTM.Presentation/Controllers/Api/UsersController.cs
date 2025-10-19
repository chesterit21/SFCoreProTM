using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Application.Features.Users.Queries.GetCurrentUser;
using SFCoreProTM.Application.Interfaces.Security;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace SFCoreProTM.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;
    
    public UsersController(IMediator mediator, ICurrentUserService currentUserService, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        _logger.LogInformation("Memanggil endpoint Me");
        
        // Logging informasi cookie
        _logger.LogInformation("Jumlah cookie: {CookieCount}", Request.Cookies.Count);
        foreach (var cookie in Request.Cookies)
        {
            _logger.LogInformation("Cookie: {CookieName} = {CookieValue}", cookie.Key, cookie.Value ?? "(null)");
        }
        
        // Logging informasi klaim pengguna
        if (User?.Identity?.IsAuthenticated == true)
        {
            _logger.LogInformation("User terautentikasi");
            var subClaim = User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            _logger.LogInformation("Klaim 'sub': {SubClaim}", subClaim ?? "(null)");
            
            // Logging tambahan untuk mendiagnosis klaim
            _logger.LogInformation("Jumlah klaim pengguna: {ClaimCount}", User?.Claims?.Count());
            foreach (var claim in User?.Claims ?? new System.Collections.Generic.List<System.Security.Claims.Claim>())
            {
                _logger.LogInformation("Klaim: {ClaimType} = {ClaimValue}", claim.Type, claim.Value);
            }
        }
        else
        {
            _logger.LogWarning("User tidak terautentikasi");
            _logger.LogInformation("Header Authorization: {AuthorizationHeader}", Request.Headers.Authorization.ToString() ?? "(null)");
        }
        
        var userId = _currentUserService.UserId;
        _logger.LogInformation("UserId dari CurrentUserService: {UserId}", userId.HasValue ? userId.Value.ToString() : "(null)");
        
        if (userId == null) 
        {
            _logger.LogWarning("UserId adalah null, mengembalikan Unauthorized");
            return Unauthorized();
        }
        
        var dto = await _mediator.Send(new GetCurrentUserQuery(userId.Value), ct);
        return Ok(dto);
    }
}