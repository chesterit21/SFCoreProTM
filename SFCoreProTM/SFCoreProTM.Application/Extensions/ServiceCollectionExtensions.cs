using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SFCoreProTM.Application.Behaviors;
using SFCoreProTM.Application.Features.Issues.Commands.CreateIssue;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Services;

namespace SFCoreProTM.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(CreateIssueCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(CreateIssueCommand).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<IIssueHistoryService, IssueHistoryService>();
        return services;
    }
}
