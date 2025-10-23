using System;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.DTOs.Projects;

public sealed class UpdateProjectRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ProjectPath { get; set; } = string.Empty;

    public ProjectStatus Status { get; set; }
}