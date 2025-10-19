using System;
using System.Collections.Generic;
using MediatR;
using SFCoreProTM.Application.DTOs.Projects;

namespace SFCoreProTM.Application.Features.Projects.Queries.GetProjectsByWorkspace;

public sealed record GetProjectsByWorkspaceQuery(Guid WorkspaceId) : IRequest<IReadOnlyCollection<ProjectSummaryDto>>;
