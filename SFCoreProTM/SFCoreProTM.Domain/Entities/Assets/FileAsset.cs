using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Assets;

public sealed class FileAsset : WorkspaceScopedEntity
{
    private FileAsset()
    {
    }

    private FileAsset(Guid id, Guid workspaceId, Guid? userId, string assetPath, StructuredData attributes)
        : base(id, workspaceId)
    {
        UserId = userId;
        AssetPath = assetPath;
        Attributes = attributes;
    }

    public StructuredData Attributes { get; private set; } = StructuredData.FromJson(null);

    public string AssetPath { get; private set; } = string.Empty;

    public Guid? UserId { get; private set; }

    public Guid? DraftIssueId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? IssueId { get; private set; }

    public Guid? CommentId { get; private set; }

    public Guid? PageId { get; private set; }

    public string? EntityType { get; private set; }

    public Guid? EntityIdentifier { get; private set; }

    public bool IsDeleted { get; private set; }

    public bool IsArchived { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public long? Size { get; private set; }

    public bool IsUploaded { get; private set; }

    public StructuredData StorageMetadata { get; private set; } = StructuredData.FromJson(null);

    public static FileAsset Create(Guid id, Guid workspaceId, Guid? userId, string assetPath, StructuredData attributes)
    {
        return new FileAsset(id, workspaceId, userId, assetPath, attributes);
    }

    public void AttachToEntity(string entityType, Guid? entityIdentifier)
    {
        EntityType = entityType;
        EntityIdentifier = entityIdentifier;
    }

    public void LinkProject(Guid? projectId)
    {
        ProjectId = projectId;
    }

    public void LinkDraftIssue(Guid? draftIssueId)
    {
        DraftIssueId = draftIssueId;
    }

    public void LinkIssue(Guid? issueId)
    {
        IssueId = issueId;
    }

    public void LinkComment(Guid? commentId)
    {
        CommentId = commentId;
    }

    public void LinkPage(Guid? pageId)
    {
        PageId = pageId;
    }

    public void SetUploadState(bool isUploaded, long? size)
    {
        IsUploaded = isUploaded;
        Size = size;
    }

    public void SetDeletionState(bool isDeleted, bool isArchived)
    {
        IsDeleted = isDeleted;
        IsArchived = isArchived;
    }

    public void SetExternalReference(string? source, string? identifier)
    {
        ExternalReference = source is null && identifier is null ? null : ExternalReference.Create(source, identifier);
    }

    public void UpdateStorageMetadata(StructuredData metadata)
    {
        StorageMetadata = metadata;
    }
}
