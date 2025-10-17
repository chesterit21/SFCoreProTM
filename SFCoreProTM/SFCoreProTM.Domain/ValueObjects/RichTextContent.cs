using System;
using System.Collections.Generic;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class RichTextContent : ValueObject
{
    private RichTextContent()
        : this(null, null, null, null)
    {
    }

    private RichTextContent(string? plainText, string? html, byte[]? binary, string? json)
    {
        PlainText = plainText;
        Html = html;
        Binary = binary;
        Json = json;
    }

    public string? PlainText { get; }

    public string? Html { get; }

    public byte[]? Binary { get; }

    public string? Json { get; }

    public static RichTextContent Create(string? plainText = null, string? html = null, byte[]? binary = null, string? json = null)
    {
        return new RichTextContent(plainText, html, binary, json);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PlainText;
        yield return Html;
        yield return Json;
        yield return Binary is null ? null : Convert.ToBase64String(Binary);
    }
}
