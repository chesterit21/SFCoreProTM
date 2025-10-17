using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Workspaces;

public sealed class UserFavorite : WorkspaceScopedEntity
{
    private UserFavorite()
    {
    }

    private UserFavorite(Guid id, Guid workspaceId, Guid userId, string entityType, Guid? entityIdentifier, bool isFolder, double sequence)
        : base(id, workspaceId)
    {
        UserId = userId;
        EntityType = entityType;
        EntityIdentifier = entityIdentifier;
        IsFolder = isFolder;
        Sequence = sequence;
    }

    public Guid UserId { get; private set; }

    public string EntityType { get; private set; } = string.Empty;

    public Guid? EntityIdentifier { get; private set; }

    public string? Name { get; private set; }

    public bool IsFolder { get; private set; }

    public double Sequence { get; private set; }

    public Guid? ParentId { get; private set; }

    public static UserFavorite Create(Guid id, Guid workspaceId, Guid userId, string entityType, Guid? entityIdentifier = null, bool isFolder = false, double sequence = 65535)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityType);
        return new UserFavorite(id, workspaceId, userId, entityType, entityIdentifier, isFolder, sequence);
    }

    public void Rename(string? name)
    {
        Name = name;
    }

    public void MoveTo(Guid? parentId, double sequence)
    {
        ParentId = parentId;
        Sequence = sequence;
    }
}
