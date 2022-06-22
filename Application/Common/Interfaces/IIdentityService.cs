using Application.Enums;
using SharedModels.Responses.Auth;

namespace Application.Common.Interfaces;
public interface IIdentityService
{
    Task<Result> Register(string email, string password);
    Task<Result<TokenResponse>> Login(string email, string password);
    Task<Result<TokenResponse>> RefreshToken(string accessToken, string refreshToken);
    Task<bool> RevokeRefreshToken(string email);
    Task<Result<string>> GetTokenForIdentityPurpose(string email, TokenPurpose purpose);
    Task<Result> ConfirmEmail(string email, string token);
    Task<Result> ResetPassword(string email, string token, string password);
    Task<Result<string>> ExternalLogin();
    Task<TokenResponse> GetExternalLoginTokens(string email, string provider);
}
