using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Invites;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Invites.Queries.GetWorkspaceInviteByToken;

public sealed class GetWorkspaceInviteByTokenQueryHandler : IRequestHandler<GetWorkspaceInviteByTokenQuery, WorkspaceInviteInfoDto?>
{
    private readonly IWorkspaceInviteRepository _repository;

    public GetWorkspaceInviteByTokenQueryHandler(IWorkspaceInviteRepository repository)
    {
        _repository = repository;
    }

    public async Task<WorkspaceInviteInfoDto?> Handle(GetWorkspaceInviteByTokenQuery request, CancellationToken cancellationToken)
    {
        var invite = await _repository.GetByTokenAsync(request.Token, cancellationToken);
        if (invite is null)
        {
            return null;
        }

        return new WorkspaceInviteInfoDto
        {
            WorkspaceId = invite.WorkspaceId,
            Email = invite.Email.Value,
            Role = (int)invite.Role,
            Accepted = invite.Accepted,
        };
    }
}

