using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Api;

public sealed class ApiToken : AggregateRoot
{
    private ApiToken()
    {
    }

    private ApiToken(Guid id, string label, Guid userId, string tokenHash, int userType)
        : base(id)
    {
        Label = label;
        UserId = userId;
        TokenHash = tokenHash;
        UserType = userType;
    }

    public string Label { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime? LastUsed { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public Guid UserId { get; private set; }

    public int UserType { get; private set; }

    public Guid? WorkspaceId { get; private set; }

    public DateTime? ExpiredAt { get; private set; }

    public bool IsServiceAccountToken { get; private set; }

    public static ApiToken Create(Guid id, string label, Guid userId, string tokenHash, int userType)
    {
        return new ApiToken(id, label, userId, tokenHash, userType);
    }

    public void UpdateMetadata(string? description, bool isServiceAccountToken, Guid? workspaceId)
    {
        Description = description;
        IsServiceAccountToken = isServiceAccountToken;
        WorkspaceId = workspaceId;
    }

    public void MarkUsed(DateTime usedAt)
    {
        LastUsed = usedAt;
    }

    public void SetExpiration(DateTime? expiredAt)
    {
        ExpiredAt = expiredAt;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

public sealed class ApiActivityLog : Entity
{
    private ApiActivityLog()
    {
    }

    private ApiActivityLog(Guid id, string tokenIdentifier, string path, string method, StructuredData queryParams, StructuredData headers, string body, int responseCode)
        : base(id)
    {
        TokenIdentifier = tokenIdentifier;
        Path = path;
        Method = method;
        QueryParams = queryParams;
        Headers = headers;
        RequestBody = body;
        ResponseCode = responseCode;
    }

    public string TokenIdentifier { get; private set; } = string.Empty;

    public string Path { get; private set; } = string.Empty;

    public string Method { get; private set; } = "GET";

    public StructuredData QueryParams { get; private set; } = StructuredData.FromJson(null);

    public StructuredData Headers { get; private set; } = StructuredData.FromJson(null);

    public string RequestBody { get; private set; } = string.Empty;

    public int ResponseCode { get; private set; }

    public string? ResponseBody { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public static ApiActivityLog Create(Guid id, string tokenIdentifier, string path, string method, StructuredData queryParams, StructuredData headers, string requestBody, int responseCode)
    {
        return new ApiActivityLog(id, tokenIdentifier, path, method, queryParams, headers, requestBody, responseCode);
    }

    public void AppendResponse(string? responseBody, int responseCode)
    {
        ResponseBody = responseBody;
        ResponseCode = responseCode;
    }

    public void SetRequestContext(string? ipAddress, string? userAgent)
    {
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
}
