using FluentValidation;

namespace SFCoreProTM.Application.Features.Authentication.Commands.SignIn;

public sealed class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty();
    }
}
