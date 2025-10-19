using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Data;
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
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IErdDefinitionRepository, ErdDefinitionRepository>();
        services.AddScoped<ISprintPlanningRepository, SprintPlanningRepository>();
        services.AddScoped<IFlowTaskRepository, FlowTaskRepository>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        return services;
    }
}