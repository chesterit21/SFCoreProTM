using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SFCoreProTM.Application.Interfaces;

namespace SFCoreProTM.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    private readonly IWorkspaceReadService _workspaceReadService;

    public CreateProjectCommandValidator(IWorkspaceReadService workspaceReadService)
    {
        _workspaceReadService = workspaceReadService;

        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .MustAsync(WorkspaceMustExist)
            .WithMessage("Workspace was not found.");

        RuleFor(x => x.ActorId)
            .NotEmpty()
            .MustAsync(async (cmd, actorId, ctx, ct) => await ActorIsMember(cmd.WorkspaceId, actorId, ct))
            .WithMessage("You must be a member of the workspace to create a project.");

        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .WithMessage("Project name is required.")
            .MaximumLength(255)
            .MustAsync(async (cmd, name, ctx, ct) => await NameIsUnique(cmd.WorkspaceId, name, ct))
            .WithMessage(cmd => $"Project name '{cmd.Payload.Name.Trim()}' is already in use.");
    }

    private async Task<bool> WorkspaceMustExist(System.Guid workspaceId, CancellationToken ct)
    {
        return await _workspaceReadService.WorkspaceExistsAsync(workspaceId, ct);
    }

    private async Task<bool> ActorIsMember(System.Guid workspaceId, System.Guid actorId, CancellationToken ct)
    {
        return await _workspaceReadService.IsMemberAsync(workspaceId, actorId, ct);
    }

    private async Task<bool> NameIsUnique(System.Guid workspaceId, string name, CancellationToken ct)
    {
        return !await _workspaceReadService.ProjectNameExistsAsync(workspaceId, name.Trim(), ct);
    }
}