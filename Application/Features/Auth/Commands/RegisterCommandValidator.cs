using FluentValidation;

namespace Application.Features.Auth.Commands;
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not in correct format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must have at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must have at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must have at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must have at least one number");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("You must confirm password")
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}