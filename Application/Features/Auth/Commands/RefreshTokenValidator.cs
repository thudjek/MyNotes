using FluentValidation;

namespace Application.Features.Auth.Commands;
public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is empty");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is empty");
    }
}