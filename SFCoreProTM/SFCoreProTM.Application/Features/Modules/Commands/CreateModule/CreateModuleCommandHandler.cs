using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Modules;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.Modules.Commands.CreateModule;

public class CreateModuleCommandHandler : IRequestHandler<CreateModuleCommand, ModuleDto>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateModuleCommandHandler(IModuleRepository moduleRepository, IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ModuleDto> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
    {
        var module = Module.Create(
            Guid.NewGuid(),
            request.Request.WorkspaceId,
            request.Request.ProjectId,
            request.Request.Name,
            request.Request.Description,
            request.Request.SortOrder
        );

        await _moduleRepository.AddAsync(module, cancellationToken);
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