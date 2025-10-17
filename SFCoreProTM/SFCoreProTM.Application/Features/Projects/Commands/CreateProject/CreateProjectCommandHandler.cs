using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using AutoMapper;
using SFCoreProTM.Application.DTOs.Projects;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private static readonly IReadOnlyList<(string Name, string HexColor, int Sequence, string Group, bool IsDefault)> DefaultStates = new List<(string, string, int, string, bool)>
    {
        ("Backlog", "#60646C", 15000, "backlog", true),
        ("Todo", "#60646C", 25000, "unstarted", false),
        ("In Progress", "#F59E0B", 35000, "started", false),
        ("Done", "#46A758", 45000, "completed", false),
        ("Cancelled", "#9AA4BC", 55000, "cancelled", false),
    };

    private readonly IWorkspaceReadService _workspaceReadService;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CreateProjectCommandHandler(
        IWorkspaceReadService workspaceReadService,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _workspaceReadService = workspaceReadService;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;

        if (!await _workspaceReadService.WorkspaceExistsAsync(request.WorkspaceId, cancellationToken))
        {
            throw new NotFoundException($"Workspace '{request.WorkspaceId}' was not found.");
        }

        if (!await _workspaceReadService.IsMemberAsync(request.WorkspaceId, request.ActorId, cancellationToken))
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("ActorId", "Actor must be a member of the workspace.") });
        }

        var identifier = payload.Identifier.Trim().ToUpperInvariant();
        var projectName = payload.Name.Trim();

        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("Identifier", "Project identifier is required.") });
        }

        if (await _workspaceReadService.ProjectIdentifierExistsAsync(request.WorkspaceId, identifier, cancellationToken))
        {
            throw new ConflictException($"Project identifier '{identifier}' is already in use.");
        }

        if (await _workspaceReadService.ProjectNameExistsAsync(request.WorkspaceId, projectName, cancellationToken))
        {
            throw new ConflictException($"Project name '{projectName}' is already in use.");
        }

        if (payload.ProjectLeadId.HasValue &&
            !await _workspaceReadService.IsMemberAsync(request.WorkspaceId, payload.ProjectLeadId.Value, cancellationToken))
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("ProjectLeadId", "Project lead must be a member of the workspace.") });
        }

        if (payload.DefaultAssigneeId.HasValue &&
            !await _workspaceReadService.IsMemberAsync(request.WorkspaceId, payload.DefaultAssigneeId.Value, cancellationToken))
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("DefaultAssigneeId", "Default assignee must be a member of the workspace.") });
        }

        var projectId = Guid.NewGuid();
        var visibility = payload.Visibility;
        var timezone = string.IsNullOrWhiteSpace(payload.Timezone) ? "UTC" : payload.Timezone;

        var project = Project.Create(projectId, request.WorkspaceId, projectName, identifier, visibility, timezone);

        var description = RichTextContent.Create(payload.DescriptionPlainText, payload.DescriptionHtml, null, payload.DescriptionJson);
        project.UpdateDetails(description, StructuredData.FromJson(null), StructuredData.FromJson(null));

        project.ConfigureViews(
            payload.ModuleViewEnabled,
            payload.CycleViewEnabled,
            payload.IssueViewsEnabled,
            payload.PageViewEnabled,
            payload.IntakeViewEnabled);

        project.ConfigureFeatures(payload.TimeTrackingEnabled, payload.IssueTypeEnabled, payload.GuestViewAllFeatures);
        project.ConfigureCadence(payload.ArchiveInMonths, payload.CloseInMonths);
        project.AssignLeadership(payload.ProjectLeadId, payload.DefaultAssigneeId);
        project.SetExternalReference(payload.ExternalSource, payload.ExternalId);

        var iconProps = StructuredData.FromJson(payload.IconPropertiesJson);
        var logoProps = StructuredData.FromJson(payload.LogoPropertiesJson);
        Url? coverImage = null;
        if (!string.IsNullOrWhiteSpace(payload.CoverImageUrl))
        {
            coverImage = Url.Create(payload.CoverImageUrl);
        }

        project.UpdateBranding(payload.Emoji, iconProps, logoProps, coverImage, null);

        var now = _dateTimeProvider.UtcNow;
        project.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));

        var states = new List<State>();
        Guid? defaultStateId = null;
        foreach (var (name, hexColor, sequence, group, isDefault) in DefaultStates)
        {
            var stateId = Guid.NewGuid();
            var state = State.Create(
                stateId,
                request.WorkspaceId,
                projectId,
                name,
                ColorCode.FromHex(hexColor),
                Slug.Create(name.ToLowerInvariant().Replace(' ', '-')),
                sequence);
            state.UpdateDetails(null, group, isTriage: false, isDefault: isDefault, sequence: sequence);
            state.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));
            states.Add(state);

            if (isDefault)
            {
                defaultStateId = stateId;
            }
        }

        project.SetDefaultState(defaultStateId);

        var projectMembers = new List<ProjectMember>();
        var issueUserProperties = new List<IssueUserProperty>();

        var adminPreferences = ProjectMemberPreferences.CreateDefault();

        var actorMember = project.AddMember(Guid.NewGuid(), request.ActorId, ProjectRole.Admin, adminPreferences, sortOrder: 0, isActive: true);
        actorMember.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));
        projectMembers.Add(actorMember);

        var actorPreferences = IssueUserProperty.Create(Guid.NewGuid(), request.WorkspaceId, projectId, request.ActorId, ViewPreferences.CreateIssueDefaults());
        actorPreferences.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));
        issueUserProperties.Add(actorPreferences);

        if (payload.ProjectLeadId.HasValue && payload.ProjectLeadId.Value != request.ActorId)
        {
            var leaderMember = project.AddMember(Guid.NewGuid(), payload.ProjectLeadId.Value, ProjectRole.Admin, adminPreferences, sortOrder: 1, isActive: true);
            leaderMember.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));
            projectMembers.Add(leaderMember);

            var leaderPreferences = IssueUserProperty.Create(Guid.NewGuid(), request.WorkspaceId, projectId, payload.ProjectLeadId.Value, ViewPreferences.CreateIssueDefaults());
            leaderPreferences.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));
            issueUserProperties.Add(leaderPreferences);
        }

        var identifierEntity = ProjectIdentifier.Create(Guid.NewGuid(), request.WorkspaceId, projectId, identifier);
        identifierEntity.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _projectRepository.AddAsync(project, states, projectMembers, issueUserProperties, identifierEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return _mapper.Map<ProjectDto>(project);
    }
}
