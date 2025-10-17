using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Issues;

public enum IssuePriority
{
    Urgent,
    High,
    Medium,
    Low,
    None,
}

public enum IssueRelationType
{
    Duplicate,
    RelatesTo,
    BlockedBy,
    StartBefore,
    FinishBefore,
    ImplementedBy,
}

public enum IssueCommentAccess
{
    Internal,
    External,
}

public enum IssueVoteDirection
{
    Downvote = -1,
    Upvote = 1,
}

public sealed class Issue : AggregateRoot
{
    private readonly HashSet<Guid> _assigneeIds = new();
    private readonly HashSet<Guid> _labelIds = new();

    private Issue()
    {
    }

    private Issue(Guid id, Guid workspaceId, Guid projectId, string name, IssuePriority priority, int sequenceId, DateRange schedule, double sortOrder, RichTextContent description)
        : base(id)
    {
        WorkspaceId = workspaceId;
        ProjectId = projectId;
        Name = name;
        Priority = priority;
        SequenceId = sequenceId;
        Schedule = schedule;
        SortOrder = sortOrder;
        Description = description;
    }

    public Guid WorkspaceId { get; private set; }

    public Guid ProjectId { get; private set; }

    public Guid? ParentId { get; private set; }

    public Guid? StateId { get; private set; }

    public int? PointEstimate { get; private set; }

    public Guid? EstimatePointId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public RichTextContent Description { get; private set; } = RichTextContent.Create();

    public IssuePriority Priority { get; private set; } = IssuePriority.None;

    public DateRange Schedule { get; private set; } = DateRange.Create(null, null);

    public IReadOnlyCollection<Guid> AssigneeIds => _assigneeIds;

    public IReadOnlyCollection<Guid> LabelIds => _labelIds;

    public int SequenceId { get; private set; }

    public double SortOrder { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? ArchivedAt { get; private set; }

    public bool IsDraft { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public Guid? IssueTypeId { get; private set; }

    public static Issue Create(Guid id, Guid workspaceId, Guid projectId, string name, IssuePriority priority, int sequenceId, DateRange schedule, double sortOrder, RichTextContent description)
    {
        return new Issue(id, workspaceId, projectId, name, priority, sequenceId, schedule, sortOrder, description);
    }

    public void SetHierarchy(Guid? parentId)
    {
        ParentId = parentId;
    }

    public void SetState(Guid? stateId)
    {
        StateId = stateId;
    }

    public void SetEstimate(int? pointEstimate, Guid? estimatePointId)
    {
        PointEstimate = pointEstimate;
        EstimatePointId = estimatePointId;
    }

    public void UpdateDetails(string name, IssuePriority priority, DateRange schedule, double sortOrder, RichTextContent description, bool isDraft)
    {
        Name = name;
        Priority = priority;
        Schedule = schedule;
        SortOrder = sortOrder;
        Description = description;
        IsDraft = isDraft;
    }

    public void MarkCompleted(DateTime? completedAt)
    {
        CompletedAt = completedAt;
    }

    public void Archive(DateTime? archivedAt)
    {
        ArchivedAt = archivedAt;
    }

    public void SetIssueType(Guid? issueTypeId)
    {
        IssueTypeId = issueTypeId;
    }

    public void SetExternalReference(string? source, string? externalId)
    {
        ExternalReference = source is null && externalId is null ? null : ExternalReference.Create(source, externalId);
    }

    public void Assign(Guid userId)
    {
        _assigneeIds.Add(userId);
    }

    public void Unassign(Guid userId)
    {
        _assigneeIds.Remove(userId);
    }

    public void ReplaceAssignees(IEnumerable<Guid> userIds)
    {
        _assigneeIds.Clear();
        foreach (var userId in userIds)
        {
            _assigneeIds.Add(userId);
        }
    }

    public void TagLabel(Guid labelId)
    {
        _labelIds.Add(labelId);
    }

    public void UntagLabel(Guid labelId)
    {
        _labelIds.Remove(labelId);
    }

    public void ReplaceLabels(IEnumerable<Guid> labelIds)
    {
        _labelIds.Clear();
        foreach (var labelId in labelIds)
        {
            _labelIds.Add(labelId);
        }
    }

    public void SoftDelete(DateTime deletedAt, Guid? actorId)
    {
        SetAuditTrail(AuditTrail.Create(
            AuditTrail.CreatedAt,
            AuditTrail.CreatedById,
            deletedAt,
            actorId,
            deletedAt));
    }
}

public sealed class IssueBlocker : ProjectScopedEntity
{
    private IssueBlocker()
    {
    }

    private IssueBlocker(Guid id, Guid workspaceId, Guid projectId, Guid blockedIssueId, Guid blockedByIssueId)
        : base(id, workspaceId, projectId)
    {
        BlockedIssueId = blockedIssueId;
        BlockedByIssueId = blockedByIssueId;
    }

    public Guid BlockedIssueId { get; private set; }

    public Guid BlockedByIssueId { get; private set; }

    public static IssueBlocker Create(Guid id, Guid workspaceId, Guid projectId, Guid blockedIssueId, Guid blockedByIssueId)
    {
        return new IssueBlocker(id, workspaceId, projectId, blockedIssueId, blockedByIssueId);
    }
}

public sealed class IssueRelation : ProjectScopedEntity
{
    private IssueRelation()
    {
    }

    private IssueRelation(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid relatedIssueId, IssueRelationType relationType)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        RelatedIssueId = relatedIssueId;
        RelationType = relationType;
    }

    public Guid IssueId { get; private set; }

    public Guid RelatedIssueId { get; private set; }

    public IssueRelationType RelationType { get; private set; } = IssueRelationType.BlockedBy;

    public static IssueRelation Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid relatedIssueId, IssueRelationType relationType)
    {
        return new IssueRelation(id, workspaceId, projectId, issueId, relatedIssueId, relationType);
    }

