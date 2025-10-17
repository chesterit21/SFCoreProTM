using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Analytics;

public sealed class AnalyticView : WorkspaceScopedEntity
{
    private AnalyticView()
    {
    }

    private AnalyticView(Guid id, Guid workspaceId, string name, StructuredData query, StructuredData queryDictionary)
        : base(id, workspaceId)
    {
        Name = name;
        Query = query;
        QueryDictionary = queryDictionary;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public StructuredData Query { get; private set; } = StructuredData.FromJson(null);

    public StructuredData QueryDictionary { get; private set; } = StructuredData.FromJson(null);

    public static AnalyticView Create(Guid id, Guid workspaceId, string name, StructuredData query, StructuredData queryDictionary)
    {
        return new AnalyticView(id, workspaceId, name, query, queryDictionary);
    }

    public void UpdateDefinition(string? description, StructuredData query, StructuredData queryDictionary)
    {
        Description = description;
        Query = query;
        QueryDictionary = queryDictionary;
    }
}
