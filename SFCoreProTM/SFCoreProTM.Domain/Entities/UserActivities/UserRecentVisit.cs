using System;
using SFCoreProTM.Domain.Entities;

namespace SFCoreProTM.Domain.Entities.UserActivities;

public enum VisitedEntity
{
    View,
    Page,
    Issue,
    Cycle,
    Module,
    Project,
}

public sealed class UserRecentVisit : WorkspaceScopedEntity
{
    private UserRecentVisit()
    {
    }

    private UserRecentVisit(Guid id, Guid workspaceId, Guid userId, VisitedEntity entity, Guid? entityIdentifier, DateTime visitedAt)
        : base(id, workspaceId)
    {
        UserId = userId;
        Entity = entity;
        EntityIdentifier = entityIdentifier;
        VisitedAt = visitedAt;
    }

    public Guid? EntityIdentifier { get; private set; }

    public VisitedEntity Entity { get; private set; }

    public Guid UserId { get; private set; }

    public DateTime VisitedAt { get; private set; }

    public static UserRecentVisit Create(Guid id, Guid workspaceId, Guid userId, VisitedEntity entity, Guid? entityIdentifier, DateTime visitedAt)
    {
        return new UserRecentVisit(id, workspaceId, userId, entity, entityIdentifier, visitedAt);
    }

    public void Touch(DateTime visitedAt)
    {
        VisitedAt = visitedAt;
    }
}
