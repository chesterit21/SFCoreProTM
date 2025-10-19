using System;

namespace SFCoreProTM.Application.DTOs.ErdDefinitions;

public class UpdateErdDefinitionRequestDto
{
    public Guid ModuleId { get; set; }
    public string TName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeType { get; set; } = string.Empty;
    public bool IsPrimaryKey { get; set; }
    public bool IsAcceptNull { get; set; }
    public string MaxChar { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 1;
}