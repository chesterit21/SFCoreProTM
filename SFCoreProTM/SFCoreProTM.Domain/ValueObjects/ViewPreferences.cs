
using System.Collections.Generic;
using System.Text.Json;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class ViewPreferences : ValueObject
{
    private ViewPreferences()
        : this(EmptyObjectJson, EmptyObjectJson, EmptyObjectJson, EmptyObjectJson)
    {
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = false,
    };

    private static readonly string EmptyObjectJson = Serialize(new Dictionary<string, object?>());

    private static readonly string IssueFiltersJson = Serialize(new Dictionary<string, object?>
    {
        ["priority"] = null,
        ["state"] = null,
        ["state_group"] = null,
        ["assignees"] = null,
        ["created_by"] = null,
        ["labels"] = null,
        ["start_date"] = null,
        ["target_date"] = null,
        ["subscriber"] = null,
    });

    private static readonly string IssueDisplayFiltersJson = Serialize(new Dictionary<string, object?>
    {
        ["group_by"] = null,
        ["order_by"] = "-created_at",
        ["type"] = null,
        ["sub_issue"] = true,
        ["show_empty_groups"] = true,
        ["layout"] = "list",
        ["calendar_date_range"] = string.Empty,
    });

    private static readonly string IssueDisplayPropertiesJson = Serialize(new Dictionary<string, object?>
    {
        ["assignee"] = true,
        ["attachment_count"] = true,
        ["created_on"] = true,
        ["due_date"] = true,
        ["estimate"] = true,
        ["key"] = true,
        ["labels"] = true,
        ["link"] = true,
        ["priority"] = true,
        ["start_date"] = true,
        ["state"] = true,
        ["sub_issue_count"] = true,
        ["updated_on"] = true,
    });

    private ViewPreferences(string? filters, string? displayFilters, string? displayProperties, string? richFilters)
    {
        Filters = filters;
        DisplayFilters = displayFilters;
        DisplayProperties = displayProperties;
        RichFilters = richFilters;
    }

    public string? Filters { get; }

    public string? DisplayFilters { get; }

    public string? DisplayProperties { get; }

    public string? RichFilters { get; }

    public static ViewPreferences Create(string? filters, string? displayFilters, string? displayProperties, string? richFilters)
    {
        return new ViewPreferences(filters, displayFilters, displayProperties, richFilters);
    }

    public static ViewPreferences CreateIssueDefaults()
    {
        return new ViewPreferences(IssueFiltersJson, IssueDisplayFiltersJson, IssueDisplayPropertiesJson, EmptyObjectJson);
    }

    public static ViewPreferences CreateModuleDefaults()
    {
        return CreateIssueDefaults();
    }

    public static ViewPreferences CreateCycleDefaults()
    {
        return CreateIssueDefaults();
    }

    public static ViewPreferences CreateEmpty()
    {
        return new ViewPreferences(EmptyObjectJson, EmptyObjectJson, EmptyObjectJson, EmptyObjectJson);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Filters;
        yield return DisplayFilters;
        yield return DisplayProperties;
        yield return RichFilters;
    }

    private static string Serialize(object value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }
}
