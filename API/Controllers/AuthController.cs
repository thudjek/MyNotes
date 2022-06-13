using Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AuthController : ApiBaseController
{
    private readonly IConfiguration _configuration;
    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var registerResultDto = await Mediator.Send(command);
        if (registerResultDto.Success)
            return Ok();

        return Conflict(new { registerResultDto.Error });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
            return Ok(result.Value);

        return Unauthorized(new { result.Error });
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
            return Ok(result.Value);

        return Unauthorized(new { result.Error });
    }

    [HttpPost]
    [Route("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeRefreshTokenCommand command)
    {
        if (await Mediator.Send(command))
            return NoContent();

        return BadRequest();
    }

    [HttpPost]
    [Route("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(new { result.Error });
    }

    [HttpPost]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(new { result.Error });
    }

    [HttpGet]
    [Route("external-login/{provider}")]
    public IActionResult GoogleLogin([FromRoute] string provider)
    {
        var properties = new AuthenticationProperties() { RedirectUri = Url.Action("ExternalLoginCallback"), AllowRefresh = true };
        properties.Items["LoginProvider"] = provider;
        return Challenge(properties, provider);
    }

    [HttpGet]
    [Route("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var command = new ExternalLoginCommand();
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Error });

        return Redirect($"{_configuration["App:ExternalLoginReturnUrl"]}{result.Value}"); //redirect to FE
    }

    [HttpPost]
    [Route("external-login-tokens")]
    public async Task<IActionResult> ExternalLoginTokens([FromBody] ExternalLoginTokensCommand command)
    {
        var tokenDto = await Mediator.Send(command);
        return Ok(tokenDto);
    }
}
