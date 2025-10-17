using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFCoreProTM.Application.Features.Invites.Commands.CreateWorkspaceInvite;
using SFCoreProTM.Presentation.Options;
using SFCoreProTM.Application.Interfaces;

namespace SFCoreProTM.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/workspaces/{workspaceId:guid}/invites")]
public sealed class WorkspaceInvitesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly InstanceOptions _instanceOptions;
    private readonly IWorkspaceReadService _workspaceReadService;

    public WorkspaceInvitesController(IMediator mediator, IOptions<InstanceOptions> instanceOptions, IWorkspaceReadService workspaceReadService)
    {
        _mediator = mediator;
        _instanceOptions = instanceOptions.Value;
        _workspaceReadService = workspaceReadService;
    }

    public sealed class CreateInviteBody
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Range(0, 3)]
        public int Role { get; set; } = 1;

        public string? Message { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid workspaceId, [FromBody] CreateInviteBody body, CancellationToken ct)
    {
        if (!_instanceOptions.InvitesEnabled)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        // Enforce instance email allowlist/domains for invited email
        var email = body.Email.Trim().ToLowerInvariant();
        var at = email.LastIndexOf('@');
        var domain = at >= 0 ? email[(at + 1)..] : string.Empty;
        if (_instanceOptions.WhitelistEmails != null && _instanceOptions.WhitelistEmails.Length > 0)
        {
            var ok = System.Array.Exists(_instanceOptions.WhitelistEmails, e => string.Equals(e?.Trim().ToLowerInvariant(), email, System.StringComparison.Ordinal));
            if (!ok) return Forbid();
        }
        if (_instanceOptions.AllowedEmailDomains != null && _instanceOptions.AllowedEmailDomains.Length > 0)
        {
            var ok = System.Array.Exists(_instanceOptions.AllowedEmailDomains, d => string.Equals(d?.Trim().ToLowerInvariant(), domain, System.StringComparison.Ordinal));
            if (!ok) return Forbid();
        }

        var actorId = GetUserId();
        if (actorId == Guid.Empty)
        {
            return Unauthorized();
        }

        var command = new CreateWorkspaceInviteCommand(workspaceId, actorId, email, body.Role, body.Message);
        var created = await _mediator.Send(command, ct);
        return Created($"/api/invites/workspace/{created.Token}", created);
    }

    [HttpGet]
    public async Task<IActionResult> List(Guid workspaceId, CancellationToken ct)
    {
        var actorId = GetUserId();
        if (!await _workspaceReadService.IsAdminAsync(workspaceId, actorId, ct)) return Forbid();
        var list = await _mediator.Send(new SFCoreProTM.Application.Features.Invites.Queries.GetWorkspaceInvites.GetWorkspaceInvitesQuery(workspaceId), ct);
        return Ok(list);
    }

    private Guid GetUserId()
    {
        var sub = User?.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }
}
