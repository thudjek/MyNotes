using Application.Common.Interfaces;
using Application.Enums;
using MediatR;

namespace Application.Features.Auth.Commands;
public class ForgotPasswordCommand : IRequest
{
    public string Email { get; set; }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    public ForgotPasswordCommandHandler(IIdentityService identityService, IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var tokenResult = await _identityService.GetTokenForIdentityPurpose(request.Email, TokenPurpose.ResetPassword);
        if(tokenResult.IsSuccess)
            await _emailService.SendPasswordResetEmail(request.Email, tokenResult.Value);

        return Unit.Value;
    }
}