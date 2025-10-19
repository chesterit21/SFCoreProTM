using System;
using System.Collections.Generic;
using MediatR;
using SFCoreProTM.Application.DTOs.Modules;

namespace SFCoreProTM.Application.Features.Modules.Queries.GetModulesByProjectId;

public class GetModulesByProjectIdQuery : IRequest<IEnumerable<ModuleDto>>
{
    public Guid ProjectId { get; set; }
}