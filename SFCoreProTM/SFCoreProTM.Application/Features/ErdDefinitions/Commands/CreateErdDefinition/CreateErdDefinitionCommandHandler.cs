using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.ErdDefinitions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.ErdDefinitions.Commands.CreateErdDefinition;

public class CreateErdDefinitionCommandHandler : IRequestHandler<CreateErdDefinitionCommand, ErdDefinitionDto>
{
    private readonly IErdDefinitionRepository _erdDefinitionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateErdDefinitionCommandHandler(IErdDefinitionRepository erdDefinitionRepository, IUnitOfWork unitOfWork)
    {
        _erdDefinitionRepository = erdDefinitionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErdDefinitionDto> Handle(CreateErdDefinitionCommand request, CancellationToken cancellationToken)
    {
        var erdDefinition = ErdDefinition.Create(
            Guid.NewGuid(),
            request.Request.ModuleId,
            request.Request.TName,
            request.Request.Description,
            request.Request.EntityName,
            request.Request.AttributeName,
            request.Request.AttributeType,
            request.Request.IsPrimaryKey,
            request.Request.IsAcceptNull,
            request.Request.MaxChar,
            request.Request.SortOrder,
            ErdStatus.NotReady
        );

        await _erdDefinitionRepository.AddAsync(erdDefinition, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ErdDefinitionDto
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
        };
    }
}