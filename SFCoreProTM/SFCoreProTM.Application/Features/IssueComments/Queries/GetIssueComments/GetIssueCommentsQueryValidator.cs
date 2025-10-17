using FluentValidation;

namespace SFCoreProTM.Application.Features.IssueComments.Queries.GetIssueComments;

public sealed class GetIssueCommentsQueryValidator : AbstractValidator<GetIssueCommentsQuery>
{
    public GetIssueCommentsQueryValidator()
    {
        RuleFor(query => query.ProjectId).NotEmpty();
        RuleFor(query => query.IssueId).NotEmpty();
    }
}