    public void UpdateRelation(IssueRelationType relationType)
    {
        RelationType = relationType;
    }
}

public sealed class IssueMention : ProjectScopedEntity
{
    private IssueMention()
    {
    }

    private IssueMention(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid userId)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        UserId = userId;
    }

    public Guid IssueId { get; private set; }

    public Guid UserId { get; private set; }

    public static IssueMention Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid userId)
    {
        return new IssueMention(id, workspaceId, projectId, issueId, userId);
    }
}

public sealed class IssueAssignee : ProjectScopedEntity
{
    private IssueAssignee()
    {
    }

    private IssueAssignee(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid assigneeId)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        AssigneeId = assigneeId;
    }

    public Guid IssueId { get; private set; }

    public Guid AssigneeId { get; private set; }

    public static IssueAssignee Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid assigneeId)
    {
        return new IssueAssignee(id, workspaceId, projectId, issueId, assigneeId);
    }
}

public sealed class IssueLink : ProjectScopedEntity
{
    private IssueLink()
    {
    }

    private IssueLink(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Url url, StructuredData metadata)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        Url = url;
        Metadata = metadata;
    }

    public Guid IssueId { get; private set; }

    public string? Title { get; private set; }

    public Url Url { get; private set; } = Url.Create("https://example.com");

    public StructuredData Metadata { get; private set; } = StructuredData.FromJson(null);

    public static IssueLink Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Url url, StructuredData metadata)
    {
        return new IssueLink(id, workspaceId, projectId, issueId, url, metadata);
    }

    public void Update(string? title, Url url, StructuredData metadata)
    {
        Title = title;
        Url = url;
        Metadata = metadata;
    }
}

public sealed class IssueAttachment : ProjectScopedEntity
{
    private IssueAttachment()
    {
    }

    private IssueAttachment(Guid id, Guid workspaceId, Guid projectId, Guid issueId, string assetPath, StructuredData attributes)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        AssetPath = assetPath;
        Attributes = attributes;
    }

    public Guid IssueId { get; private set; }

    public StructuredData Attributes { get; private set; } = StructuredData.FromJson(null);

    public string AssetPath { get; private set; } = string.Empty;

    public ExternalReference? ExternalReference { get; private set; }

    public static IssueAttachment Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, string assetPath, StructuredData attributes)
    {
        return new IssueAttachment(id, workspaceId, projectId, issueId, assetPath, attributes);
    }

    public void SetExternalReference(string? source, string? externalId)
    {
        ExternalReference = source is null && externalId is null ? null : ExternalReference.Create(source, externalId);
    }
}

