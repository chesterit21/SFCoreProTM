using System;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.DTOs.Projects;

public sealed record ProjectDto
{
    public Guid Id { get; init; }

    public Guid WorkspaceId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public ProjectVisibility Visibility { get; init; }

    public string Timezone { get; init; } = "UTC";

    public string? Emoji { get; init; }

    public Guid? ProjectLeadId { get; init; }

    public Guid? DefaultAssigneeId { get; init; }

    public bool ModuleViewEnabled { get; init; }

    public bool CycleViewEnabled { get; init; }

    public bool IssueViewsEnabled { get; init; }

    public bool PageViewEnabled { get; init; }

    public bool IntakeViewEnabled { get; init; }

    public bool TimeTrackingEnabled { get; init; }

    public bool IssueTypeEnabled { get; init; }

    public bool GuestViewAllFeatures { get; init; }

    public int ArchiveInMonths { get; init; }

    public int CloseInMonths { get; init; }

    public Guid? DefaultStateId { get; init; }
}
