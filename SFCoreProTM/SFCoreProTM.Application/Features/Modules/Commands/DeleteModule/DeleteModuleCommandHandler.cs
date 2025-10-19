using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Modules.Commands.DeleteModule;

public class DeleteModuleCommandHandler : IRequestHandler<DeleteModuleCommand, bool>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteModuleCommandHandler(IModuleRepository moduleRepository, IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);
        
        if (module == null)
        {
            return false;
        }

        await _moduleRepository.DeleteAsync(module, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}