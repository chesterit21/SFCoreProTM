using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SFCoreProTM.Presentation.Options;
using SFCoreProTM.Presentation.Services;
using Microsoft.Extensions.Logging;

namespace SFCoreProTM.Presentation.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<InstanceOptions>(configuration.GetSection(InstanceOptions.SectionName));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        var jwt = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(jwt);

        var key = Encoding.UTF8.GetBytes(jwt.Secret);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = !string.IsNullOrWhiteSpace(jwt.Issuer),
                    ValidateAudience = !string.IsNullOrWhiteSpace(jwt.Audience),
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                // Read token from cookie if Authorization header is missing
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
                        
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            logger?.LogInformation("Token tidak tersedia dalam header Authorization");
                            
                            var cookieName = jwt.CookieName ?? "auth_token";
                            logger?.LogInformation("Mencoba membaca token dari cookie dengan nama: {CookieName}", cookieName);
                            
                            if (context.Request.Cookies.TryGetValue(cookieName, out var token))
                            {
                                logger?.LogInformation("Token ditemukan dalam cookie");
                                context.Token = token;
                            }
                            else
                            {
                                logger?.LogWarning("Token tidak ditemukan dalam cookie");
                            }
                        }
                        else
                        {
                            logger?.LogInformation("Token tersedia dalam header Authorization");
                        }
                        
                        return System.Threading.Tasks.Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
                        logger?.LogInformation("Token berhasil divalidasi");
                        
                        // Log jumlah dan isi klaim
                        logger?.LogInformation("Jumlah klaim dalam token: {ClaimCount}", context.Principal?.Claims?.Count());
                        if (context.Principal?.Claims != null)
                        {
                            foreach (var claim in context.Principal.Claims)
                            {
                                logger?.LogInformation("Klaim: {ClaimType} = {ClaimValue}", claim.Type, claim.Value);
                            }
                        }
                        
                        return System.Threading.Tasks.Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
                        logger?.LogError(context.Exception, "Autentikasi gagal");
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}