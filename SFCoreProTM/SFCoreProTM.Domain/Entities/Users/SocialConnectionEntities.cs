using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Users;

public sealed class SocialLoginConnection : Entity
{
    private SocialLoginConnection()
    {
    }

    private SocialLoginConnection(Guid id, Guid userId, string medium)
        : base(id)
    {
        UserId = userId;
        Medium = medium;
    }

    public Guid UserId { get; private set; }

    public string Medium { get; private set; } = string.Empty;

    public DateTime? LastLoginAt { get; private set; }

    public DateTime? LastReceivedAt { get; private set; }

    public StructuredData TokenData { get; private set; } = StructuredData.FromJson(null);

    public StructuredData ExtraData { get; private set; } = StructuredData.FromJson(null);

    public static SocialLoginConnection Create(Guid id, Guid userId, string medium)
    {
        return new SocialLoginConnection(id, userId, medium);
    }

    public void UpdateTimestamps(DateTime? lastLoginAt, DateTime? lastReceivedAt)
    {
        LastLoginAt = lastLoginAt;
        LastReceivedAt = lastReceivedAt;
    }

    public void UpdateTokens(StructuredData tokenData, StructuredData extraData)
    {
        TokenData = tokenData;
        ExtraData = extraData;
    }
}
