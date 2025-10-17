using System;
using System.Collections.Generic;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class DateRange : ValueObject
{
    private DateRange()
        : this(null, null)
    {
    }

    private DateRange(DateTime? start, DateTime? end)
    {
        if (start.HasValue && end.HasValue && end < start)
        {
            throw new ArgumentException("End date cannot be earlier than start date.");
        }

        Start = start;
        End = end;
    }

    public DateTime? Start { get; }

    public DateTime? End { get; }

    public static DateRange Create(DateTime? start, DateTime? end) => new(start, end);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}
