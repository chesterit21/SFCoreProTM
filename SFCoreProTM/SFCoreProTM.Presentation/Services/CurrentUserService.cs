using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SFCoreProTM.Application.Interfaces.Security;
using Microsoft.Extensions.Logging;

namespace SFCoreProTM.Presentation.Services;

/// <summary>
/// Layanan untuk mendapatkan informasi pengguna saat ini dari klaim JWT
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Mendapatkan ID pengguna dari klaim "sub" (subject) dalam token JWT
    /// </summary>
    public Guid? UserId
    {
        get
        {
            //_logger.LogInformation("Mengambil UserId dari klaim");
            //_logger.LogInformation("HttpContext tersedia: {HttpContextAvailable}", _httpContextAccessor.HttpContext != null);
            
            // if (_httpContextAccessor.HttpContext != null)
            // {
            //     //_logger.LogInformation("User tersedia: {UserAvailable}", _httpContextAccessor.HttpContext.User != null);
            //     //_logger.LogInformation("Identitas terautentikasi: {IsAuthenticated}", _httpContextAccessor.HttpContext.User?.Identity?.IsAuthenticated);
                
            //     // if (_httpContextAccessor.HttpContext.User?.Identity?.IsAuthenticated == true)
            //     // {
            //     //     //_logger.LogInformation("Jumlah klaim: {ClaimCount}", _httpContextAccessor.HttpContext.User?.Claims?.Count());
            //     //     foreach (var claim in _httpContextAccessor.HttpContext.User?.Claims ?? new System.Collections.Generic.List<Claim>())
            //     //     {
            //     //         _logger.LogInformation("Klaim: {ClaimType} = {ClaimValue}", claim.Type, claim.Value);
            //     //     }
            //     // }
            // }
            
            // .NET secara otomatis memetakan klaim "sub" ke "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            var sub = _httpContextAccessor.HttpContext?.User?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            //_logger.LogInformation("Nilai klaim 'sub' (dari nameidentifier): {SubValue}", sub);
            
            if (Guid.TryParse(sub, out var id))
            {
                //_logger.LogInformation("UserId berhasil di-parse: {UserId}", id);
                return id;
            }
            
            // Fallback ke pencarian klaim "sub" langsung
            var directSub = _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
            //_logger.LogInformation("Nilai klaim 'sub' (langsung): {DirectSubValue}", directSub);
            
            if (Guid.TryParse(directSub, out var directId))
            {
                //_logger.LogInformation("UserId berhasil di-parse (langsung): {UserId}", directId);
                return directId;
            }
            
            //_logger.LogWarning("Gagal mem-parsing UserId dari klaim 'sub'");
            return null;
        }
    }

    /// <summary>
    /// Mendapatkan ID workspace dari klaim "workspace_id" dalam token JWT
    /// </summary>
    public Guid? WorkspaceId
    {
        get
        {
            //_logger.LogInformation("Mengambil WorkspaceId dari klaim");
            var workspaceId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("workspace_id");
            //_logger.LogInformation("Nilai klaim 'workspace_id': {WorkspaceIdValue}", workspaceId);
            
            if (Guid.TryParse(workspaceId, out var id))
            {
                //_logger.LogInformation("WorkspaceId berhasil di-parse: {WorkspaceId}", id);
                return id;
            }
            
            //_logger.LogWarning("Gagal mem-parsing WorkspaceId dari klaim 'workspace_id'");
            return null;
        }
    }
}