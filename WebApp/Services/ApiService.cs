using System.Net.Http.Json;

namespace WebApp.Services;

public class ApiService
{
	private readonly HttpClient _httpClient;
	public ApiService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

    public string GetBaseAddress()
    {
        return _httpClient.BaseAddress.ToString();
    }

	public async Task<TResponse> Get<TResponse>(string relativeUrl)
	{
        var httpResponse = await _httpClient.GetAsync(relativeUrl, HttpCompletionOption.ResponseHeadersRead);
        return await httpResponse.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse> Get<TResponse>(string relativeUrl, string queryString)
    {
        var httpResponse = await _httpClient.GetAsync(relativeUrl + queryString, HttpCompletionOption.ResponseHeadersRead);
        return await httpResponse.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse> Post<TRequest, TResponse>(string relativeUrl, TRequest request)
    {
        var httpResponse = await _httpClient.PostAsJsonAsync(relativeUrl, request);
        return await httpResponse.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse> Post<TResponse>(string relativeUrl)
    {
        var httpResponse = await _httpClient.PostAsync(relativeUrl, null);
        return await httpResponse.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task Post<TRequest>(string relativeUrl, TRequest request)
    {
        await _httpClient.PostAsJsonAsync(relativeUrl, request);
    }

    public async Task Post(string relativeUrl)
    {
        await _httpClient.PostAsync(relativeUrl, null);
    }

    public async Task<TResponse> Put<TRequest, TResponse>(string relativeUrl, TRequest request)
    {
        var httpResponse = await _httpClient.PutAsJsonAsync(relativeUrl, request);
        return await httpResponse.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task Put<TRequest>(string relativeUrl, TRequest request)
    {
        await _httpClient.PutAsJsonAsync(relativeUrl, request);
    }
}
