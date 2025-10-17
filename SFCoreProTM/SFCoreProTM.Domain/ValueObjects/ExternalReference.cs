using System.Collections.Generic;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class ExternalReference : ValueObject
{
    private ExternalReference()
        : this(null, null)
    {
    }

    private ExternalReference(string? source, string? identifier)
    {
        Source = source;
        Identifier = identifier;
    }

    public string? Source { get; }

    public string? Identifier { get; }

    public static ExternalReference Create(string? source, string? identifier) => new(source, identifier);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Source;
        yield return Identifier;
    }
}
