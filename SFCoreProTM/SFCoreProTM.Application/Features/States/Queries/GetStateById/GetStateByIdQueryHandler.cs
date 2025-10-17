using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using SFCoreProTM.Application.DTOs.States;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.States.Queries.GetStateById;

public sealed class GetStateByIdQueryHandler : IRequestHandler<GetStateByIdQuery, StateDto>
{
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IStateRepository _stateRepository;
    private readonly IMapper _mapper;

    public GetStateByIdQueryHandler(IWorkItemReadService workItemReadService, IStateRepository stateRepository, IMapper mapper)
    {
        _workItemReadService = workItemReadService;
        _stateRepository = stateRepository;
        _mapper = mapper;
    }

    public async Task<StateDto> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
    {
        var projectContext = await _workItemReadService.GetProjectContextAsync(request.ProjectId, cancellationToken);
        if (projectContext is null || projectContext.WorkspaceId != request.WorkspaceId)
        {
            throw new NotFoundException($"Project '{request.ProjectId}' was not found in workspace '{request.WorkspaceId}'.");
        }

        var state = await _stateRepository.GetByIdAsync(request.WorkspaceId, request.ProjectId, request.StateId, cancellationToken);
        if (state is null)
        {
            throw new NotFoundException($"State '{request.StateId}' was not found.");
        }

        return _mapper.Map<StateDto>(state);
    }
}
