
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Persistence.Data;

public static class ValueConverters
{
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private static readonly HashSet<Guid> EmptyGuidSet = new();

    public static readonly ValueConverter<AuditTrail, string?> AuditTrailConverter = new(
        value => JsonSerializer.Serialize(
            new AuditTrailSurrogate(value.CreatedAt, value.CreatedById, value.UpdatedAt, value.UpdatedById, value.DeletedAt),
            JsonOptions),
        value => DeserializeAuditTrail(value));

    public static readonly ValueComparer<AuditTrail> AuditTrailComparer = new(
        (left, right) => left != null && right != null &&
            left.CreatedAt == right.CreatedAt &&
            left.CreatedById == right.CreatedById &&
            left.UpdatedAt == right.UpdatedAt &&
            left.UpdatedById == right.UpdatedById &&
            left.DeletedAt == right.DeletedAt,
        value => value == null ? 0 : HashCode.Combine(value.CreatedAt, value.CreatedById, value.UpdatedAt, value.UpdatedById, value.DeletedAt),
        value => value == null ? AuditTrail.Empty : AuditTrail.Create(value.CreatedAt, value.CreatedById, value.UpdatedAt, value.UpdatedById, value.DeletedAt));

    public static readonly ValueConverter<HashSet<Guid>, Guid[]> GuidSetConverter = new(
        set => set == null ? Array.Empty<Guid>() : set.ToArray(),
        array => array == null ? new HashSet<Guid>() : new HashSet<Guid>(array));

    public static readonly ValueComparer<HashSet<Guid>> GuidSetComparer = new(
        (left, right) => (left ?? EmptyGuidSet).SetEquals(right ?? EmptyGuidSet),
        set => set == null ? 0 : set.Aggregate(0, (current, guid) => HashCode.Combine(current, guid.GetHashCode())),
        set => set == null ? new HashSet<Guid>() : new HashSet<Guid>(set));

    public static readonly ValueConverter<EmailAddress, string> NonNullEmailAddressConverter = new(
        value => value.Value,
        value => EmailAddress.Create(value));

    public static readonly ValueComparer<EmailAddress> NonNullEmailAddressComparer = new(
        (left, right) => left != null && right != null && string.Equals(left.Value, right.Value, StringComparison.OrdinalIgnoreCase),
        value => StringComparer.OrdinalIgnoreCase.GetHashCode(value.Value),
        value => EmailAddress.Create(value.Value));

    public static readonly ValueConverter<EmailAddress?, string?> EmailAddressConverter = new(
        value => value == null ? null : value.Value,
        value => string.IsNullOrWhiteSpace(value)
            ? null
            : EmailAddress.Create(value));

