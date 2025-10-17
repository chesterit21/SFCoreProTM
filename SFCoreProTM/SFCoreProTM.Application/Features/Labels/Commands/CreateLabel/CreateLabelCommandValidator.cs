using FluentValidation;

namespace SFCoreProTM.Application.Features.Labels.Commands.CreateLabel;

public sealed class CreateLabelCommandValidator : AbstractValidator<CreateLabelCommand>
{
    public CreateLabelCommandValidator()
    {
        RuleFor(command => command.WorkspaceId).NotEmpty();
        RuleFor(command => command.ProjectId).NotEmpty();
        RuleFor(command => command.ActorId).NotEmpty();
        RuleFor(command => command.Payload).NotNull();

        When(command => command.Payload is not null, () =>
        {
            RuleFor(command => command.Payload!.Name)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(command => command.Payload.ColorHex)
                .NotEmpty()
                .Matches("^#(?:[0-9a-fA-F]{3}){1,2}$")
                .WithMessage("Color must be a valid hex value.");
        });
    }
}
