using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFCoreProTM.Application.Interfaces.Security;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace SFCoreProTM.Presentation.Middlewares;

/// <summary>
/// Middleware untuk memastikan klaim pengguna tersedia dalam konteks ASP.NET Core MVC
/// </summary>
public class UserClaimsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserClaimsMiddleware> _logger;

    public UserClaimsMiddleware(RequestDelegate next, ILogger<UserClaimsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Logging tambahan untuk mendiagnosis masalah autentikasi
        _logger.LogInformation("Memulai UserClaimsMiddleware");
        _logger.LogInformation("Apakah pengguna terautentikasi: {IsAuthenticated}", context.User?.Identity?.IsAuthenticated);
        _logger.LogInformation("Jumlah klaim pengguna: {ClaimCount}", context.User?.Claims?.Count());
        
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            foreach (var claim in context.User?.Claims ?? new System.Collections.Generic.List<Claim>())
            {
                _logger.LogInformation("Klaim: {ClaimType} = {ClaimValue}", claim.Type, claim.Value);
            }
        }
        
        // Mendapatkan service CurrentUserService
        var currentUserService = context.RequestServices.GetService<ICurrentUserService>();
        
        // Memastikan klaim pengguna tersedia sebelum melanjutkan ke middleware berikutnya
        if (currentUserService != null)
        {
            // Klaim pengguna akan tersedia melalui CurrentUserService
            // yang mengambil data dari HttpContext.User
            try
            {
                // Memastikan UserId dan WorkspaceId dapat diakses
                var userId = currentUserService.UserId;
                var workspaceId = currentUserService.WorkspaceId;
                
                _logger.LogInformation("UserId dari CurrentUserService: {UserId}", userId);
                _logger.LogInformation("WorkspaceId dari CurrentUserService: {WorkspaceId}", workspaceId);
                
                // Untuk debugging - mencatat klaim yang tersedia
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    // Klaim tersedia, tidak perlu melakukan apa pun
                }
            }
            catch (Exception ex)
            {
                // Menangkap dan log exception
                _logger.LogError(ex, "Error saat mengakses CurrentUserService");
            }
        }

        // Melanjutkan ke middleware berikutnya
        await _next(context);
    }
}