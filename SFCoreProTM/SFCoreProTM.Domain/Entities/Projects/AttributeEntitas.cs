using System;
using SFCoreProTM.Domain.Entities;

namespace SFCoreProTM.Domain.Entities.Projects;

public sealed class AttributeEntitas : Entity
{
    private AttributeEntitas()
    {
    }

    private AttributeEntitas(
        Guid id,
        Guid erdDefinitionId,
        string? name,
        string? dataType,
        string? description,
        int? maxChar,
        int? sortOrder,
        bool? isPrimary,
        bool? isNull,
        bool? isForeignKey,
        string? foreignKeyTable)
    {
        Id = id;
        ErdDefinitionId = erdDefinitionId;
        Name = name;
        DataType = dataType;
        Description = description;
        MaxChar = maxChar;
        SortOrder = sortOrder;
        IsPrimary = isPrimary;
        IsNull = isNull;
        IsForeignKey = isForeignKey;
        ForeignKeyTable = foreignKeyTable;
    }

    public Guid ErdDefinitionId { get; private set; }
    public string? Name { get; private set; }
    public string? DataType { get; private set; }
    public string? Description { get; private set; }
    public int? MaxChar { get; private set; }
    public int? SortOrder { get; private set; }
    public bool? IsPrimary { get; private set; }
    public bool? IsNull { get; private set; }
    public bool? IsForeignKey { get; private set; }
    public string? ForeignKeyTable { get; private set; }
    public ErdDefinition? ErdDefinition { get; private set; }

    public static AttributeEntitas Create(
        Guid id,
        Guid erdDefinitionId,
        string? name,
        string? dataType,
        string? description,
        int? maxChar,
        int? sortOrder,
        bool? isPrimary,
        bool? isNull,
        bool? isForeignKey,
        string? foreignKeyTable)
    {
        return new AttributeEntitas(
            id,
            erdDefinitionId,
            name,
            dataType,
            description,
            maxChar,
            sortOrder,
            isPrimary,
            isNull,
            isForeignKey,
            foreignKeyTable);
    }

    public void UpdateDetails(
        string? name,
        string? dataType,
        string? description,
        int? maxChar,
        int? sortOrder,
        bool? isPrimary,
        bool? isNull,
        bool? isForeignKey,
        string? foreignKeyTable)
    {
        Name = name;
        DataType = dataType;
        Description = description;
        MaxChar = maxChar;
        SortOrder = sortOrder;
        IsPrimary = isPrimary;
        IsNull = isNull;
        IsForeignKey = isForeignKey;
        ForeignKeyTable = foreignKeyTable;
    }

    internal void AssignToErdDefinition(Guid erdDefinitionId)
    {
        ErdDefinitionId = erdDefinitionId;
    }
}
