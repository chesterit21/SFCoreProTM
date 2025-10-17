using FluentValidation;

namespace SFCoreProTM.Application.Features.Issues.Commands.DeleteIssue;

public sealed class DeleteIssueCommandValidator : AbstractValidator<DeleteIssueCommand>
{
    public DeleteIssueCommandValidator()
    {
        RuleFor(command => command.ProjectId).NotEmpty();
        RuleFor(command => command.IssueId).NotEmpty();
        RuleFor(command => command.ActorId).NotEmpty();
    }
}
