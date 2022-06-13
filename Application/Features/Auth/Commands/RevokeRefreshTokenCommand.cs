using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands;
public class RevokeRefreshTokenCommand : IRequest<bool>
{
    public string Email { get; set; }
}

public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand, bool>
{
    private readonly IIdentityService _identityService;
    public RevokeRefreshTokenHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RevokeRefreshToken(request.Email);
    }
}