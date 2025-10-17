using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Projects;

public sealed class Importer : ProjectScopedEntity
{
    private Importer()
    {
    }

    private Importer(Guid id, Guid workspaceId, Guid projectId, string service, string status, Guid initiatedById)
        : base(id, workspaceId, projectId)
    {
        Service = service;
        Status = status;
        InitiatedById = initiatedById;
    }

    public string Service { get; private set; } = string.Empty;

    public string Status { get; private set; } = string.Empty;

    public Guid InitiatedById { get; private set; }

    public StructuredData Metadata { get; private set; } = StructuredData.FromJson(null);

    public StructuredData Config { get; private set; } = StructuredData.FromJson(null);

    public StructuredData Data { get; private set; } = StructuredData.FromJson(null);

    public string? Token { get; private set; }

    public StructuredData ImportedData { get; private set; } = StructuredData.FromJson(null);

    public static Importer Create(Guid id, Guid workspaceId, Guid projectId, string service, string status, Guid initiatedById)
    {
        return new Importer(id, workspaceId, projectId, service, status, initiatedById);
    }

    public void UpdateState(string status, StructuredData metadata, StructuredData config)
    {
        Status = status;
        Metadata = metadata;
        Config = config;
    }

    public void UpdateProgress(StructuredData data, StructuredData importedData)
    {
        Data = data;
        ImportedData = importedData;
    }

    public void SetToken(string? token)
    {
        Token = token;
    }
}
