using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Dtos.Auth;
using Application.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Auth.Commands;
public class RegisterCommand : IRequest<RegisterResultDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResultDto>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    public RegisterCommandHandler(IIdentityService identityService, IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<RegisterResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registerResultDto = await _identityService.Register(request.Email, request.Password);

        var tokenResult = await _identityService.GetTokenForIdentityPurpose(request.Email, TokenPurpose.ConfirmEmail);

        if (tokenResult.IsSuccess)
            await _emailService.SendConfirmationEmail(request.Email, tokenResult.Value);

        return registerResultDto;
    }
}