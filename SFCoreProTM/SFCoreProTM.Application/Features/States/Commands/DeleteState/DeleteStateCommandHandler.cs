using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.States.Commands.DeleteState;

public sealed class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand>
{
    private readonly IStateRepository _stateRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStateCommandHandler(
        IStateRepository stateRepository,
        IIssueRepository issueRepository,
        IUnitOfWork unitOfWork)
    {
        _stateRepository = stateRepository;
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteStateCommand request, CancellationToken cancellationToken)
    {
        var state = await _stateRepository.GetByIdAsync(request.WorkspaceId, request.ProjectId, request.StateId, cancellationToken);
        if (state is null)
        {
            throw new NotFoundException($"State '{request.StateId}' was not found.");
        }

        if (state.IsDefault)
        {
            throw new ValidationException("Default state cannot be deleted.");
        }

        var hasIssues = await _issueRepository.HasIssuesInStateAsync(request.WorkspaceId, request.ProjectId, request.StateId, cancellationToken);
        if (hasIssues)
        {
            throw new ValidationException("The state is not empty, only empty states can be deleted.");
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _stateRepository.DeleteAsync(state, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
