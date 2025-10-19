using System;
using System.ComponentModel.DataAnnotations;

namespace SFCoreProTM.Application.Mapping.Requests.Projects;

public sealed class CreateProjectRequest
{
    [Required]
    public Guid ActorId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? DescriptionPlainText { get; set; }
}
