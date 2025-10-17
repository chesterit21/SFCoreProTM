using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Invites;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Invites.Queries.GetProjectInviteByToken;

public sealed class GetProjectInviteByTokenQueryHandler : IRequestHandler<GetProjectInviteByTokenQuery, ProjectInviteCreatedDto?>
{
    private readonly IProjectInviteRepository _repository;

    public GetProjectInviteByTokenQueryHandler(IProjectInviteRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProjectInviteCreatedDto?> Handle(GetProjectInviteByTokenQuery request, CancellationToken cancellationToken)
    {
        var invite = await _repository.GetByTokenAsync(request.Token, cancellationToken);
        if (invite is null)
        {
            return null;
        }

        return new ProjectInviteCreatedDto
        {
            WorkspaceId = invite.WorkspaceId,
            ProjectId = invite.ProjectId,
            Email = invite.Email.Value,
            Role = (int)invite.Role,
            Token = invite.Token,
        };
    }
}

