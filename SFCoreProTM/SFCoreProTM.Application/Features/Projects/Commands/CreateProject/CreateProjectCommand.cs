using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Projects;

namespace SFCoreProTM.Application.Features.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(Guid WorkspaceId, Guid ActorId, CreateProjectRequestDto Payload) : IRequest<ProjectDto>;
