using System;
using System.Collections.Generic;

namespace SFCoreProTM.Application.DTOs.ErdDefinitions;

public class CreateErdDefinitionRequestDto
{
    public Guid ModuleId { get; set; }
    public string TName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 1;
    public IEnumerable<AttributeEntitasRequestDto> Attributes { get; set; } = Array.Empty<AttributeEntitasRequestDto>();
}
