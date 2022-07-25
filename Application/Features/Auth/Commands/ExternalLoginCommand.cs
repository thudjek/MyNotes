using Application.Common;
using Application.Common.Interfaces;
using Application.Dtos.Auth;
using MediatR;

namespace Application.Features.Auth.Commands;
public class ExternalLoginCommand : IRequest<Result<ExternalLoginInfoDto>>
{
}

public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, Result<ExternalLoginInfoDto>>
{
    private readonly IIdentityService _identityService;
    public ExternalLoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<ExternalLoginInfoDto>> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ExternalLogin();
    }
}