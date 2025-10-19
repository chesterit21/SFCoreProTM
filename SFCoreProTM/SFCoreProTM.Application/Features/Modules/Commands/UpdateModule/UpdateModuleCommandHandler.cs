using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Modules;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Modules.Commands.UpdateModule;

public class UpdateModuleCommandHandler : IRequestHandler<UpdateModuleCommand, ModuleDto>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateModuleCommandHandler(IModuleRepository moduleRepository, IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ModuleDto> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);
        
        if (module == null)
        {
            throw new Exception($"Module with ID {request.ModuleId} not found.");
        }

        module.UpdateDetails(
            request.Request.Name,
            request.Request.Description,
            request.Request.SortOrder
        );

        await _moduleRepository.UpdateAsync(module, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ModuleDto
        {
            Id = module.Id,
            WorkspaceId = module.WorkspaceId,
            ProjectId = module.ProjectId,
            Name = module.Name,
            Description = module.Description,
            SortOrder = module.SortOrder,
            Status = (int)module.Status
        };
    }
}