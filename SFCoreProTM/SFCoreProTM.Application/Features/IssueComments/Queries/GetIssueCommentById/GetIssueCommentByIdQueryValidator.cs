using FluentValidation;

namespace SFCoreProTM.Application.Features.IssueComments.Queries.GetIssueCommentById;

public sealed class GetIssueCommentByIdQueryValidator : AbstractValidator<GetIssueCommentByIdQuery>
{
    public GetIssueCommentByIdQueryValidator()
    {
        RuleFor(query => query.ProjectId).NotEmpty();
        RuleFor(query => query.IssueId).NotEmpty();
        RuleFor(query => query.CommentId).NotEmpty();
    }
}
