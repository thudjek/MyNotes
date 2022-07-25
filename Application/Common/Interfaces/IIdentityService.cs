using Application.Dtos.Auth;
using Application.Enums;

namespace Application.Common.Interfaces;
public interface IIdentityService
{
    Task<Result> Register(string email, string password);
    Task<Result<TokensDto>> Login(string email, string password);
    Task<Result<TokensDto>> RefreshToken(string accessToken, string refreshToken);
    Task<bool> RevokeRefreshToken(string email);
    Task<Result<string>> GetTokenForIdentityPurpose(string email, TokenPurpose purpose);
    Task<Result> ConfirmEmail(string email, string token);
    Task<Result> ResetPassword(string email, string token, string password);
    Task<Result<ExternalLoginInfoDto>> ExternalLogin();
    Task<TokensDto> GetExternalLoginTokens(string email, string provider);
}