public sealed class IssueActivity : ProjectScopedEntity
{
    private IssueActivity()
    {
    }

    private IssueActivity(Guid id, Guid workspaceId, Guid projectId, Guid? issueId, string verb)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        Verb = verb;
    }

    public Guid? IssueId { get; private set; }

    public string Verb { get; private set; } = "created";

    public string? Field { get; private set; }

    public string? OldValue { get; private set; }

    public string? NewValue { get; private set; }

    public string? Comment { get; private set; }

    public IReadOnlyCollection<Url> Attachments => _attachments;

    public Guid? IssueCommentId { get; private set; }

    public Guid? ActorId { get; private set; }

    public Guid? OldIdentifier { get; private set; }

    public Guid? NewIdentifier { get; private set; }

    public double? Epoch { get; private set; }

    private List<Url> _attachments = new();

    public static IssueActivity Create(Guid id, Guid workspaceId, Guid projectId, Guid? issueId, string verb)
    {
        return new IssueActivity(id, workspaceId, projectId, issueId, verb);
    }

    public void UpdateDetails(string? field, string? oldValue, string? newValue, string? comment, Guid? actorId)
    {
        Field = field;
        OldValue = oldValue;
        NewValue = newValue;
        Comment = comment;
        ActorId = actorId;
    }

    public void SetIdentifiers(Guid? oldIdentifier, Guid? newIdentifier)
    {
        OldIdentifier = oldIdentifier;
        NewIdentifier = newIdentifier;
    }

    public void SetEpoch(double? epoch)
    {
        Epoch = epoch;
    }

    public void LinkComment(Guid? commentId)
    {
        IssueCommentId = commentId;
    }

    public void AddAttachment(Url attachment)
    {
        _attachments.Add(attachment);
    }
}

public sealed class IssueComment : ProjectScopedEntity
{
    private IssueComment()
    {
    }

    private IssueComment(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid? actorId, RichTextContent comment, IssueCommentAccess access)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        ActorId = actorId;
        Comment = comment;
        Access = access;
    }

    public string? CommentStripped { get; private set; }

    public StructuredData CommentJson { get; private set; } = StructuredData.FromJson(null);

    public RichTextContent Comment { get; private set; } = RichTextContent.Create();

    public IReadOnlyCollection<Url> Attachments => _attachments;

    public Guid IssueId { get; private set; }

    public Guid? ActorId { get; private set; }

    public IssueCommentAccess Access { get; private set; } = IssueCommentAccess.Internal;

    public ExternalReference? ExternalReference { get; private set; }

    public DateTime? EditedAt { get; private set; }

    private List<Url> _attachments = new();

    public static IssueComment Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid? actorId, RichTextContent comment, IssueCommentAccess access)
    {
        return new IssueComment(id, workspaceId, projectId, issueId, actorId, comment, access);
    }

    public void UpdateComment(RichTextContent comment, string? stripped, StructuredData json)
    {
        Comment = comment;
        CommentStripped = stripped;
        CommentJson = json;
    }

    public void SetExternalReference(string? source, string? externalId)
    {
        ExternalReference = source is null && externalId is null ? null : ExternalReference.Create(source, externalId);
    }

    public void SetEditedAt(DateTime? editedAt)
    {
        EditedAt = editedAt;
    }

    public void SetAccess(IssueCommentAccess access)
    {
        Access = access;
    }

    public void AddAttachment(Url url)
    {
        _attachments.Add(url);
    }

    public void ReplaceAttachments(IEnumerable<Url> urls)
    {
        _attachments = urls?.ToList() ?? new List<Url>();
    }
}

public sealed class IssueUserProperty : ProjectScopedEntity
{
    private IssueUserProperty()
    {
    }

    private IssueUserProperty(Guid id, Guid workspaceId, Guid projectId, Guid userId, ViewPreferences preferences)
        : base(id, workspaceId, projectId)
    {
        UserId = userId;
        Preferences = preferences;
    }

    public Guid UserId { get; private set; }

    public ViewPreferences Preferences { get; private set; } = ViewPreferences.CreateIssueDefaults();

