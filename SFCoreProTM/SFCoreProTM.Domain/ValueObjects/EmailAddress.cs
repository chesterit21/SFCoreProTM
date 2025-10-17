using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class EmailAddress : ValueObject
{
    private EmailAddress()
        : this(string.Empty)
    {
    }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static EmailAddress Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _ = new MailAddress(value);
        return new EmailAddress(value);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
