using FluentValidation;

namespace SFCoreProTM.Application.Features.States.Queries.GetStates;

public sealed class GetStatesQueryValidator : AbstractValidator<GetStatesQuery>
{
    public GetStatesQueryValidator()
    {
        RuleFor(query => query.WorkspaceId).NotEmpty();
        RuleFor(query => query.ProjectId).NotEmpty();
    }
}
