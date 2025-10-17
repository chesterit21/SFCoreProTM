using FluentValidation;

namespace SFCoreProTM.Application.Features.States.Commands.UpdateState;

public sealed class UpdateStateCommandValidator : AbstractValidator<UpdateStateCommand>
{
    public UpdateStateCommandValidator()
    {
        RuleFor(command => command.WorkspaceId).NotEmpty();
        RuleFor(command => command.ProjectId).NotEmpty();
        RuleFor(command => command.StateId).NotEmpty();
        RuleFor(command => command.ActorId).NotEmpty();
        RuleFor(command => command.Payload).NotNull();

        When(command => command.Payload is not null, () =>
        {
            RuleFor(command => command.Payload!.ColorHex)
                .Matches("^#(?:[0-9a-fA-F]{3}){1,2}$")
                .When(command => !string.IsNullOrWhiteSpace(command.Payload?.ColorHex))
                .WithMessage("Color must be a valid hex value.");
        });
    }
}
