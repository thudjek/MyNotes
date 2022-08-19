using API.Extensions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Requests.Auth;

namespace API.Controllers;

public class AuthController : ApiBaseController
{
    private readonly IConfiguration _configuration;
    private readonly IDateTimeService _dateTimeService;
    public AuthController(IConfiguration configuration, IDateTimeService dateTimeService)
    {
        _configuration = configuration;
        _dateTimeService = dateTimeService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = Mapper.MapTo<RegisterCommand>(request);
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
            return Ok();

        return Conflict(result.ToErrorModel());
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = Mapper.MapTo<LoginCommand>(request);
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            HttpContext.Response.Cookies.Delete("refreshToken");
            HttpContext.AddCookieToResponse("refreshToken", result.Value.RefreshToken, true, _dateTimeService.Now.AddYears(1));
            return Ok(new { result.Value.AccessToken });
        }

        return BadRequest(result.ToErrorModel());
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = Mapper.MapTo<RefreshTokenCommand>(request);
        command.RefreshToken = HttpContext.GetValueFromCookie("refreshToken");
        HttpContext.Response.Cookies.Delete("refreshToken");
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            HttpContext.AddCookieToResponse("refreshToken", result.Value.RefreshToken, true, _dateTimeService.Now.AddYears(1));
            return Ok(result.Value);
        }

        return Unauthorized(result.ToErrorModel());
    }

    [Authorize]
    [HttpPost]
    [Route("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken()
    {
        if (await Mediator.Send(new RevokeRefreshTokenCommand()))
        {
            HttpContext.Response.Cookies.Delete("refreshToken");
            return NoContent();
        }

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

        return BadRequest(result.ToErrorModel());
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

        return BadRequest(result.ToErrorModel());
    }

    [HttpPost]
    [Route("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request)
    {
        var command = Mapper.MapTo<ResendConfirmationEmailCommand>(request);
        await Mediator.Send(command);
        return Ok();
    }

    [HttpGet]
    [Route("external-login/{provider}")]
    public IActionResult ExternalLogin([FromRoute] string provider)
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
            return BadRequest(result.ToErrorModel());

        return Redirect($"{_configuration["App:ExternalLoginReturnUrl"]}?email={result.Value.Email}&provider={result.Value.Provider}");
    }

    [HttpPost]
    [Route("external-login-tokens")]
    public async Task<IActionResult> ExternalLoginTokens([FromBody] ExternalLoginTokensRequest request)
    {
        var command = Mapper.MapTo<ExternalLoginTokensCommand>(request);
        var tokensDto = await Mediator.Send(command);
        if(!string.IsNullOrWhiteSpace(tokensDto.RefreshToken))
            HttpContext.AddCookieToResponse("refreshToken", tokensDto.RefreshToken, true, _dateTimeService.Now.AddYears(1));

        return Ok(new { tokensDto.AccessToken });
    }
}
