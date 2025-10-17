using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Integrations;

public enum IntegrationVisibility
{
    Private = 1,
    Public = 2,
}

public sealed class Integration : AggregateRoot
{
    private Integration()
    {
    }

    private Integration(Guid id, string title, string provider, IntegrationVisibility visibility)
        : base(id)
    {
        Title = title;
        Provider = provider;
        Visibility = visibility;
    }

    public string Title { get; private set; } = string.Empty;

    public string Provider { get; private set; } = string.Empty;

    public IntegrationVisibility Visibility { get; private set; } = IntegrationVisibility.Private;

    public StructuredData Description { get; private set; } = StructuredData.FromJson(null);

    public string? Author { get; private set; }

    public Url? WebhookUrl { get; private set; }

    public string? WebhookSecret { get; private set; }

    public Url? RedirectUrl { get; private set; }

    public StructuredData Metadata { get; private set; } = StructuredData.FromJson(null);

    public bool Verified { get; private set; }

    public Url? AvatarUrl { get; private set; }

    public static Integration Create(Guid id, string title, string provider, IntegrationVisibility visibility)
    {
        return new Integration(id, title, provider, visibility);
    }

    public void UpdateDetails(string title, StructuredData description, string? author, IntegrationVisibility visibility)
    {
        Title = title;
        Description = description;
        Author = author;
        Visibility = visibility;
    }

    public void ConfigureWebhook(Url? webhookUrl, string? webhookSecret, Url? redirectUrl)
    {
        WebhookUrl = webhookUrl;
        WebhookSecret = webhookSecret;
        RedirectUrl = redirectUrl;
    }

    public void SetMetadata(StructuredData metadata, Url? avatarUrl, bool verified)
    {
        Metadata = metadata;
        AvatarUrl = avatarUrl;
        Verified = verified;
    }
}

public sealed class WorkspaceIntegration : WorkspaceScopedEntity
{
    private WorkspaceIntegration()
    {
    }

    private WorkspaceIntegration(Guid id, Guid workspaceId, Guid actorId, Guid integrationId, Guid apiTokenId, StructuredData metadata, StructuredData config)
        : base(id, workspaceId)
    {
        ActorId = actorId;
        IntegrationId = integrationId;
        ApiTokenId = apiTokenId;
        Metadata = metadata;
        Config = config;
    }

    public Guid ActorId { get; private set; }

    public Guid IntegrationId { get; private set; }

    public Guid ApiTokenId { get; private set; }

    public StructuredData Metadata { get; private set; } = StructuredData.FromJson(null);

    public StructuredData Config { get; private set; } = StructuredData.FromJson(null);

    public static WorkspaceIntegration Create(Guid id, Guid workspaceId, Guid actorId, Guid integrationId, Guid apiTokenId, StructuredData metadata, StructuredData config)
    {
        return new WorkspaceIntegration(id, workspaceId, actorId, integrationId, apiTokenId, metadata, config);
    }

    public void UpdateConfiguration(StructuredData metadata, StructuredData config)
    {
        Metadata = metadata;
        Config = config;
    }
}

public sealed class SlackProjectSync : ProjectScopedEntity
{
    private SlackProjectSync()
    {
    }

    private SlackProjectSync(Guid id, Guid workspaceId, Guid projectId, string accessToken, string scopes, string botUserId, Url webhookUrl, string teamId, string teamName, Guid workspaceIntegrationId, StructuredData data)
        : base(id, workspaceId, projectId)
    {
        AccessToken = accessToken;
        Scopes = scopes;
        BotUserId = botUserId;
        WebhookUrl = webhookUrl;
        TeamId = teamId;
        TeamName = teamName;
        WorkspaceIntegrationId = workspaceIntegrationId;
        Data = data;
    }

    public string AccessToken { get; private set; } = string.Empty;

    public string Scopes { get; private set; } = string.Empty;

    public string BotUserId { get; private set; } = string.Empty;

    public Url WebhookUrl { get; private set; } = Url.Create("https://hooks.slack.com/services/test");

    public StructuredData Data { get; private set; } = StructuredData.FromJson(null);

    public string TeamId { get; private set; } = string.Empty;

    public string TeamName { get; private set; } = string.Empty;

    public Guid WorkspaceIntegrationId { get; private set; }

