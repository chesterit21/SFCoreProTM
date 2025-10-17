using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class ColorCode : ValueObject
{
    private static readonly Regex HexPattern = new("^#(?:[0-9a-fA-F]{3}){1,2}$", RegexOptions.Compiled);

    private ColorCode()
        : this("#000000")
    {
    }

    private ColorCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ColorCode FromHex(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!HexPattern.IsMatch(value))
        {
            throw new ArgumentException("Color must be a valid hex code.", nameof(value));
        }

        return new ColorCode(value);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
