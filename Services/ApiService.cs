using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace MorgueManager.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        return await _http.GetFromJsonAsync<T>(endpoint);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        var response = await _http.PostAsJsonAsync(endpoint, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        var response = await _http.PutAsJsonAsync(endpoint, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task DeleteAsync(string endpoint)
    {
        var response = await _http.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
    }
}
