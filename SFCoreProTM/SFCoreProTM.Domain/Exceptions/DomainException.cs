
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SFCoreProTM.Domain.Exceptions;

public class DomainException : Exception
{
    private static readonly IReadOnlyDictionary<string, string[]> EmptyErrors =
        new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase));

    public DomainException()
        : this("A domain rule was violated.", null, null)
    {
    }

    public DomainException(string message)
        : this(message, null, null)
    {
    }

    public DomainException(string message, Exception? innerException)
        : this(message, null, innerException)
    {
    }

    public DomainException(string message, IDictionary<string, string[]>? errors, Exception? innerException = null)
        : base(string.IsNullOrWhiteSpace(message) ? "A domain rule was violated." : message, innerException)
    {
        Errors = errors is null
            ? EmptyErrors
            : new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>(errors, StringComparer.OrdinalIgnoreCase));
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
