using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.ErdDefinitions;
using SFCoreProTM.Application.Interfaces.Repositories;

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
            SortOrder = erdDefinition.SortOrder,
            ErdStatus = (int)erdDefinition.ErdStatus,
            Attributes = erdDefinition.Attributes
                .OrderBy(attribute => attribute.SortOrder ?? int.MaxValue)
                .Select(attribute => new AttributeEntitasDto
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    DataType = attribute.DataType,
                    Description = attribute.Description,
                    MaxChar = attribute.MaxChar,
                    SortOrder = attribute.SortOrder,
                    IsPrimary = attribute.IsPrimary,
                    IsNull = attribute.IsNull,
                    IsForeignKey = attribute.IsForeignKey,
                    ForeignKeyTable = attribute.ForeignKeyTable
                })
        }).ToList();
    }
}
