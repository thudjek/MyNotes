using Application.Common.Interfaces;
using Application.Enums;
using MediatR;

namespace Application.Features.Auth.Commands;
public class ResendConfirmationEmailCommand : IRequest
{
    public string Email { get; set; }
}

public class ResendConfirmationEmailCommandHandler : IRequestHandler<ResendConfirmationEmailCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    public ResendConfirmationEmailCommandHandler(IIdentityService identityService, IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        var tokenResult = await _identityService.GetTokenForIdentityPurpose(request.Email, TokenPurpose.ConfirmEmail);

        if (tokenResult.IsSuccess)
            await _emailService.SendConfirmationEmail(request.Email, tokenResult.Value);

        return Unit.Value;
    }
}