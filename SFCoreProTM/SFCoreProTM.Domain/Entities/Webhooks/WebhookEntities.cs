using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Webhooks;

public sealed class Webhook : WorkspaceScopedEntity
{
    private Webhook()
    {
    }

    private Webhook(Guid id, Guid workspaceId, Url endpoint, string secret)
        : base(id, workspaceId)
    {
        Endpoint = endpoint;
        SecretKey = secret;
    }

    public Url Endpoint { get; private set; } = Url.Create("https://example.com");

    public bool IsActive { get; private set; } = true;

    public string SecretKey { get; private set; } = string.Empty;

    public bool ProjectEvents { get; private set; }

    public bool IssueEvents { get; private set; }

    public bool ModuleEvents { get; private set; }

    public bool CycleEvents { get; private set; }

    public bool IssueCommentEvents { get; private set; }

    public bool IsInternal { get; private set; }

    public static Webhook Create(Guid id, Guid workspaceId, Url endpoint, string secret)
    {
        return new Webhook(id, workspaceId, endpoint, secret);
    }

    public void ConfigureSubscriptions(bool project, bool issue, bool module, bool cycle, bool issueComment, bool isInternal)
    {
        ProjectEvents = project;
        IssueEvents = issue;
        ModuleEvents = module;
        CycleEvents = cycle;
        IssueCommentEvents = issueComment;
        IsInternal = isInternal;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }
}

public sealed class WebhookLog : Entity
{
    private WebhookLog()
    {
    }

    private WebhookLog(Guid id, Guid workspaceId, Guid webhookId)
        : base(id)
    {
        WorkspaceId = workspaceId;
        WebhookId = webhookId;
    }

    public Guid WorkspaceId { get; private set; }

    public Guid WebhookId { get; private set; }

    public string? EventType { get; private set; }

    public string? RequestMethod { get; private set; }

    public string? RequestHeaders { get; private set; }

    public string? RequestBody { get; private set; }

    public string? ResponseStatus { get; private set; }

    public string? ResponseHeaders { get; private set; }

    public string? ResponseBody { get; private set; }

    public int RetryCount { get; private set; }

    public static WebhookLog Create(Guid id, Guid workspaceId, Guid webhookId)
    {
        return new WebhookLog(id, workspaceId, webhookId);
    }

    public void SetRequest(string? eventType, string? method, string? headers, string? body)
    {
        EventType = eventType;
        RequestMethod = method;
        RequestHeaders = headers;
        RequestBody = body;
    }

    public void SetResponse(string? status, string? headers, string? body)
    {
        ResponseStatus = status;
        ResponseHeaders = headers;
        ResponseBody = body;
    }

    public void IncrementRetry()
    {
        RetryCount += 1;
    }
}

public sealed class ProjectWebhook : ProjectScopedEntity
{
    private ProjectWebhook()
    {
    }

    private ProjectWebhook(Guid id, Guid workspaceId, Guid projectId, Guid webhookId)
        : base(id, workspaceId, projectId)
    {
        WebhookId = webhookId;
    }

    public Guid WebhookId { get; private set; }

    public static ProjectWebhook Create(Guid id, Guid workspaceId, Guid projectId, Guid webhookId)
    {
        return new ProjectWebhook(id, workspaceId, projectId, webhookId);
    }
}
