using System;
using System.Security.Cryptography;
using System.Text;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Users;

public sealed class UserSession : Entity
{
    private UserSession()
    {
    }

    private UserSession(Guid id, string sessionKey, StructuredData? deviceInfo, string? userId)
        : base(id)
    {
        SessionKey = sessionKey;
        DeviceInfo = deviceInfo;
        UserId = userId;
    }

    public string SessionKey { get; private set; } = string.Empty;

    public StructuredData? DeviceInfo { get; private set; }

    public string? UserId { get; private set; }

    public static UserSession Create(Guid id, string sessionKey, StructuredData? deviceInfo, string? userId)
    {
        return new UserSession(id, sessionKey, deviceInfo, userId);
    }

    public void AttachDeviceInfo(StructuredData? deviceInfo)
    {
        DeviceInfo = deviceInfo;
    }

    public void AttachUser(string? userId)
    {
        UserId = userId;
    }
}

public sealed class SessionKeyGenerator
{
    private const string ValidCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

    public string Generate(int length = 128)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        var bytes = RandomNumberGenerator.GetBytes(length);
        var builder = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            var index = bytes[i] % ValidCharacters.Length;
            builder.Append(ValidCharacters[index]);
        }

        return builder.ToString();
    }
}
