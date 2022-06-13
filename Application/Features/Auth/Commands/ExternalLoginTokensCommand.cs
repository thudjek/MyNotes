using Application.Common.Interfaces;
using Application.Dtos.Auth;
using MediatR;

namespace Application.Features.Auth.Commands;
public class ExternalLoginTokensCommand : IRequest<TokenDto>
{
    public string Email { get; set; }
    public string Provider { get; set; }
}

public class ExternalLoginTokensCommandHandler : IRequestHandler<ExternalLoginTokensCommand, TokenDto>
{
    private readonly IIdentityService _identityService;
    public ExternalLoginTokensCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<TokenDto> Handle(ExternalLoginTokensCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.GetExternalLoginTokens(request.Email, request.Provider);
    }
}