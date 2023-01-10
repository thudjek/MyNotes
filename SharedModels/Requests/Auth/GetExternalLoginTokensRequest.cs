namespace SharedModels.Requests.Auth;
public class GetExternalLoginTokensRequest
{
    public string Email { get; set; }
    public string Provider { get; set; }
}