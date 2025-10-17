using FluentValidation;

namespace SFCoreProTM.Application.Features.Issues.Commands.CreateIssue;

public sealed class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleFor(command => command.ProjectId)
            .NotEmpty();

        RuleFor(command => command.ActorId)
            .NotEmpty();

        RuleFor(command => command.Payload)
            .NotNull();

        RuleFor(command => command.Payload.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(command => command.Payload)
            .Must(payload => !(payload.StartDate.HasValue && payload.TargetDate.HasValue) || payload.StartDate <= payload.TargetDate)
            .WithMessage("Start date cannot be after target date.");
    }
}
