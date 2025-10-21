using System;

namespace SFCoreProTM.Application.DTOs.ErdDefinitions;

public class AttributeEntitasRequestDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? DataType { get; set; }
    public string? Description { get; set; }
    public int? MaxChar { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsPrimary { get; set; }
    public bool? IsNull { get; set; }
    public bool? IsForeignKey { get; set; }
    public string? ForeignKeyTable { get; set; }
}
