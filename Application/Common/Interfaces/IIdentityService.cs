using Application.Dtos.Auth;
using Application.Enums;

namespace Application.Common.Interfaces;
public interface IIdentityService
{
    Task<RegisterResultDto> Register(string email, string password);
    Task<Result<TokenDto>> Login(string email, string password);
    Task<Result<TokenDto>> RefreshToken(string accessToken, string refreshToken);
    Task<bool> RevokeRefreshToken(string email);
    Task<Result<string>> GetTokenForIdentityPurpose(string email, TokenPurpose purpose);
    Task<Result<bool>> ConfirmEmail(string email, string token);
    Task<Result<bool>> ResetPassword(string email, string token, string password);
    Task<Result<string>> ExternalLogin();
    Task<TokenDto> GetExternalLoginTokens(string email, string provider);
}
