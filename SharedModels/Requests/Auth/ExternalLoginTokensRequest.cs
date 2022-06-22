namespace SharedModels.Requests.Auth;
public class ExternalLoginTokensRequest
{
    public string Email { get; set; }
    public string Provider { get; set; }
}