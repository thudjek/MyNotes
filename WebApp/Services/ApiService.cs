namespace WebApp.Services;

public class ApiService
{
	private readonly HttpClient _httpClient;
	public ApiService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}
}
