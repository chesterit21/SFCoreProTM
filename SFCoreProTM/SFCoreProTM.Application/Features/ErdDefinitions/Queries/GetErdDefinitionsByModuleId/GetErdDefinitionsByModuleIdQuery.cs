using System;
using System.Collections.Generic;
using MediatR;
using SFCoreProTM.Application.DTOs.ErdDefinitions;

namespace SFCoreProTM.Application.Features.ErdDefinitions.Queries.GetErdDefinitionsByModuleId;

public class GetErdDefinitionsByModuleIdQuery : IRequest<IEnumerable<ErdDefinitionDto>>
{
    public Guid ModuleId { get; set; }
}