using System;
using System.Collections.Generic;
using FluentValidation;

namespace SFCoreProTM.Application.Features.IssueComments.Commands.UpdateIssueComment;

public sealed class UpdateIssueCommentCommandValidator : AbstractValidator<UpdateIssueCommentCommand>
{
    public UpdateIssueCommentCommandValidator()
    {
        RuleFor(command => command.ProjectId).NotEmpty();
        RuleFor(command => command.IssueId).NotEmpty();
        RuleFor(command => command.CommentId).NotEmpty();
        RuleFor(command => command.Payload).NotNull();

        When(command => command.Payload is not null, () =>
        {
            RuleFor(command => command.Payload!.ActorId).NotEmpty();
            RuleFor(command => command.Payload.CommentHtml)
                .MaximumLength(10_000);
            RuleFor(command => command.Payload.CommentPlainText)
                .MaximumLength(5_000);
            RuleForEach(command => command.Payload.Attachments ?? new List<string>())
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Attachment must be a valid absolute URL.");
        });
    }
}
