using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Application.Features.Users.Queries.GetCurrentUser;

namespace SFCoreProTM.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) { _mediator = mediator; }

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var sub = User?.FindFirst("sub")?.Value;
        if (!Guid.TryParse(sub, out var userId)) return Unauthorized();
        var dto = await _mediator.Send(new GetCurrentUserQuery(userId), ct);
        return Ok(dto);
    }
}

