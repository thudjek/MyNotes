using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands;
public class RevokeRefreshTokenCommand : IRequest<bool>
{
}

public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    public RevokeRefreshTokenHandler(IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RevokeRefreshToken(_currentUserService.Email);
    }
}