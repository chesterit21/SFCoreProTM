using System;
using System.Collections.Generic;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class Url : ValueObject
{
    private Url()
        : this(string.Empty)
    {
    }

    private Url(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Url Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!Uri.TryCreate(value, UriKind.Absolute, out _))
        {
            throw new ArgumentException("Invalid URL.", nameof(value));
        }

        return new Url(value);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
