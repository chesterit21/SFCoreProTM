using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SFCoreProTM.Application.DTOs.Issues;
using SFCoreProTM.Application.Features.IssueComments.Commands.CreateIssueComment;
using SFCoreProTM.Application.Features.IssueComments.Commands.DeleteIssueComment;
using SFCoreProTM.Application.Features.IssueComments.Commands.UpdateIssueComment;
using SFCoreProTM.Application.Features.IssueComments.Queries.GetIssueCommentById;
using SFCoreProTM.Application.Features.IssueComments.Queries.GetIssueComments;
using SFCoreProTM.Application.Features.Issues.Commands.CreateIssue;
using SFCoreProTM.Application.Features.Issues.Commands.UpdateIssue;
using SFCoreProTM.Application.Features.Issues.Commands.DeleteIssue;
using SFCoreProTM.Application.Features.Issues.Queries.GetIssueById;
using SFCoreProTM.Application.Mapping.Requests.Issues;

namespace SFCoreProTM.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:guid}/issues")]
public class IssuesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public IssuesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{issueId:guid}")]
    public async Task<IActionResult> GetIssue(Guid projectId, Guid issueId, CancellationToken cancellationToken)
    {
        var query = new GetIssueByIdQuery(projectId, issueId);
        var issue = await _mediator.Send(query, cancellationToken);
        return Ok(issue);
    }

    [HttpPost]
    public async Task<IActionResult> CreateIssue(Guid projectId, [FromBody] CreateIssueRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = _mapper.Map<CreateIssueRequestDto>(request);

        var command = new CreateIssueCommand(projectId, request.ActorId, dto);
        var createdIssue = await _mediator.Send(command, cancellationToken);

        var location = Url.Action(nameof(GetIssue), null, new { projectId, issueId = createdIssue.Id }, Request.Scheme) ?? $"/api/projects/{projectId}/issues/{createdIssue.Id}";

        return Created(location, createdIssue);
    }

    [HttpPut("{issueId:guid}")]
    public async Task<IActionResult> UpdateIssue(Guid projectId, Guid issueId, [FromBody] UpdateIssueRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = _mapper.Map<UpdateIssueRequestDto>(request);

        var command = new UpdateIssueCommand(projectId, issueId, request.ActorId, dto);
        var updatedIssue = await _mediator.Send(command, cancellationToken);

        return Ok(updatedIssue);
    }

    [HttpDelete("{issueId:guid}")]
    public async Task<IActionResult> DeleteIssue(Guid projectId, Guid issueId, [FromQuery] Guid actorId, CancellationToken cancellationToken)
    {
        if (actorId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(actorId), "ActorId is required.");
            return ValidationProblem(ModelState);
        }

        var command = new DeleteIssueCommand(projectId, issueId, actorId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("{issueId:guid}/comments")]
    public async Task<IActionResult> CreateIssueComment(Guid projectId, Guid issueId, [FromBody] CreateIssueCommentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = _mapper.Map<CreateIssueCommentRequestDto>(request);

        var command = new CreateIssueCommentCommand(projectId, issueId, dto);
        var comment = await _mediator.Send(command, cancellationToken);

        return Created($"/api/projects/{projectId}/issues/{issueId}/comments/{comment.Id}", comment);
    }

    [HttpGet("{issueId:guid}/comments")]
    public async Task<IActionResult> GetIssueComments(Guid projectId, Guid issueId, CancellationToken cancellationToken)
    {
        var query = new GetIssueCommentsQuery(projectId, issueId);
        var comments = await _mediator.Send(query, cancellationToken);
        return Ok(comments);
    }

    [HttpGet("{issueId:guid}/comments/{commentId:guid}")]
    public async Task<IActionResult> GetIssueComment(Guid projectId, Guid issueId, Guid commentId, CancellationToken cancellationToken)
    {
        var query = new GetIssueCommentByIdQuery(projectId, issueId, commentId);
        var comment = await _mediator.Send(query, cancellationToken);
        return Ok(comment);
    }

    [HttpPut("{issueId:guid}/comments/{commentId:guid}")]
    public async Task<IActionResult> UpdateIssueComment(Guid projectId, Guid issueId, Guid commentId, [FromBody] UpdateIssueCommentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = _mapper.Map<UpdateIssueCommentRequestDto>(request);

        var command = new UpdateIssueCommentCommand(projectId, issueId, commentId, dto);
        var comment = await _mediator.Send(command, cancellationToken);
        return Ok(comment);
    }

    [HttpDelete("{issueId:guid}/comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteIssueComment(Guid projectId, Guid issueId, Guid commentId, [FromQuery] Guid actorId, CancellationToken cancellationToken)
    {
        if (actorId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(actorId), "ActorId is required.");
            return ValidationProblem(ModelState);
        }

        var command = new DeleteIssueCommentCommand(projectId, issueId, commentId, actorId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
