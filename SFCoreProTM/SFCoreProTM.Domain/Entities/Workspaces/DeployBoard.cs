using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Workspaces;

public sealed class DeployBoard : WorkspaceScopedEntity
{
    private DeployBoard()
    {
    }

    private DeployBoard(Guid id, Guid workspaceId, Guid entityIdentifier, string entityName, StructuredData viewProps)
        : base(id, workspaceId)
    {
        EntityIdentifier = entityIdentifier;
        EntityName = entityName;
        ViewProperties = viewProps;
    }

    public Guid EntityIdentifier { get; private set; }

    public string EntityName { get; private set; } = string.Empty;

    public string? Anchor { get; private set; }

    public bool IsCommentsEnabled { get; private set; }

    public bool IsReactionsEnabled { get; private set; }

    public Guid? IntakeId { get; private set; }

    public bool IsVotesEnabled { get; private set; }

    public StructuredData ViewProperties { get; private set; } = StructuredData.FromJson(null);

    public bool IsActivityEnabled { get; private set; }

    public bool IsDisabled { get; private set; }

    public static DeployBoard Create(Guid id, Guid workspaceId, Guid entityIdentifier, string entityName, StructuredData viewProps)
    {
        return new DeployBoard(id, workspaceId, entityIdentifier, entityName, viewProps);
    }

    public void ConfigureInteraction(bool commentsEnabled, bool reactionsEnabled, bool votesEnabled, bool activityEnabled)
    {
        IsCommentsEnabled = commentsEnabled;
        IsReactionsEnabled = reactionsEnabled;
        IsVotesEnabled = votesEnabled;
        IsActivityEnabled = activityEnabled;
    }

    public void SetAnchor(string? anchor)
    {
        Anchor = anchor;
    }

    public void LinkIntake(Guid? intakeId)
    {
        IntakeId = intakeId;
    }

    public void Disable(bool isDisabled)
    {
        IsDisabled = isDisabled;
    }
}
