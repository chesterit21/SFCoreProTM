using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SFCoreProTM.Presentation.Options;
using SFCoreProTM.Presentation.Services;

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
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            var cookieName = jwt.CookieName ?? "auth_token";
                            if (context.Request.Cookies.TryGetValue(cookieName, out var token))
                            {
                                context.Token = token;
                            }
                        }
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}

