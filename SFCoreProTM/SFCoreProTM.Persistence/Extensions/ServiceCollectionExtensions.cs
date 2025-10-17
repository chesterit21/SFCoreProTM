using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Application.Interfaces.Security;
using SFCoreProTM.Persistence.Data;
using SFCoreProTM.Persistence.Repositories;
using SFCoreProTM.Persistence.Services;
using SFCoreProTM.Persistence.Services.Security;
using UoW = SFCoreProTM.Persistence.UnitOfWork;

namespace SFCoreProTM.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection is not configured.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUnitOfWork, UoW.UnitOfWork>();
        services.AddScoped<IWorkItemReadService, WorkItemReadService>();
        services.AddScoped<IIssueSequenceService, IssueSequenceService>();
        services.AddScoped<IIssueRepository, IssueRepository>();
        services.AddScoped<IIssueCommentRepository, IssueCommentRepository>();
        services.AddScoped<IIssueVersionRepository, IssueVersionRepository>();
        services.AddScoped<IIssueDescriptionVersionRepository, IssueDescriptionVersionRepository>();
        services.AddScoped<IWorkspaceReadService, WorkspaceReadService>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IStateRepository, StateRepository>();
        services.AddScoped<ILabelRepository, LabelRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IWorkspaceInviteRepository, WorkspaceInviteRepository>();
        services.AddScoped<IProjectInviteRepository, ProjectInviteRepository>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}