    public static IssueUserProperty Create(Guid id, Guid workspaceId, Guid projectId, Guid userId, ViewPreferences preferences)
    {
        var effectivePreferences = preferences ?? ViewPreferences.CreateIssueDefaults();
        return new IssueUserProperty(id, workspaceId, projectId, userId, effectivePreferences);
    }

    public void Update(ViewPreferences preferences)
    {
        Preferences = preferences ?? ViewPreferences.CreateIssueDefaults();
    }
}

public sealed class IssueLabel : ProjectScopedEntity
{
    private IssueLabel()
    {
    }

    private IssueLabel(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid labelId)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        LabelId = labelId;
    }

    public Guid IssueId { get; private set; }

    public Guid LabelId { get; private set; }

    public static IssueLabel Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid labelId)
    {
        return new IssueLabel(id, workspaceId, projectId, issueId, labelId);
    }
}

public sealed class IssueSequence : ProjectScopedEntity
{
    private IssueSequence()
    {
    }

    private IssueSequence(Guid id, Guid workspaceId, Guid projectId, Guid? issueId, long sequence)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        Sequence = sequence;
    }

    public Guid? IssueId { get; private set; }

    public long Sequence { get; private set; }

    public bool Deleted { get; private set; }

    public static IssueSequence Create(Guid id, Guid workspaceId, Guid projectId, Guid? issueId, long sequence)
    {
        return new IssueSequence(id, workspaceId, projectId, issueId, sequence);
    }

    public void MarkDeleted(bool deleted)
    {
        Deleted = deleted;
    }
}

public sealed class IssueSubscriber : ProjectScopedEntity
{
    private IssueSubscriber()
    {
    }

    private IssueSubscriber(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid subscriberId)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        SubscriberId = subscriberId;
    }

    public Guid IssueId { get; private set; }

    public Guid SubscriberId { get; private set; }

    public static IssueSubscriber Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid subscriberId)
    {
        return new IssueSubscriber(id, workspaceId, projectId, issueId, subscriberId);
    }
}

public sealed class IssueReaction : ProjectScopedEntity
{
    private IssueReaction()
    {
    }

    private IssueReaction(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid actorId, string reaction)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        ActorId = actorId;
        Reaction = reaction;
    }

    public Guid IssueId { get; private set; }

    public Guid ActorId { get; private set; }

    public string Reaction { get; private set; } = string.Empty;

    public static IssueReaction Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid actorId, string reaction)
    {
        return new IssueReaction(id, workspaceId, projectId, issueId, actorId, reaction);
    }
}

public sealed class CommentReaction : ProjectScopedEntity
{
    private CommentReaction()
    {
    }

    private CommentReaction(Guid id, Guid workspaceId, Guid projectId, Guid commentId, Guid actorId, string reaction)
        : base(id, workspaceId, projectId)
    {
        CommentId = commentId;
        ActorId = actorId;
        Reaction = reaction;
    }

    public Guid CommentId { get; private set; }

    public Guid ActorId { get; private set; }

    public string Reaction { get; private set; } = string.Empty;

    public static CommentReaction Create(Guid id, Guid workspaceId, Guid projectId, Guid commentId, Guid actorId, string reaction)
    {
        return new CommentReaction(id, workspaceId, projectId, commentId, actorId, reaction);
    }
}

public sealed class IssueVote : ProjectScopedEntity
{
    private IssueVote()
    {
    }

    private IssueVote(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid actorId, IssueVoteDirection vote)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        ActorId = actorId;
        Vote = vote;
    }

    public Guid IssueId { get; private set; }

    public Guid ActorId { get; private set; }

    public IssueVoteDirection Vote { get; private set; } = IssueVoteDirection.Upvote;

    public static IssueVote Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid actorId, IssueVoteDirection vote)
    {
        return new IssueVote(id, workspaceId, projectId, issueId, actorId, vote);
    }

    public void UpdateVote(IssueVoteDirection vote)
    {
        Vote = vote;
    }
}

public sealed class IssueVersion : ProjectScopedEntity
{
    private readonly HashSet<Guid> _assigneeIds = new();
    private readonly HashSet<Guid> _labelIds = new();
    private readonly HashSet<Guid> _moduleIds = new();

    private IssueVersion()
    {
    }

