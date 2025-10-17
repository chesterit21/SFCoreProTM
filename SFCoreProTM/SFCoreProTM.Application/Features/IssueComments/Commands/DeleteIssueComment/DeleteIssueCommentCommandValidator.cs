using FluentValidation;

namespace SFCoreProTM.Application.Features.IssueComments.Commands.DeleteIssueComment;

public sealed class DeleteIssueCommentCommandValidator : AbstractValidator<DeleteIssueCommentCommand>
{
    public DeleteIssueCommentCommandValidator()
    {
        RuleFor(command => command.ProjectId).NotEmpty();
        RuleFor(command => command.IssueId).NotEmpty();
        RuleFor(command => command.CommentId).NotEmpty();
        RuleFor(command => command.ActorId).NotEmpty();
    }
}
