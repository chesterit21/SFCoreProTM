using FluentValidation;

namespace SFCoreProTM.Application.Features.Authentication.Commands.SignUp;

public sealed class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(command => command.DisplayName)
            .MaximumLength(255)
            .When(command => !string.IsNullOrWhiteSpace(command.DisplayName));

        RuleFor(command => command.FirstName)
            .MaximumLength(255)
            .When(command => !string.IsNullOrWhiteSpace(command.FirstName));

        RuleFor(command => command.LastName)
            .MaximumLength(255)
            .When(command => !string.IsNullOrWhiteSpace(command.LastName));
    }
}
