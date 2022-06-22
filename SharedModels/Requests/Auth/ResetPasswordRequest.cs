namespace SharedModels.Requests.Auth;
public class ResetPasswordRequest
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}