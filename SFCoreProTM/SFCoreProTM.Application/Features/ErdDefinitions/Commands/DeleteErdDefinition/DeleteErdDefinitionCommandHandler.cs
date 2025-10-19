using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.ErdDefinitions.Commands.DeleteErdDefinition;

public class DeleteErdDefinitionCommandHandler : IRequestHandler<DeleteErdDefinitionCommand, Unit>
{
    private readonly IErdDefinitionRepository _erdDefinitionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteErdDefinitionCommandHandler(IErdDefinitionRepository erdDefinitionRepository, IUnitOfWork unitOfWork)
    {
        _erdDefinitionRepository = erdDefinitionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteErdDefinitionCommand request, CancellationToken cancellationToken)
    {
        var erdDefinition = await _erdDefinitionRepository.GetByIdAsync(request.ErdDefinitionId, cancellationToken);
        
        if (erdDefinition == null)
        {
            throw new Exception($"ErdDefinition with ID {request.ErdDefinitionId} not found.");
        }

        await _erdDefinitionRepository.DeleteAsync(erdDefinition, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}