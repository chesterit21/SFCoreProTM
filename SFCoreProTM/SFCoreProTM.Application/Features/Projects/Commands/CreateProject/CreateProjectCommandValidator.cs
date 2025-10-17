using System.Collections.Generic;
using FluentValidation;

namespace SFCoreProTM.Application.Features.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(command => command.WorkspaceId).NotEmpty();
        RuleFor(command => command.ActorId).NotEmpty();
        RuleFor(command => command.Payload).NotNull();

        When(command => command.Payload is not null, () =>
        {
            RuleFor(command => command.Payload!.Name)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(command => command.Payload.Identifier)
                .NotEmpty()
                .MaximumLength(12);

            RuleFor(command => command.Payload.ArchiveInMonths)
                .InclusiveBetween(0, 12);

            RuleFor(command => command.Payload.CloseInMonths)
                .InclusiveBetween(0, 12);
        });
    }
}
