using System;
using SFCoreProTM.Application.Interfaces;

namespace SFCoreProTM.Persistence.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
