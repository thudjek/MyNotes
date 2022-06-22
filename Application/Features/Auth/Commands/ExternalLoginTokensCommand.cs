using Application.Common.Interfaces;
using MediatR;
using SharedModels.Responses.Auth;

namespace Application.Features.Auth.Commands;
public class ExternalLoginTokensCommand : IRequest<TokenResponse>
{
    public string Email { get; set; }
    public string Provider { get; set; }
}

public class ExternalLoginTokensCommandHandler : IRequestHandler<ExternalLoginTokensCommand, TokenResponse>
{
    private readonly IIdentityService _identityService;
    public ExternalLoginTokensCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<TokenResponse> Handle(ExternalLoginTokensCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.GetExternalLoginTokens(request.Email, request.Provider);
    }
}