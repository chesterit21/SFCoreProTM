using System;
using System.Collections.Generic;
using System.Linq;
using SFCoreProTM.Domain.Entities;

namespace SFCoreProTM.Domain.Entities.Projects;

public enum ErdStatus
{
    IsReady,
    NotReady
}

public sealed class ErdDefinition : Entity
{
    private readonly List<AttributeEntitas> _attributes = new();

    private ErdDefinition()
    {
    }

    private ErdDefinition(
        Guid id,
        Guid moduleId,
        string tName,
        string description,
        string entityName,
        int sortOrder,
        ErdStatus erdStatus)
    {
        Id = id;
        ModuleId = moduleId;
        TName = tName;
        Description = description;
        EntityName = entityName;
        SortOrder = sortOrder;
        ErdStatus = erdStatus;
    }

    public Guid ModuleId { get; private set; }
    public string TName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public ErdStatus ErdStatus { get; private set; }
    public IReadOnlyCollection<AttributeEntitas> Attributes => _attributes.AsReadOnly();

    public static ErdDefinition Create(
        Guid id,
        Guid moduleId,
        string tName,
        string description,
        string entityName,
        int sortOrder,
        ErdStatus erdStatus)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tName);

        return new ErdDefinition(
            id,
            moduleId,
            tName,
            description ?? string.Empty,
            entityName ?? string.Empty,
            sortOrder,
            erdStatus);
    }

    public void UpdateDetails(
        string tName,
        string description,
        string entityName,
        int sortOrder,
        ErdStatus erdStatus)
    {
        if (!string.IsNullOrWhiteSpace(tName))
        {
            TName = tName;
        }

        Description = description ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            EntityName = entityName;
        }

        SortOrder = sortOrder;
        ErdStatus = erdStatus;
    }

    public void SetAttributes(IEnumerable<AttributeEntitas> attributes)
    {
        var incoming = attributes?.ToDictionary(attribute => attribute.Id) ?? new Dictionary<Guid, AttributeEntitas>();

        for (var index = _attributes.Count - 1; index >= 0; index--)
        {
            var existingAttribute = _attributes[index];
            if (!incoming.TryGetValue(existingAttribute.Id, out var updatedAttribute))
            {
                _attributes.RemoveAt(index);
                continue;
            }

        existingAttribute.UpdateDetails(
            updatedAttribute.Name,
            updatedAttribute.DataType,
            updatedAttribute.Description,
            updatedAttribute.MaxChar,
            updatedAttribute.SortOrder,
            updatedAttribute.IsPrimary,
            updatedAttribute.IsNull,
            updatedAttribute.IsForeignKey,
            updatedAttribute.ForeignKeyTable);

        existingAttribute.AssignToErdDefinition(Id);

        incoming.Remove(existingAttribute.Id);
    }

        foreach (var remainingAttribute in incoming.Values)
        {
            remainingAttribute.AssignToErdDefinition(Id);
            _attributes.Add(remainingAttribute);
        }
    }
}
