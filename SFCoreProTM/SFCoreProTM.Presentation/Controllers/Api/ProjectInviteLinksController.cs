using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Application.Features.Invites.Queries.GetProjectInviteByToken;
using SFCoreProTM.Application.Features.Invites.Commands.AcceptProjectInvite;
using SFCoreProTM.Application.Features.Users.Queries.GetCurrentUser;

namespace SFCoreProTM.Presentation.Controllers;

[ApiController]
[Route("api/invites/project")] 
public sealed class ProjectInviteLinksController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectInviteLinksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{token:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(Guid token, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetProjectInviteByTokenQuery(token), ct);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpPost("{token:guid}/accept")]
    [Authorize]
    public async Task<IActionResult> Accept(Guid token, CancellationToken ct)
    {
        var sub = User?.FindFirst("sub")?.Value;
        if (!Guid.TryParse(sub, out var userId)) return Unauthorized();
        var inviteInfo = await _mediator.Send(new GetProjectInviteByTokenQuery(token), ct);
        if (inviteInfo is null) return NotFound();
        var me = await _mediator.Send(new GetCurrentUserQuery(userId), ct);
        if (!string.Equals(inviteInfo.Email, me.Email, System.StringComparison.OrdinalIgnoreCase)) return Forbid();

        var ok = await _mediator.Send(new AcceptProjectInviteCommand(token, userId), ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}
