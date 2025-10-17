using FluentValidation;

namespace SFCoreProTM.Application.Features.States.Commands.DeleteState;

public sealed class DeleteStateCommandValidator : AbstractValidator<DeleteStateCommand>
{
    public DeleteStateCommandValidator()
    {
        RuleFor(command => command.WorkspaceId).NotEmpty();
        RuleFor(command => command.ProjectId).NotEmpty();
        RuleFor(command => command.StateId).NotEmpty();
        RuleFor(command => command.ActorId).NotEmpty();
    }
}
