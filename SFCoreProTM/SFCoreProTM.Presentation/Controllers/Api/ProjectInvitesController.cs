using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFCoreProTM.Application.Features.Invites.Commands.CreateProjectInvite;
using SFCoreProTM.Presentation.Options;

namespace SFCoreProTM.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:guid}/invites")]
public sealed class ProjectInvitesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly InstanceOptions _instanceOptions;

    public ProjectInvitesController(IMediator mediator, IOptions<InstanceOptions> instanceOptions)
    {
        _mediator = mediator;
        _instanceOptions = instanceOptions.Value;
    }

    public sealed class CreateInviteBody
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public Guid WorkspaceId { get; set; }

        [Range(0, 100)]
        public int Role { get; set; } = 15; // default Member

        public string? Message { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateInviteBody body, CancellationToken ct)
    {
        if (!_instanceOptions.InvitesEnabled)
        {
            return Forbid();
        }
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var actorId = GetUserId();
        if (actorId == Guid.Empty) return Unauthorized();

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

        var cmd = new CreateProjectInviteCommand(body.WorkspaceId, projectId, actorId, email, body.Role, body.Message);
        var created = await _mediator.Send(cmd, ct);
        return Created($"/api/invites/project/{created.Token}", created);
    }

    [HttpGet]
    public async Task<IActionResult> List(Guid projectId, [FromQuery] Guid workspaceId, CancellationToken ct)
    {
        if (workspaceId == Guid.Empty) return ValidationProblem("workspaceId is required");
        var list = await _mediator.Send(new SFCoreProTM.Application.Features.Invites.Queries.GetProjectInvites.GetProjectInvitesQuery(workspaceId, projectId), ct);
        return Ok(list);
    }

    private Guid GetUserId()
    {
        var sub = User?.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }
}
