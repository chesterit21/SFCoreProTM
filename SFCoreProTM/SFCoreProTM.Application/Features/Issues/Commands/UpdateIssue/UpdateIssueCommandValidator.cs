using FluentValidation;

namespace SFCoreProTM.Application.Features.Issues.Commands.UpdateIssue;

public sealed class UpdateIssueCommandValidator : AbstractValidator<UpdateIssueCommand>
{
    public UpdateIssueCommandValidator()
    {
        RuleFor(command => command.ProjectId).NotEmpty();
        RuleFor(command => command.IssueId).NotEmpty();
        RuleFor(command => command.ActorId).NotEmpty();

        RuleFor(command => command.Payload)
            .NotNull();

        When(command => command.Payload is not null, () =>
        {
            RuleFor(command => command.Payload!.Name)
                .MaximumLength(255);

            RuleFor(command => command.Payload)
                .Must(payload => !(payload.StartDate.HasValue && payload.TargetDate.HasValue) || payload.StartDate <= payload.TargetDate)
                .WithMessage("Start date cannot be after target date.");
        });
    }
}
