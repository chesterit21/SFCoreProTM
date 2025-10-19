using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Projects;

namespace SFCoreProTM.Application.Features.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(Guid ProjectId) : IRequest<ProjectDto>;