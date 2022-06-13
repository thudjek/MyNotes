using Application.Common;
using Application.Common.Interfaces;
using Application.Dtos.Auth;
using MediatR;

namespace Application.Features.Auth.Commands;
public class ExternalLoginCommand : IRequest<Result<string>>
{
}

public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, Result<string>>
{
    private readonly IIdentityService _identityService;
    public ExternalLoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<string>> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ExternalLogin();
    }
}