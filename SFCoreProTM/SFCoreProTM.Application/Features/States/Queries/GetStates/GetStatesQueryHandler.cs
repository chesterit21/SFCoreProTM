using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using SFCoreProTM.Application.DTOs.States;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.States.Queries.GetStates;

public sealed class GetStatesQueryHandler : IRequestHandler<GetStatesQuery, IReadOnlyCollection<StateDto>>
{
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IStateRepository _stateRepository;
    private readonly IMapper _mapper;

    public GetStatesQueryHandler(IWorkItemReadService workItemReadService, IStateRepository stateRepository, IMapper mapper)
    {
        _workItemReadService = workItemReadService;
        _stateRepository = stateRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<StateDto>> Handle(GetStatesQuery request, CancellationToken cancellationToken)
    {
        var projectContext = await _workItemReadService.GetProjectContextAsync(request.ProjectId, cancellationToken);
        if (projectContext is null || projectContext.WorkspaceId != request.WorkspaceId)
        {
            throw new NotFoundException($"Project '{request.ProjectId}' was not found in workspace '{request.WorkspaceId}'.");
        }

        var states = await _stateRepository.GetByProjectAsync(request.WorkspaceId, request.ProjectId, cancellationToken);

        return states.Select(s => _mapper.Map<StateDto>(s)).ToList();
    }
}
