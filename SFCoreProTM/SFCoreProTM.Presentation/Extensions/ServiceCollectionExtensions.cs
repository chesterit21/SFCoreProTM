using Microsoft.Extensions.DependencyInjection;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Persistence.Repositories;

namespace SFCoreProTM.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IErdDefinitionRepository, ErdDefinitionRepository>();
        services.AddScoped<ISprintPlanningRepository, SprintPlanningRepository>();
        services.AddScoped<IFlowTaskRepository, FlowTaskRepository>();
        
        return services;
    }
}