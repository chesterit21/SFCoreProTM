using FluentValidation;

namespace SFCoreProTM.Application.Features.States.Queries.GetStateById;

public sealed class GetStateByIdQueryValidator : AbstractValidator<GetStateByIdQuery>
{
    public GetStateByIdQueryValidator()
    {
        RuleFor(query => query.WorkspaceId).NotEmpty();
        RuleFor(query => query.ProjectId).NotEmpty();
        RuleFor(query => query.StateId).NotEmpty();
    }
}
