using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Application.Features.Invites.Queries.GetWorkspaceInviteByToken;
using SFCoreProTM.Application.Features.Invites.Commands.AcceptWorkspaceInvite;

namespace SFCoreProTM.Presentation.Controllers;

[ApiController]
[Route("api/invites")]
public sealed class InvitesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvitesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("workspace/{token:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWorkspaceInvite(Guid token, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetWorkspaceInviteByTokenQuery(token), ct);
        if (dto is null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPost("workspace/{token:guid}/accept")]
    [Authorize]
    public async Task<IActionResult> AcceptWorkspaceInvite(Guid token, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var inviteInfo = await _mediator.Send(new GetWorkspaceInviteByTokenQuery(token), ct);
        if (inviteInfo is null) return NotFound();
        var me = await _mediator.Send(new SFCoreProTM.Application.Features.Users.Queries.GetCurrentUser.GetCurrentUserQuery(userId), ct);
        if (!string.Equals(inviteInfo.Email, me.Email, System.StringComparison.OrdinalIgnoreCase)) return Forbid();

        var ok = await _mediator.Send(new AcceptWorkspaceInviteCommand(token, userId), ct);
        if (!ok)
        {
            return NotFound();
        }
        return NoContent();
    }

    private Guid GetUserId()
    {
        var sub = User?.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }
}

