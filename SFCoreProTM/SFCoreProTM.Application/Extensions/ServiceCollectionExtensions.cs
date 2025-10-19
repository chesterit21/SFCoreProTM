using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SFCoreProTM.Application.Behaviors;
using SFCoreProTM.Application.Interfaces;

namespace SFCoreProTM.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}