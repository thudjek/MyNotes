using Application.Common;
using Application.Common.Interfaces;
using MediatR;
using SharedModels.Responses.Auth;

namespace Application.Features.Auth.Commands;
public class LoginCommand : IRequest<Result<TokenResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly IIdentityService _identityService;
    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.Login(request.Email, request.Password);
    }
}
