using Microsoft.JSInterop;

namespace MorgueManager.Web.Services;

public class AuthService
{
    private readonly IJSRuntime _js;
    private static string? _memoryToken;

    public AuthService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<bool> IsLoggedInAsync()
    {
        if (!string.IsNullOrEmpty(_memoryToken))
            return true;

        try
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "morguemanager-token");
            if (!string.IsNullOrEmpty(token))
            {
                _memoryToken = token;
                return true;
            }
        }
        catch { }

        return false;
    }

    public async Task LoginAsync(string token)
    {
        _memoryToken = token;
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "morguemanager-token", token);
        }
        catch { }
    }

    public async Task LogoutAsync()
    {
        _memoryToken = null;
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "morguemanager-token");
        }
        catch { }
    }
}
