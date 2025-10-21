using System;
using System.Collections.Generic;

namespace SFCoreProTM.Application.DTOs.ErdDefinitions;

public class ErdDefinitionDto
{
    public Guid Id { get; set; }
    public Guid ModuleId { get; set; }
    public string TName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int ErdStatus { get; set; } // ErdStatus enum value
    public IEnumerable<AttributeEntitasDto> Attributes { get; set; } = Array.Empty<AttributeEntitasDto>();
}
