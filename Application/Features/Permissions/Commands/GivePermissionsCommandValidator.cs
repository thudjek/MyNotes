using Domain.Enums;
using FluentValidation;

namespace Application.Features.Permissions.Commands;
public class GivePermissionsCommandValidator : AbstractValidator<GivePermissionsCommand>
{
    public GivePermissionsCommandValidator()
    {
        RuleFor(x => x.Permissions)
            .NotEmpty().WithMessage("Email and permissions must be chosen")
            .Must(x => x.All(y => !string.IsNullOrWhiteSpace(y.Email) && y.Scope != PermissionScope.None)).WithMessage("Email and permissions must be chosen");
    }
}