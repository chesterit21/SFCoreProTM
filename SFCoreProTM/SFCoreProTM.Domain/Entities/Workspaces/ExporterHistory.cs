using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Workspaces;

public sealed class ExporterHistory : WorkspaceScopedEntity
{
    private ExporterHistory()
    {
    }

    private ExporterHistory(Guid id, Guid workspaceId, string name, string exportType, string provider, string status, Guid initiatedById)
        : base(id, workspaceId)
    {
        Name = name;
        Type = exportType;
        Provider = provider;
        Status = status;
        InitiatedById = initiatedById;
    }

    public string Name { get; private set; } = string.Empty;

    public string Type { get; private set; } = string.Empty;

    public string Provider { get; private set; } = string.Empty;

    public string Status { get; private set; } = string.Empty;

    public string? Reason { get; private set; }

    public string? Key { get; private set; }

    public Url? Url { get; private set; }

    public string? Token { get; private set; }

    public Guid InitiatedById { get; private set; }

    public StructuredData Filters { get; private set; } = StructuredData.FromJson(null);

    public StructuredData RichFilters { get; private set; } = StructuredData.FromJson(null);

    public static ExporterHistory Create(Guid id, Guid workspaceId, string name, string exportType, string provider, string status, Guid initiatedById)
    {
        return new ExporterHistory(id, workspaceId, name, exportType, provider, status, initiatedById);
    }

    public void AttachFilters(StructuredData filters, StructuredData richFilters)
    {
        Filters = filters;
        RichFilters = richFilters;
    }

    public void Complete(string status, string? reason, string? token, Url? url)
    {
        Status = status;
        Reason = reason;
        Token = token;
        Url = url;
    }
}
