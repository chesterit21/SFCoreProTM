using System.ComponentModel.DataAnnotations;

namespace SFCoreProTM.Application.Mapping.Requests.Projects;

public sealed class UpdateProjectRequest
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? DescriptionPlainText { get; set; }
}