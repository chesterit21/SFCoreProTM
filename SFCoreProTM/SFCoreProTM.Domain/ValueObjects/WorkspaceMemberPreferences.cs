
using System.Collections.Generic;
using System.Text.Json;

namespace SFCoreProTM.Domain.ValueObjects;

public sealed class WorkspaceMemberPreferences : ValueObject
{
    private WorkspaceMemberPreferences()
        : this(DefaultViewProps, DefaultViewProps, DefaultIssueProps)
    {
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = false,
    };

    private static readonly StructuredData DefaultViewProps = StructuredData.FromJson(BuildPropsJson());
    private static readonly StructuredData DefaultIssueProps = StructuredData.FromJson(BuildIssueJson());

    private WorkspaceMemberPreferences(StructuredData view, StructuredData defaults, StructuredData issue)
    {
        View = view;
        Defaults = defaults;
        Issue = issue;
    }

    public StructuredData View { get; }

    public StructuredData Defaults { get; }

    public StructuredData Issue { get; }

    public static WorkspaceMemberPreferences Create(StructuredData view, StructuredData defaults, StructuredData issue)
        => new(view, defaults, issue);

    public static WorkspaceMemberPreferences CreateDefault()
        => new(DefaultViewProps, DefaultViewProps, DefaultIssueProps);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return View;
        yield return Defaults;
        yield return Issue;
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

    private static string BuildIssueJson()
    {
        var issue = new Dictionary<string, object?>
        {
            ["subscribed"] = true,
            ["assigned"] = true,
            ["created"] = true,
            ["all_issues"] = true,
        };

        return JsonSerializer.Serialize(issue, SerializerOptions);
    }
}
