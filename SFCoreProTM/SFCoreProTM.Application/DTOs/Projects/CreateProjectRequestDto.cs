using System;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.DTOs.Projects;

public sealed class CreateProjectRequestDto
{
    public string Name { get; set; } = string.Empty;

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

    public int ArchiveInMonths { get; set; }

    public int CloseInMonths { get; set; }

    public string Timezone { get; set; } = "UTC";

    public string? Emoji { get; set; }

    public string? IconPropertiesJson { get; set; }

    public string? LogoPropertiesJson { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? ExternalSource { get; set; }

    public string? ExternalId { get; set; }

    public ProjectVisibility Visibility { get; set; } = ProjectVisibility.Public;
}
