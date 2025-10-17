using System.Collections.Generic;
using FluentValidation;

namespace SFCoreProTM.Application.Features.States.Commands.CreateState;

public sealed class CreateStateCommandValidator : AbstractValidator<CreateStateCommand>
{
    public CreateStateCommandValidator()
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

            RuleFor(command => command.Payload.Group)
                .NotEmpty();
        });
    }
}
