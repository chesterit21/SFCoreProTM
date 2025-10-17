using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class Slug : ValueObject
{
    private static readonly Regex SlugPattern = new("^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled);

    private Slug()
        : this(string.Empty)
    {
    }

    private Slug(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Slug Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!SlugPattern.IsMatch(value))
        {
            throw new ArgumentException("Slug must be lowercase alphanumeric words separated by hyphen.", nameof(value));
        }

        return new Slug(value);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
