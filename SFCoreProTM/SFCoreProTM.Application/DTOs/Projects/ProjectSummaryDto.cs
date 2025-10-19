using System;

namespace SFCoreProTM.Application.DTOs.Projects;

public sealed class ProjectSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}