
using System.Collections.Generic;
using System.Text.Json;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class ProjectMemberPreferences : ValueObject
{
    private ProjectMemberPreferences()
        : this(DefaultViewProps, DefaultViewProps, DefaultPreferences)
    {
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = false,
    };

    private static readonly StructuredData DefaultViewProps = StructuredData.FromJson(BuildPropsJson());
    private static readonly StructuredData DefaultPreferences = StructuredData.FromJson(BuildPreferencesJson());

    private ProjectMemberPreferences(StructuredData view, StructuredData defaults, StructuredData preferences)
    {
        View = view;
        Defaults = defaults;
        Preferences = preferences;
    }

    public StructuredData View { get; }

    public StructuredData Defaults { get; }

    public StructuredData Preferences { get; }

    public static ProjectMemberPreferences Create(StructuredData view, StructuredData defaults, StructuredData preferences)
        => new(view, defaults, preferences);

    public static ProjectMemberPreferences CreateDefault()
        => new(DefaultViewProps, DefaultViewProps, DefaultPreferences);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return View;
        yield return Defaults;
        yield return Preferences;
    }

    private static string BuildPropsJson()
    {
        var filters = new Dictionary<string, object?>
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
        };

        var displayFilters = new Dictionary<string, object?>
        {
            ["group_by"] = null,
            ["order_by"] = "-created_at",
            ["type"] = null,
            ["sub_issue"] = true,
            ["show_empty_groups"] = true,
            ["layout"] = "list",
            ["calendar_date_range"] = string.Empty,
        };

        var displayProperties = new Dictionary<string, object?>
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
        };

        var payload = new Dictionary<string, object?>
        {
            ["filters"] = filters,
            ["display_filters"] = displayFilters,
            ["display_properties"] = displayProperties,
        };

        return JsonSerializer.Serialize(payload, SerializerOptions);
    }

    private static string BuildPreferencesJson()
    {
        var payload = new Dictionary<string, object?>
        {
            ["pages"] = new Dictionary<string, object?>
            {
                ["block_display"] = true,
            },
        };

        return JsonSerializer.Serialize(payload, SerializerOptions);
    }
}
