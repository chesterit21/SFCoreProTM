using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Notifications;

public sealed class Notification : AggregateRoot
{
    private Notification()
    {
    }

    private Notification(Guid id, Guid workspaceId, Guid receiverId, string entityName, string title, RichTextContent message)
        : base(id)
    {
        WorkspaceId = workspaceId;
        ReceiverId = receiverId;
        EntityName = entityName;
        Title = title;
        Message = message;
    }

    public Guid WorkspaceId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public StructuredData Data { get; private set; } = StructuredData.FromJson(null);

    public Guid? EntityIdentifier { get; private set; }

    public string EntityName { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public RichTextContent Message { get; private set; } = RichTextContent.Create();

    public string Sender { get; private set; } = string.Empty;

    public Guid? TriggeredById { get; private set; }

    public Guid ReceiverId { get; private set; }

    public DateTime? ReadAt { get; private set; }

    public DateTime? SnoozedTill { get; private set; }

    public DateTime? ArchivedAt { get; private set; }

    public static Notification Create(Guid id, Guid workspaceId, Guid receiverId, string entityName, string title, RichTextContent message)
    {
        return new Notification(id, workspaceId, receiverId, entityName, title, message);
    }

    public void SetContext(Guid? projectId, Guid? entityIdentifier, StructuredData data)
    {
        ProjectId = projectId;
        EntityIdentifier = entityIdentifier;
        Data = data;
    }

    public void SetSender(string sender, Guid? triggeredById)
    {
        Sender = sender;
        TriggeredById = triggeredById;
    }

    public void MarkRead(DateTime readAt)
    {
        ReadAt = readAt;
    }

    public void Snooze(DateTime? snoozedTill)
    {
        SnoozedTill = snoozedTill;
    }

    public void Archive(DateTime? archivedAt)
    {
        ArchivedAt = archivedAt;
    }

    public void UpdateMessage(RichTextContent message)
    {
        Message = message;
    }
}

public sealed class UserNotificationPreference : Entity
{
    private UserNotificationPreference()
    {
    }

    private UserNotificationPreference(Guid id, Guid userId)
        : base(id)
    {
        UserId = userId;
    }

    public Guid UserId { get; private set; }

    public Guid? WorkspaceId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public bool PropertyChange { get; private set; } = true;

    public bool StateChange { get; private set; } = true;

    public bool Comment { get; private set; } = true;

    public bool Mention { get; private set; } = true;

    public bool IssueCompleted { get; private set; } = true;

    public static UserNotificationPreference Create(Guid id, Guid userId)
    {
        return new UserNotificationPreference(id, userId);
    }

    public void SetScope(Guid? workspaceId, Guid? projectId)
    {
        WorkspaceId = workspaceId;
        ProjectId = projectId;
    }

    public void UpdatePreferences(bool propertyChange, bool stateChange, bool comment, bool mention, bool issueCompleted)
    {
        PropertyChange = propertyChange;
        StateChange = stateChange;
        Comment = comment;
        Mention = mention;
        IssueCompleted = issueCompleted;
    }
}

public sealed class EmailNotificationLog : Entity
{
    private EmailNotificationLog()
    {
    }

    private EmailNotificationLog(Guid id, Guid receiverId, Guid triggeredById, string entityName, string entity)
        : base(id)
    {
        ReceiverId = receiverId;
        TriggeredById = triggeredById;
        EntityName = entityName;
        Entity = entity;
    }

    public Guid ReceiverId { get; private set; }

    public Guid TriggeredById { get; private set; }

    public Guid? EntityIdentifier { get; private set; }

    public string EntityName { get; private set; } = string.Empty;

    public StructuredData Data { get; private set; } = StructuredData.FromJson(null);

    public DateTime? ProcessedAt { get; private set; }

    public DateTime? SentAt { get; private set; }

    public string Entity { get; private set; } = string.Empty;

    public string? OldValue { get; private set; }

    public string? NewValue { get; private set; }

    public static EmailNotificationLog Create(Guid id, Guid receiverId, Guid triggeredById, string entityName, string entity)
    {
        return new EmailNotificationLog(id, receiverId, triggeredById, entityName, entity);
    }

    public void SetEntityIdentifier(Guid? entityIdentifier)
    {
        EntityIdentifier = entityIdentifier;
    }

    public void UpdateData(StructuredData data, string? oldValue, string? newValue)
    {
        Data = data;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public void MarkProcessed(DateTime processedAt, DateTime? sentAt)
    {
        ProcessedAt = processedAt;
        SentAt = sentAt;
    }
}
