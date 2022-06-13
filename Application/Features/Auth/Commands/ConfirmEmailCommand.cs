using Application.Common;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands;
public class ConfirmEmailCommand : IRequest<Result<bool>>
{
    public string Email { get; set; }
    public string Token { get; set; }
}

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<bool>>
{
    private readonly IIdentityService _identityService;
    public ConfirmEmailCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ConfirmEmail(request.Email, request.Token);
    }
}