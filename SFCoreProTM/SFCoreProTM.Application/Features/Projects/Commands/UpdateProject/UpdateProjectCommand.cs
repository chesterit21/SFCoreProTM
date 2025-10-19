using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Projects;

namespace SFCoreProTM.Application.Features.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(Guid ProjectId, UpdateProjectRequestDto Payload) : IRequest<ProjectDto>;