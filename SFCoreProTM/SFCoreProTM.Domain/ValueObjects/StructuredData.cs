using System.Collections.Generic;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class StructuredData : ValueObject
{
    private StructuredData()
        : this(null)
    {
    }

    private StructuredData(string? rawJson)
    {
        RawJson = rawJson;
    }

    public string? RawJson { get; }

    public static StructuredData FromJson(string? rawJson) => new(rawJson);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return RawJson;
    }
}
