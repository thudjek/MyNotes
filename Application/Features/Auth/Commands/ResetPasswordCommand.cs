using Application.Common;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands;
public class ResetPasswordCommand : IRequest<Result>
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;
    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ResetPassword(request.Email, request.Token, request.Password);
    }
}