    private IssueVersion(Guid id, Guid workspaceId, Guid projectId, Guid issueId, string name, IssuePriority priority, int sequenceId, DateTime recordedAt)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        Name = name;
        Priority = priority;
        SequenceId = sequenceId;
        RecordedAt = recordedAt;
    }

    public Guid IssueId { get; private set; }

    public Guid? ParentId { get; private set; }

    public Guid? StateId { get; private set; }

    public Guid? EstimatePointId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public IssuePriority Priority { get; private set; } = IssuePriority.None;

    public DateTime? StartDate { get; private set; }

    public DateTime? TargetDate { get; private set; }

    public IReadOnlyCollection<Guid> AssigneeIds => _assigneeIds;

    public int SequenceId { get; private set; }

    public IReadOnlyCollection<Guid> LabelIds => _labelIds;

    public double SortOrder { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? ArchivedAt { get; private set; }

    public bool IsDraft { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public Guid? IssueTypeId { get; private set; }

    public Guid? CycleId { get; private set; }

    public IReadOnlyCollection<Guid> ModuleIds => _moduleIds;

    public StructuredData Properties { get; private set; } = StructuredData.FromJson(null);

    public StructuredData Metadata { get; private set; } = StructuredData.FromJson(null);

    public DateTime RecordedAt { get; private set; }

    public Guid? ActivityId { get; private set; }

    public Guid OwnerId { get; private set; }

    public static IssueVersion Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, string name, IssuePriority priority, int sequenceId, DateTime recordedAt, Guid ownerId)
    {
        var version = new IssueVersion(id, workspaceId, projectId, issueId, name, priority, sequenceId, recordedAt)
        {
            OwnerId = ownerId,
        };
        return version;
    }

    public void SetContext(Guid? parentId, Guid? stateId, Guid? estimatePointId, DateTime? startDate, DateTime? targetDate, double sortOrder, bool isDraft)
    {
        ParentId = parentId;
        StateId = stateId;
        EstimatePointId = estimatePointId;
        StartDate = startDate;
        TargetDate = targetDate;
        SortOrder = sortOrder;
        IsDraft = isDraft;
    }

    public void SetLifecycle(DateTime? completedAt, DateTime? archivedAt)
    {
        CompletedAt = completedAt;
        ArchivedAt = archivedAt;
    }

    public void SetExternalReference(string? source, string? identifier)
    {
        ExternalReference = source is null && identifier is null ? null : ExternalReference.Create(source, identifier);
    }

    public void SetIssueType(Guid? issueTypeId)
    {
        IssueTypeId = issueTypeId;
    }

    public void SetCycle(Guid? cycleId)
    {
        CycleId = cycleId;
    }

    public void AddAssignees(IEnumerable<Guid> assigneeIds)
    {
        foreach (var id in assigneeIds)
        {
            _assigneeIds.Add(id);
        }
    }

    public void AddLabels(IEnumerable<Guid> labelIds)
    {
        foreach (var id in labelIds)
        {
            _labelIds.Add(id);
        }
    }

    public void AddModules(IEnumerable<Guid> moduleIds)
    {
        foreach (var id in moduleIds)
        {
            _moduleIds.Add(id);
        }
    }

    public void SetProperties(StructuredData properties, StructuredData metadata)
    {
        Properties = properties;
        Metadata = metadata;
    }

    public void LinkActivity(Guid? activityId)
    {
        ActivityId = activityId;
    }
}

public sealed class IssueDescriptionVersion : ProjectScopedEntity
{
    private IssueDescriptionVersion()
    {
    }

    private IssueDescriptionVersion(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid ownerId, DateTime recordedAt, RichTextContent description)
        : base(id, workspaceId, projectId)
    {
        IssueId = issueId;
        OwnerId = ownerId;
        RecordedAt = recordedAt;
        Description = description;
    }

    public Guid IssueId { get; private set; }

    public RichTextContent Description { get; private set; } = RichTextContent.Create();

    public DateTime RecordedAt { get; private set; }

    public Guid OwnerId { get; private set; }

    public static IssueDescriptionVersion Create(Guid id, Guid workspaceId, Guid projectId, Guid issueId, Guid ownerId, DateTime recordedAt, RichTextContent description)
    {
        return new IssueDescriptionVersion(id, workspaceId, projectId, issueId, ownerId, recordedAt, description);
    }
}
