using System;
using System.ComponentModel.DataAnnotations;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Mapping.Requests.Projects;

public sealed class CreateProjectRequest
{
    [Required]
    public Guid ActorId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(12)]
    public string Identifier { get; set; } = string.Empty;

    public string? DescriptionPlainText { get; set; }

    public string? DescriptionHtml { get; set; }

    public string? DescriptionJson { get; set; }

    public Guid? ProjectLeadId { get; set; }

    public Guid? DefaultAssigneeId { get; set; }

    public bool ModuleViewEnabled { get; set; }

    public bool CycleViewEnabled { get; set; }

    public bool IssueViewsEnabled { get; set; }

    public bool PageViewEnabled { get; set; } = true;

    public bool IntakeViewEnabled { get; set; }

    public bool TimeTrackingEnabled { get; set; }

    public bool IssueTypeEnabled { get; set; }

    public bool GuestViewAllFeatures { get; set; }

    [Range(0, 12)]
    public int ArchiveInMonths { get; set; }

    [Range(0, 12)]
    public int CloseInMonths { get; set; }

    [MaxLength(255)]
    public string? Timezone { get; set; }

    public string? Emoji { get; set; }

    public string? IconPropertiesJson { get; set; }

    public string? LogoPropertiesJson { get; set; }

    [Url]
    public string? CoverImageUrl { get; set; }

    public string? ExternalSource { get; set; }

    public string? ExternalId { get; set; }

    public ProjectVisibility Visibility { get; set; } = ProjectVisibility.Public;
}

