using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.ErdDefinitions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.ErdDefinitions.Commands.UpdateErdDefinition;

public class UpdateErdDefinitionCommandHandler : IRequestHandler<UpdateErdDefinitionCommand, ErdDefinitionDto>
{
    private readonly IErdDefinitionRepository _erdDefinitionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateErdDefinitionCommandHandler(IErdDefinitionRepository erdDefinitionRepository, IUnitOfWork unitOfWork)
    {
        _erdDefinitionRepository = erdDefinitionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErdDefinitionDto> Handle(UpdateErdDefinitionCommand request, CancellationToken cancellationToken)
    {
        var erdDefinition = await _erdDefinitionRepository.GetByIdAsync(request.ErdDefinitionId, cancellationToken);
        
        if (erdDefinition == null)
        {
            throw new Exception($"ErdDefinition with ID {request.ErdDefinitionId} not found.");
        }

        erdDefinition.UpdateDetails(
            request.Request.TName,
            request.Request.Description,
            request.Request.EntityName,
            request.Request.SortOrder,
            erdDefinition.ErdStatus
        );

        var attributes = (request.Request.Attributes ?? Array.Empty<AttributeEntitasRequestDto>())
            .Select(attribute => AttributeEntitas.Create(
                attribute.Id ?? Guid.NewGuid(),
                erdDefinition.Id,
                attribute.Name,
                attribute.DataType,
                attribute.Description,
                attribute.MaxChar,
                attribute.SortOrder,
                attribute.IsPrimary,
                attribute.IsNull,
                attribute.IsForeignKey,
                attribute.ForeignKeyTable
            ));

        erdDefinition.SetAttributes(attributes);

        await _erdDefinitionRepository.UpdateAsync(erdDefinition, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ErdDefinitionDto
        {
            Id = erdDefinition.Id,
            ModuleId = erdDefinition.ModuleId,
            TName = erdDefinition.TName,
            Description = erdDefinition.Description,
            EntityName = erdDefinition.EntityName,
            SortOrder = erdDefinition.SortOrder,
            ErdStatus = (int)erdDefinition.ErdStatus,
            Attributes = erdDefinition.Attributes
                .OrderBy(a => a.SortOrder ?? int.MaxValue)
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
        };
    }
}
