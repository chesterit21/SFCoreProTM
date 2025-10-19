using System;
using SFCoreProTM.Domain.Entities;

namespace SFCoreProTM.Domain.Entities.Projects;

public enum ErdStatus
{
    IsReady,
    NotReady
}

public sealed class ErdDefinition : Entity
{
    private ErdDefinition()
    {
    }

    private ErdDefinition(
        Guid id,
        Guid moduleId,
        string tName,
        string description,
        string entityName,
        string attributeName,
        string attributeType,
        bool isPrimaryKey,
        bool isAcceptNull,
        string maxChar,
        int sortOrder,
        ErdStatus erdStatus)
    {
        Id = id;
        ModuleId = moduleId;
        TName = tName;
        Description = description;
        EntityName = entityName;
        AttributeName = attributeName;
        AttributeType = attributeType;
        IsPrimaryKey = isPrimaryKey;
        IsAcceptNull = isAcceptNull;
        MaxChar = maxChar;
        SortOrder = sortOrder;
        ErdStatus = erdStatus;
    }

    public Guid ModuleId { get; private set; }
    public string TName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public string AttributeName { get; private set; } = string.Empty;
    public string AttributeType { get; private set; } = string.Empty;
    public bool IsPrimaryKey { get; private set; }
    public bool IsAcceptNull { get; private set; }
    public string MaxChar { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public ErdStatus ErdStatus { get; private set; }

    public static ErdDefinition Create(
        Guid id,
        Guid moduleId,
        string tName,
        string description,
        string entityName,
        string attributeName,
        string attributeType,
        bool isPrimaryKey,
        bool isAcceptNull,
        string maxChar,
        int sortOrder,
        ErdStatus erdStatus)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tName);
        ArgumentException.ThrowIfNullOrWhiteSpace(entityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(attributeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(attributeType);

        return new ErdDefinition(
            id,
            moduleId,
            tName,
            description ?? string.Empty,
            entityName,
            attributeName,
            attributeType,
            isPrimaryKey,
            isAcceptNull,
            maxChar ?? string.Empty,
            sortOrder,
            erdStatus);
    }

    public void UpdateDetails(
        string tName,
        string description,
        string entityName,
        string attributeName,
        string attributeType,
        bool isPrimaryKey,
        bool isAcceptNull,
        string maxChar,
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

        if (!string.IsNullOrWhiteSpace(attributeName))
        {
            AttributeName = attributeName;
        }

        if (!string.IsNullOrWhiteSpace(attributeType))
        {
            AttributeType = attributeType;
        }

        IsPrimaryKey = isPrimaryKey;
        IsAcceptNull = isAcceptNull;
        MaxChar = maxChar ?? string.Empty;
        SortOrder = sortOrder;
        ErdStatus = erdStatus;
    }
}