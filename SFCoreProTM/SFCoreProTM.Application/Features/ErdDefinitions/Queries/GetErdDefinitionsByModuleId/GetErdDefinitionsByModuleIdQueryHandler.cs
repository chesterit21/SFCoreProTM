using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.ErdDefinitions;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.ErdDefinitions.Queries.GetErdDefinitionsByModuleId;

public class GetErdDefinitionsByModuleIdQueryHandler : IRequestHandler<GetErdDefinitionsByModuleIdQuery, IEnumerable<ErdDefinitionDto>>
{
    private readonly IErdDefinitionRepository _erdDefinitionRepository;

    public GetErdDefinitionsByModuleIdQueryHandler(IErdDefinitionRepository erdDefinitionRepository)
    {
        _erdDefinitionRepository = erdDefinitionRepository;
    }

    public async Task<IEnumerable<ErdDefinitionDto>> Handle(GetErdDefinitionsByModuleIdQuery request, CancellationToken cancellationToken)
    {
        var erdDefinitions = await _erdDefinitionRepository.GetByModuleIdAsync(request.ModuleId, cancellationToken);
        
        return erdDefinitions.Select(erdDefinition => new ErdDefinitionDto
        {
            Id = erdDefinition.Id,
            ModuleId = erdDefinition.ModuleId,
            TName = erdDefinition.TName,
            Description = erdDefinition.Description,
            EntityName = erdDefinition.EntityName,
            AttributeName = erdDefinition.AttributeName,
            AttributeType = erdDefinition.AttributeType,
            IsPrimaryKey = erdDefinition.IsPrimaryKey,
            IsAcceptNull = erdDefinition.IsAcceptNull,
            MaxChar = erdDefinition.MaxChar,
            SortOrder = erdDefinition.SortOrder,
            ErdStatus = (int)erdDefinition.ErdStatus
        }).ToList();
    }
}