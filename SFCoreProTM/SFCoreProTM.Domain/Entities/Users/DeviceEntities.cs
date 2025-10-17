using System;
using SFCoreProTM.Domain.Entities;

namespace SFCoreProTM.Domain.Entities.Users;

public sealed class Device : Entity
{
    private Device()
    {
    }

    private Device(Guid id, Guid userId, string deviceIdentifier, string deviceType)
        : base(id)
    {
        UserId = userId;
        DeviceIdentifier = deviceIdentifier;
        DeviceType = deviceType;
    }

    public Guid UserId { get; private set; }

    public string DeviceIdentifier { get; private set; } = string.Empty;

    public string DeviceType { get; private set; } = string.Empty;

    public string? PushToken { get; private set; }

    public bool IsActive { get; private set; } = true;

    public static Device Create(Guid id, Guid userId, string deviceIdentifier, string deviceType)
    {
        return new Device(id, userId, deviceIdentifier, deviceType);
    }

    public void UpdatePushToken(string? pushToken)
    {
        PushToken = pushToken;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }
}

public sealed class DeviceSession : Entity
{
    private DeviceSession()
    {
    }

    private DeviceSession(Guid id, Guid deviceId, Guid sessionId, string userAgent, string ipAddress, DateTime startTime)
        : base(id)
    {
        DeviceId = deviceId;
        SessionId = sessionId;
        UserAgent = userAgent;
        IpAddress = ipAddress;
        StartTime = startTime;
    }

    public Guid DeviceId { get; private set; }

    public Guid SessionId { get; private set; }

    public bool IsActive { get; private set; } = true;

    public string UserAgent { get; private set; } = string.Empty;

    public string IpAddress { get; private set; } = string.Empty;

    public DateTime StartTime { get; private set; }

    public DateTime? EndTime { get; private set; }

    public static DeviceSession Create(Guid id, Guid deviceId, Guid sessionId, string userAgent, string ipAddress, DateTime startTime)
    {
        return new DeviceSession(id, deviceId, sessionId, userAgent, ipAddress, startTime);
    }

    public void Close(DateTime endTime)
    {
        EndTime = endTime;
        IsActive = false;
    }
}
