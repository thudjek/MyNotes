using Application.Common;
using Application.Common.Interfaces;
using Application.Dtos.Auth;
using MediatR;

namespace Application.Features.Auth.Commands;
public class LoginCommand : IRequest<Result<TokensDto>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokensDto>>
{
    private readonly IIdentityService _identityService;
    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<TokensDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.Login(request.Email, request.Password);
    }
}
