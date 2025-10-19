using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Projects;

public sealed class Project : AggregateRoot
{
    private Project()
    {
    }

    private Project(Guid id, Guid workspaceId, string name, string description, string projectPath)
        : base(id)
    {
        WorkspaceId = workspaceId;
        Name = name;
        Description = description;
        ProjectPath = projectPath;
    }

    public Guid WorkspaceId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string ProjectPath { get; private set; } = string.Empty;

    public static Project Create(Guid id, Guid workspaceId, string name, string description, string projectPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Project(id, workspaceId, name, description ?? string.Empty, projectPath ?? string.Empty);
    }

    public void UpdateDetails(string name, string description, string projectPath)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        
        Description = description ?? string.Empty;
        ProjectPath = projectPath ?? string.Empty;
    }
}
