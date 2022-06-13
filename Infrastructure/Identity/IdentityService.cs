using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Dtos.Auth;
using Application.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity;
public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IDateTimeService _dateTimeService;
    public IdentityService(UserManager<User> userManager, IConfiguration configuration, IDateTimeService dateTimeService, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _dateTimeService = dateTimeService;
        _signInManager = signInManager;
    }

    public async Task<RegisterResultDto> Register(string email, string password)
    {
        if (await _userManager.FindByEmailAsync(email) != null)
            return new RegisterResultDto() { Success = false, Error = "User with that email already exists" };

        var user = new User()
        {
            Email = email,
            UserName = email
        };

        var registerResult = await _userManager.CreateAsync(user, password);
        if (registerResult != IdentityResult.Success)
            throw new IdentityException(registerResult.Errors.Select(e => e.Description));

        return new RegisterResultDto() { Success = true }; ;
    }

    public async Task<Result<TokenDto>> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            return Result<TokenDto>.Failure("Wrong email or password");

        if(!user.EmailConfirmed)
            return Result<TokenDto>.Failure("Email is not confirmed");

        var claims = GetUserClaims(user);
        var tokenDto = await GetTokensForUser(user, claims);

        return Result<TokenDto>.Success(tokenDto);
    }

    public async Task<Result<TokenDto>> RefreshToken(string accessToken, string refreshToken)
    {
        var principal = GetClaimsPrincipalFromAccessToken(accessToken);
        if (principal == null)
            return Result<TokenDto>.Failure("Invalid access token or refresh token");

        var email = principal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(email);

        if(user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= _dateTimeService.Now)
            return Result<TokenDto>.Failure("Invalid access token or refresh token");

        var tokenDto = await GetTokensForUser(user, principal.Claims.ToList());

        return Result<TokenDto>.Success(tokenDto);
    }

    public async Task<bool> RevokeRefreshToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return false;

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<Result<string>> GetTokenForIdentityPurpose(string email, TokenPurpose purpose)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result<string>.Failure();

        string token = string.Empty;

        switch (purpose)
        {
            case TokenPurpose.ConfirmEmail:
                token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                break;
            case TokenPurpose.ResetPassword:
                token = await _userManager.GeneratePasswordResetTokenAsync(user);
                break;
        }

        return Result<string>.Success(token);
    }

    public async Task<Result<bool>> ConfirmEmail(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result<bool>.Failure("There was an error while processing the request");

        var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, token);

        return Result<bool>.Success(confirmEmailResult.Succeeded);
    }

    public async Task<Result<bool>> ResetPassword(string email, string token, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result<bool>.Failure("There was an error while processing the request");

        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, password);

        return Result<bool>.Success(resetPasswordResult.Succeeded);
    }

    public async Task<Result<string>> ExternalLogin()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (!info.Principal.Claims.Any())
            return Result<string>.Failure("Error while trying to login");

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(email);
        var claims = GetUserClaims(user);

        if (signInResult.Succeeded)
        {
            var tokenDto = await GetTokensForUser(user, claims);
            await _userManager.SetAuthenticationTokenAsync(user, info.LoginProvider, "AccessToken", tokenDto.AccessToken);
            await _userManager.SetAuthenticationTokenAsync(user, info.LoginProvider, "RefreshToken", tokenDto.RefreshToken);
            return Result<string>.Success(user.Email);
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            if (user == null)
            {
                user = new User()
                {
                    Email = email,
                    UserName = email
                };

                var createUserResult = await _userManager.CreateAsync(user);
                if (createUserResult != IdentityResult.Success)
                    throw new IdentityException(createUserResult.Errors.Select(e => e.Description));

            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (addLoginResult != IdentityResult.Success)
                throw new IdentityException(addLoginResult.Errors.Select(e => e.Description));

            await _signInManager.SignInAsync(user, false);

            var tokenDto = await GetTokensForUser(user, claims);

            return Result<string>.Success(user.Email);
        }

        return Result<string>.Failure("Error while trying to login");
    }

    public async Task<TokenDto> GetExternalLoginTokens(string email, string provider)
    {
        var tokenDto = new TokenDto();

        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(provider))
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                tokenDto.AccessToken = await _userManager.GetAuthenticationTokenAsync(user, provider, "AccessToken");
                tokenDto.RefreshToken = await _userManager.GetAuthenticationTokenAsync(user, provider, "RefreshToken");

                await _userManager.RemoveAuthenticationTokenAsync(user, provider, "AccessToken");
                await _userManager.RemoveAuthenticationTokenAsync(user, provider, "RefreshToken");
            }
        }

        return tokenDto;
    }

    private static List<Claim> GetUserClaims(User user)
    {
        var claims = new List<Claim>();

        if (user != null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        return claims;
    }

    private async Task<TokenDto> GetTokensForUser(User user, List<Claim> claims)
    {
        var accessToken = GenerateAccessToken(claims);
        var refreshToken = GenerateRefreshToken();

        _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _dateTimeService.Now.AddDays(refreshTokenValidityInDays);

        await _userManager.UpdateAsync(user);

        return new TokenDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private string GenerateAccessToken(List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        _ = int.TryParse(_configuration["JWT:AccessTokenValidityInMinutes"], out int tokenValidityInMinutes);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: _dateTimeService.Now.AddMinutes(tokenValidityInMinutes),
            claims: claims,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetClaimsPrincipalFromAccessToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid access token");

        return principal;
    }
}