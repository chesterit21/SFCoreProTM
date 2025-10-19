using System;

namespace SFCoreProTM.Application.DTOs.Projects;

public sealed class UpdateProjectRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}