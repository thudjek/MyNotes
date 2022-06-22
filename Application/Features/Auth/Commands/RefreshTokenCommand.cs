using Application.Common;
using Application.Common.Interfaces;
using MediatR;
using SharedModels.Responses.Auth;

namespace Application.Features.Auth.Commands;
public class RefreshTokenCommand : IRequest<Result<TokenResponse>>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly IIdentityService _identityService;
    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return _identityService.RefreshToken(request.AccessToken, request.RefreshToken);
    }
}