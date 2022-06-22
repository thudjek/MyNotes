using Application.Common;
using Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Requests.Auth;

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
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = Mapper.MapTo<RegisterCommand>(request);
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
            return Ok();

        return Conflict(new { result.Error });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = Mapper.MapTo<LoginCommand>(request);
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
            return Ok(result.Value);

        return Unauthorized(new { result.Error });
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = Mapper.MapTo<RefreshTokenCommand>(request);
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
            return Ok(result.Value);

        return Unauthorized(new { result.Error });
    }

    [HttpPost]
    [Route("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeRefreshTokenRequest request)
    {
        var command = Mapper.MapTo<RevokeRefreshTokenCommand>(request);
        if (await Mediator.Send(command))
            return NoContent();

        return BadRequest();
    }

    [HttpPost]
    [Route("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var command = Mapper.MapTo<ConfirmEmailCommand>(request);
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
            return Ok();

        return BadRequest(new { result.Error });
    }

    [HttpPost]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var command = Mapper.MapTo<ForgotPasswordCommand>(request);
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var command = Mapper.MapTo<ResetPasswordCommand>(request);
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
    public async Task<IActionResult> ExternalLoginTokens([FromBody] ExternalLoginTokensRequest request)
    {
        var command = Mapper.MapTo<ExternalLoginTokensCommand>(request);
        var tokenResponse = await Mediator.Send(command);
        return Ok(tokenResponse);
    }
}
