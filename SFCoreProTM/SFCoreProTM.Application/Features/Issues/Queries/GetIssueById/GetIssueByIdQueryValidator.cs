using FluentValidation;

namespace SFCoreProTM.Application.Features.Issues.Queries.GetIssueById;

public sealed class GetIssueByIdQueryValidator : AbstractValidator<GetIssueByIdQuery>
{
    public GetIssueByIdQueryValidator()
    {
        RuleFor(query => query.ProjectId).NotEmpty();
        RuleFor(query => query.IssueId).NotEmpty();
    }
}