    public static readonly ValueComparer<EmailAddress?> EmailAddressComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null && string.Equals(left.Value, right.Value, StringComparison.OrdinalIgnoreCase)),
        value => value == null
            ? 0
            : StringComparer.OrdinalIgnoreCase.GetHashCode(value.Value),
        value => value == null
            ? null
            : EmailAddress.Create(value.Value));

    public static readonly ValueConverter<StructuredData?, string?> StructuredDataConverter = new(
        value => value == null ? null : value.RawJson,
        json => StructuredData.FromJson(json));

    public static readonly ValueComparer<StructuredData?> StructuredDataComparer = new(
        (left, right) => string.Equals(left == null ? null : left.RawJson, right == null ? null : right.RawJson, StringComparison.Ordinal),
        value => value == null || value.RawJson == null ? 0 : StringComparer.Ordinal.GetHashCode(value.RawJson),
        value => value == null ? StructuredData.FromJson(null) : StructuredData.FromJson(value.RawJson));

    public static readonly ValueConverter<RichTextContent, string?> RichTextContentConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(
                new RichTextContentSurrogate(
                    value.PlainText,
                    value.Html,
                    value.Json,
                    value.Binary == null ? null : Convert.ToBase64String(value.Binary)),
                JsonOptions),
        json => DeserializeRichText(json));

    public static readonly ValueComparer<RichTextContent> RichTextContentComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             string.Equals(left.PlainText, right.PlainText, StringComparison.Ordinal) &&
             string.Equals(left.Html, right.Html, StringComparison.Ordinal) &&
             string.Equals(left.Json, right.Json, StringComparison.Ordinal) &&
             StructuralComparisons.StructuralEqualityComparer.Equals(left.Binary, right.Binary)),
        value => value == null ? 0 : HashCode.Combine(value.PlainText, value.Html, value.Json, value.Binary),
        value => value == null ? RichTextContent.Create(null, null, null, null) : RichTextContent.Create(value.PlainText, value.Html, value.Binary, value.Json));

    public static readonly ValueConverter<DateRange, string?> DateRangeConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(new DateRangeSurrogate(value.Start, value.End), JsonOptions),
        json => DeserializeDateRange(json));

    public static readonly ValueComparer<DateRange> DateRangeComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             Nullable.Equals(left.Start, right.Start) &&
             Nullable.Equals(left.End, right.End)),
        value => value == null ? 0 : HashCode.Combine(value.Start, value.End),
        value => value == null ? DateRange.Create(null, null) : DateRange.Create(value.Start, value.End));

    public static readonly ValueConverter<ExternalReference?, string?> ExternalReferenceConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(new ExternalReferenceSurrogate(value.Source, value.Identifier), JsonOptions),
        json => DeserializeExternalReference(json));

    public static readonly ValueComparer<ExternalReference?> ExternalReferenceComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             string.Equals(left.Source, right.Source, StringComparison.Ordinal) &&
             string.Equals(left.Identifier, right.Identifier, StringComparison.Ordinal)),
        value => value == null ? 0 : HashCode.Combine(value.Source, value.Identifier),
        value => value == null ? null : ExternalReference.Create(value.Source, value.Identifier));

    public static readonly ValueConverter<List<Url>, string?> UrlListConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(value.Select(url => url.Value), JsonOptions),
        json => DeserializeUrlList(json));

    public static readonly ValueComparer<List<Url>> UrlListComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             left.Count == right.Count &&
             left.Select(url => url.Value).SequenceEqual(right.Select(url => url.Value), StringComparer.Ordinal)),
        value => value == null
            ? 0
            : value.Aggregate(0, (current, url) => HashCode.Combine(current, StringComparer.Ordinal.GetHashCode(url.Value))),
        value => value == null
            ? new List<Url>()
            : value.Select(url => Url.Create(url.Value)).ToList());

    public static readonly ValueConverter<Url?, string?> UrlConverter = new(
        value => value == null ? null : value.Value,
        json => DeserializeUrl(json));

    public static readonly ValueComparer<Url?> UrlComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null && string.Equals(left.Value, right.Value, StringComparison.Ordinal)),
        value => value == null ? 0 : StringComparer.Ordinal.GetHashCode(value.Value),
        value => value == null ? null : Url.Create(value.Value));

    public static readonly ValueConverter<ViewPreferences, string?> ViewPreferencesConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(
                new ViewPreferencesSurrogate(
                    value.Filters,
                    value.DisplayFilters,
                    value.DisplayProperties,
                    value.RichFilters),
                JsonOptions),
        json => DeserializeViewPreferences(json));

    public static readonly ValueComparer<ViewPreferences> ViewPreferencesComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             string.Equals(left.Filters, right.Filters, StringComparison.Ordinal) &&
             string.Equals(left.DisplayFilters, right.DisplayFilters, StringComparison.Ordinal) &&
             string.Equals(left.DisplayProperties, right.DisplayProperties, StringComparison.Ordinal) &&
             string.Equals(left.RichFilters, right.RichFilters, StringComparison.Ordinal)),
        value => value == null
            ? 0
            : HashCode.Combine(value.Filters, value.DisplayFilters, value.DisplayProperties, value.RichFilters),
        value => value == null
            ? ViewPreferences.CreateIssueDefaults()
            : ViewPreferences.Create(value.Filters, value.DisplayFilters, value.DisplayProperties, value.RichFilters));


    public static readonly ValueConverter<ProjectMemberPreferences, string?> ProjectMemberPreferencesConverter = new(
        value => value == null
            ? null
            : JsonSerializer.Serialize(
                new ProjectMemberPreferencesSurrogate(
                    value.View.RawJson,
                    value.Defaults.RawJson,
                    value.Preferences.RawJson),
                JsonOptions),
        json => DeserializeProjectMemberPreferences(json));

    public static readonly ValueComparer<ProjectMemberPreferences> ProjectMemberPreferencesComparer = new(
        (left, right) =>
            (left == null && right == null) ||
            (left != null && right != null &&
             string.Equals(left.View.RawJson, right.View.RawJson, StringComparison.Ordinal) &&
             string.Equals(left.Defaults.RawJson, right.Defaults.RawJson, StringComparison.Ordinal) &&
             string.Equals(left.Preferences.RawJson, right.Preferences.RawJson, StringComparison.Ordinal)),
        value => value == null
            ? 0
            : HashCode.Combine(value.View.RawJson, value.Defaults.RawJson, value.Preferences.RawJson),
        value => value == null
            ? ProjectMemberPreferences.CreateDefault()
            : ProjectMemberPreferences.Create(
                StructuredData.FromJson(value.View.RawJson),
                StructuredData.FromJson(value.Defaults.RawJson),
                StructuredData.FromJson(value.Preferences.RawJson)));

    private static RichTextContent DeserializeRichText(string? value)
    {
        if (value is null)
        {
            return RichTextContent.Create();
        }

        var payload = value.Trim();
        if (payload.Length == 0)
        {
            return RichTextContent.Create();
        }

        var data = JsonSerializer.Deserialize<RichTextContentSurrogate>(payload, JsonOptions);
        if (data == null)
        {
            return RichTextContent.Create();
        }

        var binary = string.IsNullOrWhiteSpace(data.Binary) ? null : Convert.FromBase64String(data.Binary);
        return RichTextContent.Create(data.PlainText, data.Html, binary, data.Json);
    }

    private static DateRange DeserializeDateRange(string? value)
    {
        if (value is null)
        {
            return DateRange.Create(null, null);
        }

        var payload = value.Trim();
        if (payload.Length == 0)
        {
            return DateRange.Create(null, null);
        }

        var data = JsonSerializer.Deserialize<DateRangeSurrogate>(payload, JsonOptions);
        return data == null ? DateRange.Create(null, null) : DateRange.Create(data.Start, data.End);
    }

    private static ExternalReference? DeserializeExternalReference(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var payload = value.Trim();
        if (payload.Length == 0)
        {
            return null;
        }

        var data = JsonSerializer.Deserialize<ExternalReferenceSurrogate>(payload, JsonOptions);
        return data == null ? null : ExternalReference.Create(data.Source, data.Identifier);
    }

    private static ProjectMemberPreferences DeserializeProjectMemberPreferences(string? value)
    {
        if (value is null)
        {
            return ProjectMemberPreferences.CreateDefault();
        }

        var payload = value.Trim();
        if (payload.Length == 0)
        {
            return ProjectMemberPreferences.CreateDefault();
        }

        var data = JsonSerializer.Deserialize<ProjectMemberPreferencesSurrogate>(payload, JsonOptions);
        if (data == null)
        {
            return ProjectMemberPreferences.CreateDefault();
        }
        return ProjectMemberPreferences.Create(
            StructuredData.FromJson(data.View),
            StructuredData.FromJson(data.Defaults),
            StructuredData.FromJson(data.Preferences));
    }
    private static List<Url> DeserializeUrlList(string? value)
    {
        if (value is null)
        {
            return new List<Url>();
        }

        var payload = value.Trim();
        if (payload.Length == 0)
        {
            return new List<Url>();
        }

        var values = JsonSerializer.Deserialize<List<string>>(payload, JsonOptions);
        if (values == null)
        {
            return new List<Url>();
        }

        return values.Select(Url.Create).ToList();
    }

    private static Url? DeserializeUrl(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var payload = value.Trim();
        if (payload.Length == 0)
        {
            return null;
        }

        return Url.Create(payload);
    }

    private static ViewPreferences DeserializeViewPreferences(string? value)
    {
        if (value is null)
        {
            return ViewPreferences.CreateIssueDefaults();
        }

        var payload = value.Trim();
        if (payload.Length == 0)
        {
            return ViewPreferences.CreateIssueDefaults();
        }

        var data = JsonSerializer.Deserialize<ViewPreferencesSurrogate>(payload, JsonOptions);
        return data == null
            ? ViewPreferences.CreateIssueDefaults()
            : ViewPreferences.Create(data.Filters, data.DisplayFilters, data.DisplayProperties, data.RichFilters);
    }

    private static AuditTrail DeserializeAuditTrail(string? value)
    {
        if (value is null)
        {
            return AuditTrail.Empty;
        }

        var payload = value.Trim();
        if (payload.Length == 0)
        {
            return AuditTrail.Empty;
        }

        var data = JsonSerializer.Deserialize<AuditTrailSurrogate>(payload, JsonOptions);
        if (data == null)
        {
            return AuditTrail.Empty;
        }

        return AuditTrail.Create(data.CreatedAt, data.CreatedById, data.UpdatedAt, data.UpdatedById, data.DeletedAt);
    }

    private sealed record RichTextContentSurrogate(string? PlainText, string? Html, string? Json, string? Binary);
    private sealed record DateRangeSurrogate(DateTime? Start, DateTime? End);
    private sealed record ExternalReferenceSurrogate(string? Source, string? Identifier);
    private sealed record AuditTrailSurrogate(DateTime CreatedAt, Guid? CreatedById, DateTime? UpdatedAt, Guid? UpdatedById, DateTime? DeletedAt);
    private sealed record ProjectMemberPreferencesSurrogate(string? View, string? Defaults, string? Preferences);
    private sealed record WorkspaceMemberPreferencesSurrogate(string? View, string? Defaults, string? Issue);
    private sealed record ViewPreferencesSurrogate(string? Filters, string? DisplayFilters, string? DisplayProperties, string? RichFilters);
}