    public static SlackProjectSync Create(Guid id, Guid workspaceId, Guid projectId, string accessToken, string scopes, string botUserId, Url webhookUrl, string teamId, string teamName, Guid workspaceIntegrationId, StructuredData data)
    {
        return new SlackProjectSync(id, workspaceId, projectId, accessToken, scopes, botUserId, webhookUrl, teamId, teamName, workspaceIntegrationId, data);
    }
}

public sealed class GithubRepository : ProjectScopedEntity
{
    private GithubRepository()
    {
    }

    private GithubRepository(Guid id, Guid workspaceId, Guid projectId, string name, Url? url, StructuredData config, long repositoryId, string owner)
        : base(id, workspaceId, projectId)
    {
        Name = name;
        Url = url;
        Config = config;
        RepositoryId = repositoryId;
        Owner = owner;
    }

    public string Name { get; private set; } = string.Empty;

    public Url? Url { get; private set; }

    public StructuredData Config { get; private set; } = StructuredData.FromJson(null);

    public long RepositoryId { get; private set; }

    public string Owner { get; private set; } = string.Empty;

    public static GithubRepository Create(Guid id, Guid workspaceId, Guid projectId, string name, Url? url, StructuredData config, long repositoryId, string owner)
    {
        return new GithubRepository(id, workspaceId, projectId, name, url, config, repositoryId, owner);
    }
}

public sealed class GithubRepositorySync : ProjectScopedEntity
{
    private GithubRepositorySync()
    {
    }

    private GithubRepositorySync(Guid id, Guid workspaceId, Guid projectId, Guid repositoryId, StructuredData credentials, Guid actorId, Guid workspaceIntegrationId, Guid? labelId)
        : base(id, workspaceId, projectId)
    {
        RepositoryId = repositoryId;
        Credentials = credentials;
        ActorId = actorId;
        WorkspaceIntegrationId = workspaceIntegrationId;
        LabelId = labelId;
    }

    public Guid RepositoryId { get; private set; }

    public StructuredData Credentials { get; private set; } = StructuredData.FromJson(null);

    public Guid ActorId { get; private set; }

    public Guid WorkspaceIntegrationId { get; private set; }

    public Guid? LabelId { get; private set; }

    public static GithubRepositorySync Create(Guid id, Guid workspaceId, Guid projectId, Guid repositoryId, StructuredData credentials, Guid actorId, Guid workspaceIntegrationId, Guid? labelId)
    {
        return new GithubRepositorySync(id, workspaceId, projectId, repositoryId, credentials, actorId, workspaceIntegrationId, labelId);
    }
}

public sealed class GithubIssueSync : ProjectScopedEntity
{
    private GithubIssueSync()
    {
    }

    private GithubIssueSync(Guid id, Guid workspaceId, Guid projectId, long repositoryIssueId, long githubIssueId, Url issueUrl, Guid issueId, Guid repositorySyncId)
        : base(id, workspaceId, projectId)
    {
        RepositoryIssueId = repositoryIssueId;
        GithubIssueId = githubIssueId;
        IssueUrl = issueUrl;
        IssueId = issueId;
        RepositorySyncId = repositorySyncId;
    }

    public long RepositoryIssueId { get; private set; }

    public long GithubIssueId { get; private set; }

    public Url IssueUrl { get; private set; } = Url.Create("https://github.com");

    public Guid IssueId { get; private set; }

    public Guid RepositorySyncId { get; private set; }

    public static GithubIssueSync Create(Guid id, Guid workspaceId, Guid projectId, long repositoryIssueId, long githubIssueId, Url issueUrl, Guid issueId, Guid repositorySyncId)
    {
        return new GithubIssueSync(id, workspaceId, projectId, repositoryIssueId, githubIssueId, issueUrl, issueId, repositorySyncId);
    }
}

public sealed class GithubCommentSync : ProjectScopedEntity
{
    private GithubCommentSync()
    {
    }

    private GithubCommentSync(Guid id, Guid workspaceId, Guid projectId, long repositoryCommentId, Guid commentId, Guid issueSyncId)
        : base(id, workspaceId, projectId)
    {
        RepositoryCommentId = repositoryCommentId;
        CommentId = commentId;
        IssueSyncId = issueSyncId;
    }

    public long RepositoryCommentId { get; private set; }

    public Guid CommentId { get; private set; }

    public Guid IssueSyncId { get; private set; }

    public static GithubCommentSync Create(Guid id, Guid workspaceId, Guid projectId, long repositoryCommentId, Guid commentId, Guid issueSyncId)
    {
        return new GithubCommentSync(id, workspaceId, projectId, repositoryCommentId, commentId, issueSyncId);
    }
}
