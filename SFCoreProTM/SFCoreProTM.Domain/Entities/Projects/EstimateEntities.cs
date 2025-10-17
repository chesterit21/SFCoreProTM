using System;
using SFCoreProTM.Domain.Entities;

namespace SFCoreProTM.Domain.Entities.Projects;

public sealed class Estimate : ProjectScopedEntity
{
    private Estimate()
    {
    }

    private Estimate(Guid id, Guid workspaceId, Guid projectId, string name, string type)
        : base(id, workspaceId, projectId)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public DateTime? LastUsed { get; private set; }

    public static Estimate Create(Guid id, Guid workspaceId, Guid projectId, string name, string type)
    {
        return new Estimate(id, workspaceId, projectId, name, type);
    }

    public void UpdateDetails(string name, string? description, string type)
    {
        Name = name;
        Description = description;
        Type = type;
    }

    public void Touch(DateTime timestamp)
    {
        LastUsed = timestamp;
    }
}

public sealed class EstimatePoint : ProjectScopedEntity
{
    private EstimatePoint()
    {
    }

    private EstimatePoint(Guid id, Guid workspaceId, Guid projectId, Guid estimateId, string key, decimal value)
        : base(id, workspaceId, projectId)
    {
        EstimateId = estimateId;
        Key = key;
        Value = value;
    }

    public Guid EstimateId { get; private set; }

    public string Key { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Value { get; private set; }

    public static EstimatePoint Create(Guid id, Guid workspaceId, Guid projectId, Guid estimateId, string key, decimal value)
    {
        return new EstimatePoint(id, workspaceId, projectId, estimateId, key, value);
    }

    public void UpdateDetails(string key, string? description, decimal value)
    {
        Key = key;
        Description = description;
        Value = value;
    }
}
